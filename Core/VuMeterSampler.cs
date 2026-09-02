using System;
using NAudio.Wave;

namespace DSMusic.Core;

/// <summary>
/// 再生ストリームからサンプルを横取りして L/R ピーク値と
/// 簡易スペクトラムバー値を公開するサンプルプロバイダー。
/// </summary>
public class VuMeterSampler : ISampleProvider
{
    private readonly ISampleProvider _source;

    // バーの本数（スペクトラム表示用）
    public const int BarCount = 20;

    // 最新の L/R ピーク (0.0f–1.0f)
    public float PeakL { get; private set; }
    public float PeakR { get; private set; }

    // 最新のスペクトラムバー値 (0.0f–1.0f) × BarCount
    private readonly float[] _bars = new float[BarCount];
    public float[] Bars => _bars;

    // ピーク保持（ドロップダウン表示用）
    private float _holdL;
    private float _holdR;
    private int _holdCounterL;
    private int _holdCounterR;
    private const int HoldFrames = 15;  // 約1.5秒保持（100ms×15）

    public float HoldL => _holdL;
    public float HoldR => _holdR;

    // 直近バッファを蓄積してバー計算するワークバッファ
    private readonly float[] _fftWork = new float[1024];
    private int _fftWritePos;

    public WaveFormat WaveFormat => _source.WaveFormat;

    public VuMeterSampler(ISampleProvider source)
    {
        _source = source;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        int read = _source.Read(buffer, offset, count);

        if (read == 0) return read;

        int channels = _source.WaveFormat.Channels;
        float peakL = 0f, peakR = 0f;

        for (int i = 0; i < read; i += channels)
        {
            float sL = Math.Abs(buffer[offset + i]);
            float sR = channels > 1 ? Math.Abs(buffer[offset + i + 1]) : sL;
            if (sL > peakL) peakL = sL;
            if (sR > peakR) peakR = sR;

            // FFT ワークバッファに合計サンプルを詰める
            float mono = (sL + sR) * 0.5f;
            _fftWork[_fftWritePos % _fftWork.Length] = mono;
            _fftWritePos++;
        }

        // ピーク更新（指数移動平均でスムーズに落とす）
        const float attack = 0.8f;
        const float release = 0.92f;
        PeakL = peakL > PeakL ? peakL * attack + PeakL * (1 - attack) : PeakL * release;
        PeakR = peakR > PeakR ? peakR * attack + PeakR * (1 - attack) : PeakR * release;

        // ピーク保持
        if (peakL >= _holdL) { _holdL = peakL; _holdCounterL = HoldFrames; }
        else if (_holdCounterL > 0) _holdCounterL--;
        else _holdL *= 0.88f;

        if (peakR >= _holdR) { _holdR = peakR; _holdCounterR = HoldFrames; }
        else if (_holdCounterR > 0) _holdCounterR--;
        else _holdR *= 0.88f;

        // 簡易スペクトラム: バッファを BarCount 等分してRMSを計算
        int blockSize = Math.Max(1, read / channels / BarCount);
        for (int b = 0; b < BarCount; b++)
        {
            float sum = 0f;
            int start = b * blockSize;
            int end = Math.Min(start + blockSize, read / channels);
            int cnt = 0;
            for (int s = start; s < end; s++)
            {
                int idx = s * channels;
                if (idx < read)
                {
                    float v = Math.Abs(buffer[offset + idx]);
                    sum += v * v;
                    cnt++;
                }
            }
            float rms = cnt > 0 ? MathF.Sqrt(sum / cnt) : 0f;
            // スムーズ追従
            float prev = _bars[b];
            _bars[b] = rms > prev ? rms * 0.7f + prev * 0.3f : prev * 0.80f;
        }

        return read;
    }

    public void Reset()
    {
        PeakL = PeakR = _holdL = _holdR = 0f;
        _holdCounterL = _holdCounterR = 0;
        Array.Clear(_bars, 0, _bars.Length);
    }
}
