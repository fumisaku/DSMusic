using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace DSMusic.Core;

public enum PlayState { Idle, Playing, Paused, FadingOut, FadingIn, Completed, Error }

public class PlayController : IDisposable
{
    public event Action<PlayState>? PlayStateChanged;

    private readonly PlaybackEngine _playbackEngine;
    private readonly SettingsService _settingsService;
    private readonly WebSocketService _webSocketService;
    private readonly AssignService _assignService;

    private PlayState _state = PlayState.Idle;
    public PlayState State
    {
        get => _state;
        private set
        {
            if (_state == value) return;
            _state = value;
            PlayStateChanged?.Invoke(_state);
        }
    }

    private HeatAssignment? _currentHeat;
    public HeatAssignment? CurrentHeat => _currentHeat;

    private System.Threading.Timer? _playTimer;
    private double _targetPlaySeconds = 90.0;
    private bool _disposed;

    public PlayController(
        PlaybackEngine playbackEngine,
        SettingsService settingsService,
        WebSocketService webSocketService,
        AssignService assignService)
    {
        _playbackEngine = playbackEngine;
        _settingsService = settingsService;
        _webSocketService = webSocketService;
        _assignService = assignService;

        // 種目P等が曲末尾まで自然再生されたときに終了処理を行う
        _playbackEngine.CompetitionNaturallyEnded += OnCompetitionNaturallyEnded;
    }

    private void OnCompetitionNaturallyEnded()
    {
        // FadingOut/Idle など終了処理中・終了済みの場合は無視
        if (State == PlayState.FadingOut || State == PlayState.Idle || State == PlayState.Completed)
            return;

        EndWithFadeOut();
    }

    public void StartPlay(HeatAssignment heat)
    {
        _currentHeat = heat;

        if (string.IsNullOrEmpty(heat.FilePath) || !File.Exists(heat.FilePath))
        {
            State = PlayState.Error;
            SendStatusUpdate("File not found: " + heat.FilePath);
            return;
        }

        if (_playbackEngine.IsBgmPlaying)
        {
            _playbackEngine.StopBgmWithFadeOut(_settingsService.Current.FadeOutSeconds, () =>
            {
                ExecutePlay(heat);
            });
        }
        else
        {
            ExecutePlay(heat);
        }
    }

    private void ExecutePlay(HeatAssignment heat)
    {
        try
        {
            CancelTimer();

            float gain = _playbackEngine.CalcGain(heat.FilePath, _settingsService.Current.TargetVolume_dBFS);

            // イントロスキップ位置を計算（設定が有効 かつ 種目 P 以外の場合のみ）
            double skipTo = 0.0;
            var cfg = _settingsService.Current;
            if (cfg.IntroSkipEnabled && heat.DncCd != "P")
            {
                skipTo = _playbackEngine.CalcIntroSkipPosition(
                    heat.FilePath, cfg.IntroSkipThresholdDbfs, cfg.IntroSkipPreRollSeconds);
            }

            _playbackEngine.PlayCompetition(heat.FilePath, gain, skipTo);

            heat.PlayStatus = "Playing";
            heat.PlayStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _assignService.Save();

            State = PlayState.Playing;
            SendStatusUpdate();

            // P (Paso) 以外はタイマー設定
            if (heat.DncCd != "P")
            {
                var playTimeSetting = _assignService.FindPlayTimeSetting(_settingsService.Current, heat.DncCd, heat.KbnNo, heat.RndNo);
                _targetPlaySeconds = playTimeSetting?.PlaySeconds ?? 90.0;

                double fadeOutSec = _settingsService.Current.FadeOutSeconds;
                double timerSec = _targetPlaySeconds - fadeOutSec;
                if (timerSec < 0) timerSec = 0;

                _playTimer = new System.Threading.Timer(OnPlayTimerExpired, null, TimeSpan.FromSeconds(timerSec), Timeout.InfiniteTimeSpan);
            }
            else
            {
                _targetPlaySeconds = 0.0; // P has no fixed target seconds unless finished
            }
        }
        catch (Exception ex)
        {
            State = PlayState.Error;
            SendStatusUpdate("Play execution failed: " + ex.Message);
        }
    }

    private void OnPlayTimerExpired(object? state)
    {
        EndWithFadeOut();
    }

    public void EmergencyStop()
    {
        if (State == PlayState.Playing || State == PlayState.FadingIn || State == PlayState.FadingOut)
        {
            CancelTimer();
            _playbackEngine.PauseCompetition();
            State = PlayState.Paused;
            SendStatusUpdate();
        }
    }

    public void ResumeFromPause(bool fromStart)
    {
        if (State != PlayState.Paused || _currentHeat == null) return;

        CancelTimer();

        if (fromStart)
        {
            _playbackEngine.ResumeCompetitionFromStart();
            State = PlayState.Playing;
            SendStatusUpdate();

            if (_currentHeat.DncCd != "P")
            {
                double fadeOutSec = _settingsService.Current.FadeOutSeconds;
                double timerSec = _targetPlaySeconds - fadeOutSec;
                if (timerSec < 0) timerSec = 0;
                _playTimer = new System.Threading.Timer(OnPlayTimerExpired, null, TimeSpan.FromSeconds(timerSec), Timeout.InfiniteTimeSpan);
            }
        }
        else
        {
            State = PlayState.FadingIn;
            SendStatusUpdate();

            double fadeInSec = _settingsService.Current.FadeInSeconds;
            _playbackEngine.ResumeCompetitionWithFadeIn(fadeInSec);

            // フェードイン完了タイマー
            _playTimer = new System.Threading.Timer(_ =>
            {
                State = PlayState.Playing;
                SendStatusUpdate();

                if (_currentHeat.DncCd != "P")
                {
                    double elapsed = _playbackEngine.CompetitionElapsed.TotalSeconds;
                    double fadeOutSec = _settingsService.Current.FadeOutSeconds;
                    double remaining = _targetPlaySeconds - elapsed - fadeOutSec;

                    if (remaining <= 0)
                    {
                        EndWithFadeOut();
                    }
                    else
                    {
                        _playTimer = new System.Threading.Timer(OnPlayTimerExpired, null, TimeSpan.FromSeconds(remaining), Timeout.InfiniteTimeSpan);
                    }
                }
            }, null, TimeSpan.FromSeconds(fadeInSec), Timeout.InfiniteTimeSpan);
        }
    }

    public void EndWithFadeOut()
    {
        if (State == PlayState.Idle || _currentHeat == null) return;

        CancelTimer();
        State = PlayState.FadingOut;
        SendStatusUpdate();

        // フェードアウト完了後に MU_NOTIFY_END を送信するためにヒート情報を保持
        var heatForNotify = _currentHeat;

        double fadeOutSec = _settingsService.Current.FadeOutSeconds;
        _playbackEngine.StopCompetitionWithFadeOut(fadeOutSec, () =>
        {
            if (_currentHeat != null)
            {
                _currentHeat.PlayStatus = "Completed";
                _currentHeat.PlayEndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _currentHeat.ActualPlaySeconds = _playbackEngine.CompetitionElapsed.TotalSeconds;
                _assignService.Save();
            }

            State = PlayState.Completed;
            SendStatusUpdate();

            // MU_NOTIFY_END をサーバーに送信（Key1情報付き）
            _ = _webSocketService.SendMuNotifyEndAsync(heatForNotify);

            State = PlayState.Idle;
            SendStatusUpdate();
        });
    }

    public void StopImmediate()
    {
        CancelTimer();
        _playbackEngine.StopCompetitionImmediate();
        State = PlayState.Idle;
        SendStatusUpdate();
    }

    public void SendStatusUpdate(string? errorMessage = null)
    {
        if (string.IsNullOrEmpty(_webSocketService.OrgCd) || string.IsNullOrEmpty(_webSocketService.CmpNo)) return;

        var payload = new MuStatusPayload
        {
            OrgCd = _webSocketService.OrgCd,
            CmpNo = _webSocketService.CmpNo,
            PlayState = State.ToString(),
            Channel = "Competition",
            ErrorMessage = errorMessage
        };

        if (_currentHeat != null)
        {
            // Key1（フロア・進行番号）
            payload.FloorCd  = _currentHeat.FloorCd;
            payload.PrgNo    = _currentHeat.PrgNo;
            payload.PrgSubNo = _currentHeat.PrgSubNo;
            // Key2（区分・ラウンド等）
            payload.KbnNo    = _currentHeat.KbnNo;
            payload.RndNo    = _currentHeat.RndNo;
            payload.DGrpNo   = _currentHeat.DGrpNo;
            payload.DncNo    = _currentHeat.DncNo;
            payload.DncCd    = _currentHeat.DncCd;
            payload.HeatNo   = _currentHeat.HeatNo;
            payload.ElapsedSeconds = _playbackEngine.CompetitionElapsed.TotalSeconds;
            payload.TotalSeconds   = _targetPlaySeconds;
        }

        _ = _webSocketService.SendMuStatusAsync(payload);
    }

    private void CancelTimer()
    {
        _playTimer?.Dispose();
        _playTimer = null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        CancelTimer();
        _playbackEngine.CompetitionNaturallyEnded -= OnCompetitionNaturallyEnded;
        GC.SuppressFinalize(this);
    }
}
