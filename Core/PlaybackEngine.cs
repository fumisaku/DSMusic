using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NAudio.Wave;

namespace DSMusic.Core;

public class PeakCacheEntry
{
    public long FileSize { get; set; }
    public string LastWriteTime { get; set; } = "";
    public float Peak { get; set; }
    public float Gain { get; set; }
}

public class LoopStream : WaveStream
{
    private readonly WaveStream _sourceStream;

    public LoopStream(WaveStream sourceStream)
    {
        _sourceStream = sourceStream;
        EnableLooping = true;
    }

    public bool EnableLooping { get; set; }

    public WaveStream SourceStream => _sourceStream;

    public override long Length => _sourceStream.Length;

    public override long Position
    {
        get => _sourceStream.Position;
        set => _sourceStream.Position = value;
    }

    public override WaveFormat WaveFormat => _sourceStream.WaveFormat;

    public override int Read(byte[] buffer, int offset, int count)
    {
        int totalBytesRead = 0;

        while (totalBytesRead < count)
        {
            int bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

            if (bytesRead == 0)
            {
                if (_sourceStream.Position == 0 || !EnableLooping)
                {
                    break;
                }
                _sourceStream.Position = 0;
            }
            totalBytesRead += bytesRead;
        }

        return totalBytesRead;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sourceStream.Dispose();
        }
        base.Dispose(disposing);
    }
}

public class PlaybackEngine : IDisposable
{
    private static readonly string PeakCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DSMusic_PeakCache.json");
    private Dictionary<string, PeakCacheEntry> _peakCache = new(StringComparer.OrdinalIgnoreCase);

    private readonly object _lock = new();

    /// <summary>競技曲チャンネル（A）が自然終了（ファイル末尾到達）したときに発火する。</summary>
    public event Action? CompetitionNaturallyEnded;

    // VU メーターサンプラー（各チャンネル）
    private VuMeterSampler? _vuSamplerA;
    private VuMeterSampler? _vuSamplerB;
    private VuMeterSampler? _vuSamplerC;

    /// <summary>現在アクティブなサンプラーを優先順位 A > C > B で返す。</summary>
    public VuMeterSampler? VuSampler
    {
        get
        {
            if (_vuSamplerA != null && IsCompetitionPlaying) return _vuSamplerA;
            if (_vuSamplerC != null && IsSePlaying)          return _vuSamplerC;
            if (_vuSamplerB != null && IsBgmPlaying)         return _vuSamplerB;
            // 停止中でも最後のサンプラーを返して残像を見せる（ゼロ収束）
            return _vuSamplerA ?? _vuSamplerC ?? _vuSamplerB;
        }
    }

    // チャンネル A (競技曲)
    private WaveOutEvent? _outA;
    private WaveStream? _streamA;
    private System.Threading.Timer? _fadeTimerA;
    private float _baseGainA = 1.0f;

    // チャンネル B (BGM)
    private WaveOutEvent? _outB;
    private WaveStream? _streamB;
    private System.Threading.Timer? _fadeTimerB;
    private float _baseGainB = 1.0f;

    // チャンネル C (SE)
    private WaveOutEvent? _outC;
    private WaveStream? _streamC;
    private System.Threading.Timer? _fadeTimerC;
    private float _baseGainC = 1.0f;

    private bool _disposed;

    public PlaybackEngine()
    {
        LoadPeakCache();
    }

    private void LoadPeakCache()
    {
        if (!File.Exists(PeakCachePath)) return;
        try
        {
            var json = File.ReadAllText(PeakCachePath);
            _peakCache = JsonConvert.DeserializeObject<Dictionary<string, PeakCacheEntry>>(json) 
                ?? new Dictionary<string, PeakCacheEntry>(StringComparer.OrdinalIgnoreCase);
        }
        catch { }
    }

    private void SavePeakCache()
    {
        try
        {
            var json = JsonConvert.SerializeObject(_peakCache, Formatting.Indented);
            File.WriteAllText(PeakCachePath, json);
        }
        catch { }
    }

    private WaveStream CreateWaveStream(string filePath)
    {
        if (filePath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
        {
            return new WaveChannel32(new Mp3FileReader(filePath));
        }
        else
        {
            return new AudioFileReader(filePath);
        }
    }

    private void SetStreamVolume(WaveStream stream, float volume)
    {
        if (stream is AudioFileReader afr)
        {
            afr.Volume = volume;
        }
        else if (stream is WaveChannel32 wc)
        {
            wc.Volume = volume;
        }
        else if (stream is LoopStream ls)
        {
            SetStreamVolume(ls.SourceStream, volume);
        }
    }

    private float GetStreamVolume(WaveStream stream)
    {
        if (stream is AudioFileReader afr)
        {
            return afr.Volume;
        }
        else if (stream is WaveChannel32 wc)
        {
            return wc.Volume;
        }
        else if (stream is LoopStream ls)
        {
            return GetStreamVolume(ls.SourceStream);
        }
        return 1.0f;
    }

    // ────────── 音量正規化 ──────────

    public float CalcGain(string filePath, double targetDbfs)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return 1.0f;

        var fileInfo = new FileInfo(filePath);
        long size = fileInfo.Length;
        string lastWrite = fileInfo.LastWriteTime.ToString("o");

        lock (_lock)
        {
            if (_peakCache.TryGetValue(filePath, out var entry))
            {
                if (entry.FileSize == size && entry.LastWriteTime == lastWrite)
                {
                    return entry.Gain;
                }
            }
        }

        // ピーク検出（全走査）
        float peak = 0f;
        double introSkipSeconds = 0.0; // 副産物としてスキップ位置も計算
        try
        {
            using (var reader = new AudioFileReader(filePath))
            {
                float[] buffer = new float[16384];
                int read;
                long totalSamples = 0;
                long skipSamplePos = -1;
                int channels = reader.WaveFormat.Channels;
                int sampleRate = reader.WaveFormat.SampleRate;

                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < read; i++)
                    {
                        float abs = Math.Abs(buffer[i]);
                        if (abs > peak) peak = abs;

                        // イントロスキップ: -40 dBFS 相当（0.01f）を超えた最初のサンプル位置を記録
                        // 実際の閾値は CalcGain 呼び出し時には設定値を持っていないので固定で記録しておき
                        // CalcIntroSkipPosition で改めて算出する
                        if (skipSamplePos < 0 && abs > 0.01f)
                        {
                            skipSamplePos = totalSamples + i;
                        }
                    }
                    totalSamples += read;
                }

                if (skipSamplePos >= 0 && channels > 0 && sampleRate > 0)
                {
                    introSkipSeconds = (double)(skipSamplePos / channels) / sampleRate;
                }
            }
        }
        catch
        {
            peak = 0.5f;
        }

        if (peak <= 0) peak = 0.001f;

        double targetRatio = Math.Pow(10, targetDbfs / 20.0);
        float gain = (float)(targetRatio / peak);
        if (gain > 10.0f) gain = 10.0f;

        lock (_lock)
        {
            _peakCache[filePath] = new PeakCacheEntry
            {
                FileSize = size,
                LastWriteTime = lastWrite,
                Peak = peak,
                Gain = gain
            };
            SavePeakCache();
        }

        return gain;
    }

    // ────────── イントロスキップ位置計算 ──────────

    /// <summary>
    /// ファイルを走査し、指定窓幅（windowSeconds）のRMSが
    /// 指定閾値（dBFS）を超える最初の窓の開始位置を返す。
    /// 単発音・瞬間ノイズではなく「継続的な音が出始める場所」を検出する。
    /// プリロール秒数を引いた位置（0以上）を秒単位で返す。
    /// </summary>
    public double CalcIntroSkipPosition(string filePath, double thresholdDbfs, double preRollSeconds,
        double windowSeconds = 0.3)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return 0.0;

        // RMS閾値: dBFS → 線形（補正なし。設定値をそのまま使う）
        float rmsThreshold = (float)Math.Pow(10.0, thresholdDbfs / 20.0);

        try
        {
            using var reader = new AudioFileReader(filePath);
            int channels = Math.Max(1, reader.WaveFormat.Channels);
            int sampleRate = reader.WaveFormat.SampleRate;
            int windowFrames = Math.Max(1, (int)(windowSeconds * sampleRate));
            int stepFrames = Math.Max(1, sampleRate / 20); // 50ms ステップ
            float[] buf = new float[windowFrames * channels];

            long framePos = 0;
            long totalFrames = reader.Length / (reader.WaveFormat.BitsPerSample / 8 * channels);

            while (framePos + windowFrames <= totalFrames)
            {
                // 窓の開始位置へシーク（byte 位置 = frame * channels * sizeof(float) = frame * channels * 4）
                reader.Position = framePos * channels * 4L;

                int filled = 0;
                int needed = buf.Length;
                while (filled < needed)
                {
                    int rd = reader.Read(buf, filled, needed - filled);
                    if (rd == 0) break;
                    filled += rd;
                }
                if (filled < needed) break; // EOF

                // 窓内 RMS 計算（モノラル換算）
                double sumSq = 0;
                int frameCount = filled / channels;
                for (int i = 0; i < filled; i += channels)
                {
                    double mono = 0;
                    for (int ch = 0; ch < channels; ch++)
                        mono += buf[i + ch];
                    mono /= channels;
                    sumSq += mono * mono;
                }
                float rms = frameCount > 0 ? (float)Math.Sqrt(sumSq / frameCount) : 0f;

                if (rms >= rmsThreshold)
                {
                    double hitSeconds = (double)framePos / sampleRate;
                    double startSeconds = Math.Max(0.0, hitSeconds - preRollSeconds);
                    return startSeconds;
                }

                framePos += stepFrames;
            }
        }
        catch { }

        return 0.0;
    }

    // ────────── チャンネル A (競技曲) ──────────

    /// <param name="skipToSeconds">再生開始位置（秒）。0 なら先頭から。</param>
    public void PlayCompetition(string filePath, float gain = 1.0f, double skipToSeconds = 0.0)
    {
        lock (_lock)
        {
            CleanupChannel(ref _outA, ref _streamA, ref _fadeTimerA);

            _baseGainA = gain;
            _streamA = CreateWaveStream(filePath);
            SetStreamVolume(_streamA, gain);

            // イントロスキップ: 指定秒数へシーク
            if (skipToSeconds > 0.0)
            {
                try
                {
                    _streamA.CurrentTime = TimeSpan.FromSeconds(skipToSeconds);
                }
                catch { /* シーク失敗時は先頭から */ }
            }

            // VU メーターサンプラーをストリームに挟む
            _vuSamplerA = new VuMeterSampler(_streamA.ToSampleProvider());
            _outA = new WaveOutEvent();
            _outA.Init(new NAudio.Wave.SampleProviders.SampleToWaveProvider(_vuSamplerA));

            // ファイル末尾で自然終了したときに通知（種目P等の自然終了対応）
            _outA.PlaybackStopped += OnCompetitionPlaybackStopped;

            _outA.Play();
        }
    }

    private void OnCompetitionPlaybackStopped(object? sender, NAudio.Wave.StoppedEventArgs e)
    {
        // 意図的な停止（FadeOut/Immediate）では CleanupChannel で _outA が null になるより先にここが呼ばれる
        // ストリームが末尾まで読み切った場合のみ通知する（Position == Length）
        bool naturalEnd;
        lock (_lock)
        {
            naturalEnd = _streamA != null && _streamA.Position >= _streamA.Length;
        }
        if (naturalEnd)
        {
            CompetitionNaturallyEnded?.Invoke();
        }
    }

    public void PauseCompetition()
    {
        lock (_lock)
        {
            if (_outA is { PlaybackState: PlaybackState.Playing })
            {
                _outA.Pause();
            }
        }
    }

    public void ResumeCompetitionWithFadeIn(double fadeInSeconds)
    {
        lock (_lock)
        {
            if (_outA != null && _streamA != null)
            {
                FadeInChannel(_outA, _streamA, _fadeTimerA, t => _fadeTimerA = t, fadeInSeconds, _baseGainA);
            }
        }
    }

    public void ResumeCompetitionFromStart()
    {
        lock (_lock)
        {
            if (_outA != null && _streamA != null)
            {
                _streamA.Position = 0;
                SetStreamVolume(_streamA, _baseGainA);
                _outA.Play();
            }
        }
    }

    public void StopCompetitionWithFadeOut(double fadeOutSeconds, Action? onCompleted = null)
    {
        lock (_lock)
        {
            FadeOutChannel(_outA, _streamA, _fadeTimerA, t => _fadeTimerA = t, fadeOutSeconds, () =>
            {
                lock (_lock)
                {
                    CleanupChannel(ref _outA, ref _streamA, ref _fadeTimerA);
                }
                onCompleted?.Invoke();
            });
        }
    }

    public void StopCompetitionImmediate()
    {
        lock (_lock)
        {
            CleanupChannel(ref _outA, ref _streamA, ref _fadeTimerA);
        }
    }

    public TimeSpan CompetitionElapsed
    {
        get
        {
            lock (_lock)
            {
                return _streamA?.CurrentTime ?? TimeSpan.Zero;
            }
        }
    }

    public bool IsCompetitionPlaying
    {
        get
        {
            lock (_lock)
            {
                return _outA is { PlaybackState: PlaybackState.Playing };
            }
        }
    }

    // ────────── チャンネル B (BGM) ──────────

    /// <param name="skipToSeconds">再生開始位置（秒）。0 なら先頭から。</param>
    public void PlayBgm(string filePath, bool loop, float gain = 1.0f, double skipToSeconds = 0.0)
    {
        lock (_lock)
        {
            CleanupChannel(ref _outB, ref _streamB, ref _fadeTimerB);

            _baseGainB = gain;
            var rawStream = CreateWaveStream(filePath);

            // イントロスキップ: LoopStream 前にシーク（先頭ループで戻るため rawStream に適用）
            if (skipToSeconds > 0.0)
            {
                try { rawStream.CurrentTime = TimeSpan.FromSeconds(skipToSeconds); }
                catch { }
            }

            if (loop)
            {
                _streamB = new LoopStream(rawStream);
            }
            else
            {
                _streamB = rawStream;
            }

            SetStreamVolume(_streamB, gain);

            // VU メーターサンプラーを挟む
            _vuSamplerB = new VuMeterSampler(_streamB.ToSampleProvider());
            _outB = new WaveOutEvent();
            _outB.Init(new NAudio.Wave.SampleProviders.SampleToWaveProvider(_vuSamplerB));
            _outB.Play();
        }
    }

    public void StopBgmWithFadeOut(double fadeOutSeconds, Action? onCompleted = null)
    {
        lock (_lock)
        {
            FadeOutChannel(_outB, _streamB, _fadeTimerB, t => _fadeTimerB = t, fadeOutSeconds, () =>
            {
                lock (_lock)
                {
                    CleanupChannel(ref _outB, ref _streamB, ref _fadeTimerB);
                }
                onCompleted?.Invoke();
            });
        }
    }

    public bool IsBgmPlaying
    {
        get
        {
            lock (_lock)
            {
                return _outB is { PlaybackState: PlaybackState.Playing };
            }
        }
    }

    // ────────── チャンネル C (SE) ──────────

    public void PlaySe(string filePath, float gain = 1.0f)
    {
        lock (_lock)
        {
            CleanupChannel(ref _outC, ref _streamC, ref _fadeTimerC);

            _baseGainC = gain;
            _streamC = CreateWaveStream(filePath);
            SetStreamVolume(_streamC, gain);

            // VU メーターサンプラーを挟む
            _vuSamplerC = new VuMeterSampler(_streamC.ToSampleProvider());
            _outC = new WaveOutEvent();
            _outC.Init(new NAudio.Wave.SampleProviders.SampleToWaveProvider(_vuSamplerC));
            _outC.Play();
        }
    }

    public void StopSeWithFadeOut(double fadeOutSeconds, Action? onCompleted = null)
    {
        lock (_lock)
        {
            FadeOutChannel(_outC, _streamC, _fadeTimerC, t => _fadeTimerC = t, fadeOutSeconds, () =>
            {
                lock (_lock)
                {
                    CleanupChannel(ref _outC, ref _streamC, ref _fadeTimerC);
                }
                onCompleted?.Invoke();
            });
        }
    }

    public bool IsSePlaying
    {
        get
        {
            lock (_lock)
            {
                return _outC is { PlaybackState: PlaybackState.Playing };
            }
        }
    }

    // ────────── 共通フェード実装 ──────────

    private void FadeOutChannel(WaveOutEvent? waveOut, WaveStream? stream, System.Threading.Timer? currentTimer, Action<System.Threading.Timer?> timerSetter, double seconds, Action? onCompleted)
    {
        if (currentTimer != null)
        {
            currentTimer.Dispose();
            timerSetter(null);
        }

        if (waveOut == null || stream == null || waveOut.PlaybackState != PlaybackState.Playing || seconds <= 0)
        {
            if (waveOut != null)
            {
                try { waveOut.Stop(); } catch { }
            }
            onCompleted?.Invoke();
            return;
        }

        float initialVol = GetStreamVolume(stream);
        int intervalMs = 50;
        int totalSteps = (int)((seconds * 1000) / intervalMs);
        if (totalSteps <= 0) totalSteps = 1;
        int currentStep = 0;

        System.Threading.Timer? timerInstance = null;
        timerInstance = new System.Threading.Timer(_ =>
        {
            lock (_lock)
            {
                currentStep++;
                if (currentStep >= totalSteps || stream == null || waveOut == null || waveOut.PlaybackState != PlaybackState.Playing)
                {
                    if (stream != null) SetStreamVolume(stream, 0f);
                    if (waveOut != null) { try { waveOut.Stop(); } catch { } }
                    timerInstance?.Dispose();
                    timerSetter(null);
                    onCompleted?.Invoke();
                    return;
                }

                float ratio = 1.0f - ((float)currentStep / totalSteps);
                float newVol = initialVol * ratio;
                SetStreamVolume(stream, newVol);
            }
        }, null, intervalMs, intervalMs);

        timerSetter(timerInstance);
    }

    private void FadeInChannel(WaveOutEvent? waveOut, WaveStream? stream, System.Threading.Timer? currentTimer, Action<System.Threading.Timer?> timerSetter, double seconds, float targetVolume)
    {
        if (currentTimer != null)
        {
            currentTimer.Dispose();
            timerSetter(null);
        }

        if (waveOut == null || stream == null || seconds <= 0)
        {
            if (stream != null) SetStreamVolume(stream, targetVolume);
            waveOut?.Play();
            return;
        }

        SetStreamVolume(stream, 0f);
        waveOut.Play();

        int intervalMs = 50;
        int totalSteps = (int)((seconds * 1000) / intervalMs);
        if (totalSteps <= 0) totalSteps = 1;
        int currentStep = 0;

        System.Threading.Timer? timerInstance = null;
        timerInstance = new System.Threading.Timer(_ =>
        {
            lock (_lock)
            {
                currentStep++;
                if (currentStep >= totalSteps || stream == null || waveOut == null || waveOut.PlaybackState != PlaybackState.Playing)
                {
                    if (stream != null) SetStreamVolume(stream, targetVolume);
                    timerInstance?.Dispose();
                    timerSetter(null);
                    return;
                }

                float ratio = (float)currentStep / totalSteps;
                float newVol = targetVolume * ratio;
                SetStreamVolume(stream, newVol);
            }
        }, null, intervalMs, intervalMs);

        timerSetter(timerInstance);
    }

    private void CleanupChannel(ref WaveOutEvent? waveOut, ref WaveStream? stream, ref System.Threading.Timer? fadeTimer)
    {
        if (fadeTimer != null)
        {
            fadeTimer.Dispose();
            fadeTimer = null;
        }
        if (waveOut != null)
        {
            try { waveOut.Stop(); } catch { }
            waveOut.Dispose();
            waveOut = null;
        }
        if (stream != null)
        {
            stream.Dispose();
            stream = null;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        lock (_lock)
        {
            CleanupChannel(ref _outA, ref _streamA, ref _fadeTimerA);
            CleanupChannel(ref _outB, ref _streamB, ref _fadeTimerB);
            CleanupChannel(ref _outC, ref _streamC, ref _fadeTimerC);
        }

        GC.SuppressFinalize(this);
    }
}
