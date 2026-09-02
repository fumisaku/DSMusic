using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DSMusic.Core;
using Newtonsoft.Json;

namespace DSMusic.Forms;

public partial class F010_Main : Form
{
    private readonly SettingsService _settingsService;
    private readonly WebSocketService _webSocketService;
    private DsStatus _dsStatus;
    private readonly DaMaster _daMaster;

    private readonly AssignService _assignService;
    private readonly PlaybackEngine _playbackEngine;
    private readonly PlayController _playController;

    private HeatAssignment? _selectedHeat;
    private readonly Dictionary<string, Button> _bgmButtons = new();
    // 0=停止, 1=再生中, 2=フェードアウト中
    private readonly Dictionary<string, int> _bgmPlayingState = new();

    // オナーダンス：種目コード→再生中のファイルパスと演奏秒数
    private string? _honorCurrentKey;
    private double _honorTargetSeconds;
    private DateTime _honorPlayStart;

    private bool _isUpdatingGrid;

    public F010_Main(
        SettingsService settingsService,
        WebSocketService webSocketService,
        DsStatus dsStatus,
        DaMaster daMaster)
    {
        InitializeComponent();

        _settingsService = settingsService;
        _webSocketService = webSocketService;
        _dsStatus = dsStatus;
        _daMaster = daMaster;

        _assignService = new AssignService();
        _assignService.Load(_dsStatus.OrgCd, _dsStatus.CompNo);
        _assignService.SyncWithDsStatus(_dsStatus, _daMaster, _settingsService.Current);

        _playbackEngine = new PlaybackEngine();
        _playController = new PlayController(_playbackEngine, _settingsService, _webSocketService, _assignService);

        // イベント登録
        _playController.PlayStateChanged += OnPlayStateChanged;
        _webSocketService.StateChanged += OnWsStateChanged;
        _webSocketService.LoginReceived += OnLoginReceived;
        _webSocketService.DsStatusReceived += OnDsStatusReceived;
        _webSocketService.NotifyPlayReceived += OnNotifyPlayReceived;
        _webSocketService.NotifyStopReceived += OnNotifyStopReceived;
        _webSocketService.NotifyResumeReceived += OnNotifyResumeReceived;
        _webSocketService.NotifyEndReceived += OnNotifyEndReceived;
        _webSocketService.NotifyBgmPlayReceived += OnNotifyBgmPlayReceived;
        _webSocketService.NotifyBgmStopReceived += OnNotifyBgmStopReceived;
        _webSocketService.HeatNotifyReceived += OnHeatNotifyReceived;
        _webSocketService.NotifyPlayTimeReceived += OnNotifyPlayTimeReceived;
    }

    private void F010_Main_Load(object sender, EventArgs e)
    {
        lblCmpName.Text = _daMaster.CompName;
        UpdateWsStateLabel(_webSocketService.State);

        // バージョン情報を表示
        // Version形式: 1.yy.MMdd.HHmm  例: 1.25.720.1432 → "v1.25 build 2025-07-20 14:32"
        var ver = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version;
        if (ver != null)
        {
            int year  = 2000 + ver.Minor;
            int month = ver.Build / 100;
            int day   = ver.Build % 100;
            int hour  = ver.Revision / 100;
            int min   = ver.Revision % 100;
            lblVersion.Text = $"v{ver.Major}.{ver.Minor} build {year}-{month:D2}-{day:D2} {hour:D2}:{min:D2}";
        }

        InitializeBgmSeButtons();
        InitializeGridColumns();
        RefreshGrid();

        timerPlayback.Start();
        UpdatePlayControlUi();

        // VU メーター初期状態（サンプラー未接続）
        vuMeter.SetSampler(null);
    }

    private void F010_Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        _settingsService.Save();
        _assignService.Save();
    }

    private void F010_Main_FormClosed(object sender, FormClosedEventArgs e)
    {
        // イベント解除
        _playController.PlayStateChanged -= OnPlayStateChanged;
        _webSocketService.StateChanged -= OnWsStateChanged;
        _webSocketService.LoginReceived -= OnLoginReceived;
        _webSocketService.DsStatusReceived -= OnDsStatusReceived;
        _webSocketService.NotifyPlayReceived -= OnNotifyPlayReceived;
        _webSocketService.NotifyStopReceived -= OnNotifyStopReceived;
        _webSocketService.NotifyResumeReceived -= OnNotifyResumeReceived;
        _webSocketService.NotifyEndReceived -= OnNotifyEndReceived;
        _webSocketService.NotifyBgmPlayReceived -= OnNotifyBgmPlayReceived;
        _webSocketService.NotifyBgmStopReceived -= OnNotifyBgmStopReceived;
        _webSocketService.HeatNotifyReceived -= OnHeatNotifyReceived;
        _webSocketService.NotifyPlayTimeReceived -= OnNotifyPlayTimeReceived;

        timerPlayback.Stop();
        _playController.Dispose();
        _playbackEngine.Dispose();
        _ = _webSocketService.DisconnectAsync();
        _webSocketService.Dispose();
    }

    // ────────── UI生成・初期化 ──────────

    private void InitializeBgmSeButtons()
    {
        flowBgm.Controls.Clear();
        flowSe.Controls.Clear();
        flowHonor.Controls.Clear();

        foreach (var key in BgmKeys.All)
        {
            var btn = new Button
            {
                Text = key,
                Width = 135,
                Height = 35,
                Tag = key,
                UseVisualStyleBackColor = true
            };
            btn.Click += BgmSeButton_Click;

            _bgmButtons[key] = btn;
            _bgmPlayingState[key] = 0;

            if (BgmKeys.IsSeChannel(key))
            {
                flowSe.Controls.Add(btn);
            }
            else if (key.StartsWith("オナーダンス_"))
            {
                // ⑤ 「オナー」を削除して種目記号のみ表示（例: "W"）
                btn.Text = key.Replace("オナーダンス_", "");
                flowHonor.Controls.Add(btn);
            }
            else
            {
                flowBgm.Controls.Add(btn);
            }
        }
    }

    private void InitializeGridColumns()
    {
        dgvHeats.Columns.Clear();
        dgvHeats.AutoGenerateColumns = false;

        dgvHeats.Columns.Add(new DataGridViewTextBoxColumn { Name = "Progress", HeaderText = "進行", Width = 60, ReadOnly = true });
        dgvHeats.Columns.Add(new DataGridViewTextBoxColumn { Name = "Kubun", HeaderText = "区分", Width = 100, ReadOnly = true });
        dgvHeats.Columns.Add(new DataGridViewTextBoxColumn { Name = "Round", HeaderText = "ラウンド", Width = 80, ReadOnly = true });
        dgvHeats.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dance", HeaderText = "種目", Width = 80, ReadOnly = true });
        dgvHeats.Columns.Add(new DataGridViewTextBoxColumn { Name = "Heat", HeaderText = "ヒート", Width = 50, ReadOnly = true });
        dgvHeats.Columns.Add(new DataGridViewTextBoxColumn { Name = "PlayTime", HeaderText = "演奏時間", Width = 80, ReadOnly = true });
        dgvHeats.Columns.Add(new DataGridViewTextBoxColumn { Name = "FilePath", HeaderText = "アサイン曲", Width = 180, ReadOnly = true });
        dgvHeats.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "状態", Width = 80, ReadOnly = true });

        var btnCol = new DataGridViewButtonColumn
        {
            Name = "Change",
            HeaderText = "変更",
            Text = "変更",
            UseColumnTextForButtonValue = true,
            Width = 60
        };
        dgvHeats.Columns.Add(btnCol);
    }

    private void RefreshGrid()
    {
        // 再構築前に選択中のヒートを記憶する
        var previouslySelected = _selectedHeat;

        _isUpdatingGrid = true;
        dgvHeats.Rows.Clear();

        int index = 1;
        foreach (var h in _assignService.CurrentData.HeatAssignments)
        {
            string kubunName = _daMaster.Kubuns.FirstOrDefault(k => k.KbnNo == h.KbnNo)?.KbnDispName ?? h.KbnNo;
            string roundName = _daMaster.Kubuns.FirstOrDefault(k => k.KbnNo == h.KbnNo)?.Rounds.FirstOrDefault(r => r.RndNo == h.RndNo)?.RndNameJ ?? h.RndNo;
            string danceName = _daMaster.Kubuns.FirstOrDefault(k => k.KbnNo == h.KbnNo)?.Rounds.FirstOrDefault(r => r.RndNo == h.RndNo)?.DGroups.FirstOrDefault(dg => dg.DGrpNo == h.DGrpNo)?.Dances.FirstOrDefault(d => d.DncNo == h.DncNo)?.DncNameJ ?? h.DncCd;

            // 演奏時間の表示テキストを組み立てる
            string playTimeText = GetPlayTimeText(h);

            int rowIndex = dgvHeats.Rows.Add(
                index++,
                kubunName,
                roundName,
                danceName,
                h.HeatNo,
                playTimeText,
                Path.GetFileName(h.FilePath),
                StatusDisplayName(h.PlayStatus)   // ② 日本語表示
            );

            dgvHeats.Rows[rowIndex].Tag = h;

            // ② 色分け
            ApplyRowColor(dgvHeats.Rows[rowIndex], h.PlayStatus);
        }

        _isUpdatingGrid = false;

        // 再構築後、以前選択していたヒートと同じ行を再選択する
        // （KbnNo + RndNo + DGrpNo + DncNo + HeatNo で同一性を判定）
        if (previouslySelected != null)
        {
            foreach (DataGridViewRow row in dgvHeats.Rows)
            {
                if (row.Tag is HeatAssignment h &&
                    h.KbnNo == previouslySelected.KbnNo &&
                    h.RndNo == previouslySelected.RndNo &&
                    h.DGrpNo == previouslySelected.DGrpNo &&
                    h.DncNo == previouslySelected.DncNo &&
                    h.HeatNo == previouslySelected.HeatNo)
                {
                    dgvHeats.ClearSelection();
                    row.Selected = true;
                    dgvHeats.FirstDisplayedScrollingRowIndex = row.Index;
                    _selectedHeat = row.Tag as HeatAssignment;
                    UpdateSelectedHeatUi();
                    return;
                }
            }
        }

        // 以前の選択が復元できない場合は通常の更新
        UpdateSelectedHeatFromGrid();
    }

    /// <summary>状態文字列→日本語表示名</summary>
    private static string StatusDisplayName(string status) => status switch
    {
        "Completed" => "完了",
        "Playing"   => "演奏中",
        _           => "未開始",
    };

    /// <summary>行の PlayStatus に応じて背景/前景色を設定する</summary>
    private static void ApplyRowColor(DataGridViewRow row, string status)
    {
        switch (status)
        {
            case "Completed":
                row.DefaultCellStyle.ForeColor = Color.Gray;
                row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                break;
            case "Playing":
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.BackColor = Color.LightGreen;
                break;
            default:
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.BackColor = Color.White;
                break;
        }
    }

    /// <summary>
    /// ヒートに適用される演奏時間設定テキストを返す。
    /// P は "2ndHL" / "Full" などのパターン名を返す。
    /// 設定が見つからない場合は "未設定" を返す。
    /// </summary>
    private string GetPlayTimeText(HeatAssignment h)
    {
        var setting = _assignService.FindPlayTimeSetting(
            _settingsService.Current, h.DncCd, h.KbnNo, h.RndNo);

        if (setting == null) return "未設定";

        if (h.DncCd == "P")
        {
            return setting.PasoPattern switch
            {
                "FullChorus" => "P:Full",
                _ => "P:2ndHL"
            };
        }

        if (setting.PlaySeconds.HasValue)
        {
            int sec = (int)setting.PlaySeconds.Value;
            return $"{sec / 60}:{sec % 60:D2}";
        }

        return "未設定";
    }

    // ────────── イベントハンドラ（コントロール） ──────────

    private void dgvHeats_SelectionChanged(object sender, EventArgs e)
    {
        if (_isUpdatingGrid) return;
        UpdateSelectedHeatFromGrid();
    }

    private void UpdateSelectedHeatFromGrid()
    {
        if (dgvHeats.SelectedRows.Count > 0)
        {
            var row = dgvHeats.SelectedRows[0];
            _selectedHeat = row.Tag as HeatAssignment;
        }
        else
        {
            _selectedHeat = null;
        }

        UpdateSelectedHeatUi();
    }

    private void UpdateSelectedHeatUi()
    {
        if (_selectedHeat != null)
        {
            string kubunName = _daMaster.Kubuns.FirstOrDefault(k => k.KbnNo == _selectedHeat.KbnNo)?.KbnDispName ?? _selectedHeat.KbnNo;
            string roundName = _daMaster.Kubuns.FirstOrDefault(k => k.KbnNo == _selectedHeat.KbnNo)?.Rounds.FirstOrDefault(r => r.RndNo == _selectedHeat.RndNo)?.RndNameJ ?? _selectedHeat.RndNo;
            string danceName = _daMaster.Kubuns.FirstOrDefault(k => k.KbnNo == _selectedHeat.KbnNo)?.Rounds.FirstOrDefault(r => r.RndNo == _selectedHeat.RndNo)?.DGroups.FirstOrDefault(dg => dg.DGrpNo == _selectedHeat.DGrpNo)?.Dances.FirstOrDefault(d => d.DncNo == _selectedHeat.DncNo)?.DncNameJ ?? _selectedHeat.DncCd;

            lblHeatDetails.Text = $"{kubunName} - {roundName} / {danceName} (第 {_selectedHeat.HeatNo} ヒート)";
            lblSongDetails.Text = $"曲名: {Path.GetFileName(_selectedHeat.FilePath)} ({_selectedHeat.FilePath})";
        }
        else
        {
            lblHeatDetails.Text = "ヒートが選択されていません。";
            lblSongDetails.Text = "曲名: -";
        }

        UpdatePlayControlUi();
    }

    private void dgvHeats_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= dgvHeats.Rows.Count) return;

        // "Change" 列 (Index = 7)
        if (dgvHeats.Columns[e.ColumnIndex].Name == "Change")
        {
            var row = dgvHeats.Rows[e.RowIndex];
            if (row.Tag is HeatAssignment h)
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "音楽ファイル|*.mp3;*.wav|すべてのファイル|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        h.FilePath = ofd.FileName;
                        h.ManuallyAssigned = true;
                        _assignService.Save();
                        RefreshGrid();
                    }
                }
            }
        }
    }

    // ────────── キーボード操作 ──────────

    /// <summary>
    /// Space / Enter : 演奏開始 または 曲終了（トグル動作）
    ///   Idle / Completed / Error → 演奏開始（btnPlayStart と同じ）
    ///   Playing / FadingIn     → 曲終了フェードアウト（btnEndWithFadeOut と同じ）
    ///   Paused                 → 最初から再開（btnPlayStart と同じ）
    /// ESC : 緊急停止 / Paused 時は途中から再開（btnEmergencyStop と同じ）
    /// </summary>
    private void F010_Main_KeyDown(object sender, KeyEventArgs e)
    {
        // テキスト入力コントロールにフォーカスがある場合はスルー
        if (ActiveControl is TextBox or NumericUpDown or ComboBox or RichTextBox)
            return;

        switch (e.KeyCode)
        {
            case Keys.Space:
            case Keys.Enter:
                e.Handled = true;          // DataGridView の行移動等を抑止
                e.SuppressKeyPress = true;
                HandlePlayToggleKey();
                break;

            case Keys.Escape:
                e.Handled = true;
                e.SuppressKeyPress = true;
                HandleEscapeKey();
                break;
        }
    }

    private void HandlePlayToggleKey()
    {
        switch (_playController.State)
        {
            case PlayState.Idle:
            case PlayState.Completed:
            case PlayState.Error:
                // 演奏開始（ヒート選択中のみ）
                if (_selectedHeat != null)
                    _playController.StartPlay(_selectedHeat);
                break;

            case PlayState.Playing:
            case PlayState.FadingIn:
                // 演奏中 → 曲終了（フェードアウト）
                _playController.EndWithFadeOut();
                break;

            case PlayState.Paused:
                // 停止中 → 最初から再開
                _playController.ResumeFromPause(fromStart: true);
                break;

            // FadingOut 中は無視
        }
    }

    private void HandleEscapeKey()
    {
        switch (_playController.State)
        {
            case PlayState.Playing:
            case PlayState.FadingIn:
            case PlayState.FadingOut:
                // 演奏中 → 緊急停止
                _playController.EmergencyStop();
                break;

            case PlayState.Paused:
                // 停止中 → 途中から再開（フェードイン）
                _playController.ResumeFromPause(fromStart: false);
                break;

            // Idle / Completed / Error は無視
        }
    }

    private void btnPlayStart_Click(object sender, EventArgs e)
    {
        if (_playController.State == PlayState.Paused)
        {
            // 最初から再開
            _playController.ResumeFromPause(fromStart: true);
        }
        else if (_selectedHeat != null)
        {
            _playController.StartPlay(_selectedHeat);
        }
    }

    private void btnEmergencyStop_Click(object sender, EventArgs e)
    {
        if (_playController.State == PlayState.Paused)
        {
            // 途中から再開
            _playController.ResumeFromPause(fromStart: false);
        }
        else
        {
            _playController.EmergencyStop();
        }
    }

    private void btnEndWithFadeOut_Click(object sender, EventArgs e)
    {
        _playController.EndWithFadeOut();
    }

    private async void btnDsRefresh_Click(object sender, EventArgs e)
    {
        if (_webSocketService != null)
        {
            await _webSocketService.SendGmAskDsAsync(_dsStatus.OrgCd, _dsStatus.CompNo);
        }
    }

    private void btnSettings_Click(object sender, EventArgs e)
    {
        using (var fSettings = new F020_Settings(_settingsService))
        {
            if (fSettings.ShowDialog() == DialogResult.OK)
            {
                _assignService.AutoAssign(_settingsService.Current);
                RefreshGrid();
            }
        }
    }

    // ────────── BGM / SE 制御 ──────────

    private void BgmSeButton_Click(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.Tag is string key)
        {
            int state = _bgmPlayingState[key];
            if (state == 1) // 演奏中 → フェードアウト停止
            {
                StopBgmSe(key);
            }
            else if (state == 0) // 停止中 → 演奏開始
            {
                PlayBgmSe(key);
            }
            // state==2(フェードアウト中) はボタン操作を無視
        }
    }

    private void PlayBgmSe(string key)
    {
        // ⑤ オナーダンスはフォルダからランダム選曲
        if (key.StartsWith("オナーダンス_"))
        {
            PlayHonorDance(key);
            return;
        }

        if (!_settingsService.Current.BgmSeSettings.TryGetValue(key, out var bgmSetting) ||
            string.IsNullOrEmpty(bgmSetting.FilePath) || !File.Exists(bgmSetting.FilePath))
        {
            MessageBox.Show($"「{key}」のファイルが設定されていないか、見つかりません。", "再生エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        float gain = _playbackEngine.CalcGain(bgmSetting.FilePath, _settingsService.Current.TargetVolume_dBFS);

        if (BgmKeys.IsSeChannel(key))
        {
            _playbackEngine.PlaySe(bgmSetting.FilePath, gain);
            _bgmPlayingState[key] = 1;
        }
        else
        {
            // 他のBGM(チャンネルB)再生中のものをすべて停止状態にする
            foreach (var bKey in _bgmPlayingState.Keys.ToList())
            {
                if (!BgmKeys.IsSeChannel(bKey))
                    _bgmPlayingState[bKey] = 0;
            }

            _playbackEngine.PlayBgm(bgmSetting.FilePath, bgmSetting.Repeat, gain);
            _bgmPlayingState[key] = 1;
        }

        UpdateBgmButtonUi();
    }

    // ⑤ オナーダンス：フォルダからランダム選曲して演奏
    private void PlayHonorDance(string key)
    {
        // 対応するフォルダを BgmSeSettings から取得（FilePath をフォルダパスとして流用）
        if (!_settingsService.Current.BgmSeSettings.TryGetValue(key, out var bgmSetting) ||
            string.IsNullOrEmpty(bgmSetting.FilePath) || !Directory.Exists(bgmSetting.FilePath))
        {
            MessageBox.Show($"「{key}」のフォルダが設定されていないか、見つかりません。\n設定画面でフォルダパスを登録してください。", "再生エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var files = Directory.GetFiles(bgmSetting.FilePath, "*.mp3")
            .Concat(Directory.GetFiles(bgmSetting.FilePath, "*.wav"))
            .ToArray();

        if (files.Length == 0)
        {
            MessageBox.Show($"「{key}」フォルダに音楽ファイルが見つかりません。", "再生エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var rng = new Random();
        string chosen = files[rng.Next(files.Length)];

        // 他のBGMを停止
        foreach (var bKey in _bgmPlayingState.Keys.ToList())
        {
            if (!BgmKeys.IsSeChannel(bKey))
                _bgmPlayingState[bKey] = 0;
        }

        float gain = _playbackEngine.CalcGain(chosen, _settingsService.Current.TargetVolume_dBFS);

        // イントロスキップ（オナーダンスにも適用。ただし種目 P は除外）
        double skipTo = 0.0;
        var cfg = _settingsService.Current;
        string dncCdFromKey = key.Replace("オナーダンス_", "");
        if (cfg.IntroSkipEnabled && dncCdFromKey != "P")
        {
            skipTo = _playbackEngine.CalcIntroSkipPosition(
                chosen, cfg.IntroSkipThresholdDbfs, cfg.IntroSkipPreRollSeconds);
        }

        _playbackEngine.PlayBgm(chosen, bgmSetting.Repeat, gain, skipTo);

        // ④ オナーダンス演奏情報を記録（スライダー反映用）
        _honorCurrentKey = key;
        _honorPlayStart = DateTime.Now;
        // オナーダンスは演奏時間設定なし（固定180秒をデフォルト表示）
        _honorTargetSeconds = 0;
        try
        {
            using var reader = new NAudio.Wave.AudioFileReader(chosen);
            _honorTargetSeconds = reader.TotalTime.TotalSeconds;
        }
        catch { _honorTargetSeconds = 180; }

        _bgmPlayingState[key] = 1;
        UpdateBgmButtonUi();
    }

    private void StopBgmSe(string key)
    {
        double fadeOutSec = _settingsService.Current.FadeOutSeconds;

        // ③ フェードアウト中は黄色にしてから完了後にリセット
        _bgmPlayingState[key] = 2;
        UpdateBgmButtonUi();

        if (BgmKeys.IsSeChannel(key))
        {
            _playbackEngine.StopSeWithFadeOut(fadeOutSec, () =>
            {
                _bgmPlayingState[key] = 0;
                if (key == _honorCurrentKey) _honorCurrentKey = null;
                this.BeginInvoke(UpdateBgmButtonUi);
            });
        }
        else
        {
            _playbackEngine.StopBgmWithFadeOut(fadeOutSec, () =>
            {
                _bgmPlayingState[key] = 0;
                if (key == _honorCurrentKey) _honorCurrentKey = null;
                this.BeginInvoke(UpdateBgmButtonUi);
            });
        }
    }

    private void UpdateBgmButtonUi()
    {
        foreach (var pair in _bgmButtons)
        {
            var key = pair.Key;
            var btn = pair.Value;
            int state = _bgmPlayingState[key];

            switch (state)
            {
                case 1: // 演奏中 → 緑
                    btn.BackColor = Color.LightGreen;
                    btn.UseVisualStyleBackColor = false;
                    break;
                case 2: // フェードアウト中 → ③ 黄色
                    btn.BackColor = Color.Yellow;
                    btn.UseVisualStyleBackColor = false;
                    break;
                default: // 停止中 → デフォルト
                    btn.UseVisualStyleBackColor = true;
                    break;
            }
        }
    }

    // ────────── タイマー・表示更新 ──────────

    private void timerPlayback_Tick(object sender, EventArgs e)
    {
        if (_playController.State == PlayState.Playing ||
            _playController.State == PlayState.FadingIn ||
            _playController.State == PlayState.FadingOut ||
            _playController.State == PlayState.Paused)
        {
            // 競技曲チャンネルA の進行をスライダーに表示
            var elapsed = _playbackEngine.CompetitionElapsed;

            if (_playController.CurrentHeat != null)
            {
                if (_playController.CurrentHeat.DncCd == "P")
                {
                    lblTimeProgress.Text = $"{elapsed:mm\\:ss} / Paso";
                    prgPlayback.Value = 0;
                }
                else
                {
                    var playTimeSetting = _assignService.FindPlayTimeSetting(_settingsService.Current, _playController.CurrentHeat.DncCd, _playController.CurrentHeat.KbnNo, _playController.CurrentHeat.RndNo);
                    double targetSec = playTimeSetting?.PlaySeconds ?? 90.0;

                    lblTimeProgress.Text = $"{elapsed:mm\\:ss} / {TimeSpan.FromSeconds(targetSec):mm\\:ss}";

                    if (targetSec > 0)
                    {
                        int val = (int)((elapsed.TotalSeconds / targetSec) * 100);
                        prgPlayback.Value = Math.Min(100, Math.Max(0, val));
                    }
                }
            }
        }
        // ④ オナーダンス演奏中はスライダーに反映
        else if (_honorCurrentKey != null && _playbackEngine.IsBgmPlaying)
        {
            double elapsed = (DateTime.Now - _honorPlayStart).TotalSeconds;
            double total = _honorTargetSeconds > 0 ? _honorTargetSeconds : 180.0;

            lblTimeProgress.Text = $"{TimeSpan.FromSeconds(elapsed):mm\\:ss} / {TimeSpan.FromSeconds(total):mm\\:ss}";
            int val = (int)((elapsed / total) * 100);
            prgPlayback.Value = Math.Min(100, Math.Max(0, val));
        }
        else
        {
            lblTimeProgress.Text = "00:00 / 00:00";
            prgPlayback.Value = 0;
        }

        // VU メーター更新（タイマー毎に再描画）
        vuMeter.SetSampler(_playbackEngine.VuSampler);
        vuMeter.Tick();

        // ツールバーの進行情報更新
        UpdateProgressLabel();
    }

    private void UpdateProgressLabel()
    {
        if (_selectedHeat != null)
        {
            string kubunName = _daMaster.Kubuns.FirstOrDefault(k => k.KbnNo == _selectedHeat.KbnNo)?.KbnDispName ?? _selectedHeat.KbnNo;
            string roundName = _daMaster.Kubuns.FirstOrDefault(k => k.KbnNo == _selectedHeat.KbnNo)?.Rounds.FirstOrDefault(r => r.RndNo == _selectedHeat.RndNo)?.RndNameJ ?? _selectedHeat.RndNo;
            lblProgressInfo.Text = $"選択中: {kubunName} - {roundName} (第 {_selectedHeat.HeatNo} ヒート)";
        }
        else
        {
            lblProgressInfo.Text = "現在進行情報: なし";
        }
    }

    private void UpdatePlayControlUi()
    {
        switch (_playController.State)
        {
            case PlayState.Idle:
            case PlayState.Completed:
            case PlayState.Error:
                btnPlayStart.Text = "▶ 演奏開始";
                btnPlayStart.Enabled = _selectedHeat != null;
                btnEmergencyStop.Text = "⏹ 緊急停止";
                btnEmergencyStop.Enabled = false;
                btnEndWithFadeOut.Enabled = false;
                break;

            case PlayState.Playing:
            case PlayState.FadingIn:
            case PlayState.FadingOut:
                btnPlayStart.Text = "▶ 演奏開始";
                btnPlayStart.Enabled = false;
                btnEmergencyStop.Text = "⏹ 緊急停止";
                btnEmergencyStop.Enabled = true;
                btnEndWithFadeOut.Enabled = true;
                break;

            case PlayState.Paused:
                btnPlayStart.Text = "▷ 最初から再開";
                btnPlayStart.Enabled = true;
                btnEmergencyStop.Text = "▷ 途中から再開";
                btnEmergencyStop.Enabled = true;
                btnEndWithFadeOut.Enabled = true;
                break;
        }
    }

    private void UpdateWsStateLabel(ConnectionState state)
    {
        lblWsState.Text = state.ToString();
        switch (state)
        {
            case ConnectionState.Connected:
                lblWsState.ForeColor = Color.Green;
                break;
            case ConnectionState.Connecting:
                lblWsState.ForeColor = Color.Orange;
                break;
            case ConnectionState.Reconnecting:
                lblWsState.ForeColor = Color.DarkOrange;
                break;
            case ConnectionState.Disconnected:
                lblWsState.ForeColor = Color.Red;
                break;
        }
    }

    // ────────── WebSocket 受信イベント ──────────

    private void OnWsStateChanged(ConnectionState state)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() => UpdateWsStateLabel(state));
    }

    /// <summary>
    /// MU_ANS_LOGIN 受信時：AuthId が更新された後に GM_ASK_DS を送信してセッションを登録する。
    /// これにより再接続後も DSMc からの演奏開始指示（MU_NOTIFY_PLAY）を受け取れる。
    /// </summary>
    private void OnLoginReceived(MuAnsLoginPayload payload)
    {
        if (this.IsDisposed) return;
        // AuthId はすでに WebSocketService 内で更新済み。
        // GM_ASK_DS を送ることでサーバー側に新セッションを登録し、
        // 最新 DS 状態もグリッドに反映する。
        if (!string.IsNullOrEmpty(_dsStatus.OrgCd) && !string.IsNullOrEmpty(_dsStatus.CompNo))
        {
            _ = _webSocketService.SendGmAskDsAsync(_dsStatus.OrgCd, _dsStatus.CompNo);
        }
    }

    private void OnDsStatusReceived(string orgCd, string json)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() =>
        {
            try
            {
                var ds = JsonConvert.DeserializeObject<DsStatus>(json);
                if (ds != null)
                {
                    _dsStatus = ds;
                    _assignService.SyncWithDsStatus(_dsStatus, _daMaster, _settingsService.Current);
                    RefreshGrid();
                }
            }
            catch { }
        });
    }

    private void OnNotifyPlayReceived(MuNotifyPlayPayload p)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() =>
        {
            // 該当するヒートを検索
            // Key2（KbnNo/RndNo/DGrpNo）がある場合は完全一致、
            // DSMc 電文のように Key1（PrgNo）＋種目順＋ヒート番号のみの場合はそちらで照合する
            HeatAssignment? heat;
            if (!string.IsNullOrEmpty(p.KbnNo))
            {
                heat = _assignService.CurrentData.HeatAssignments.FirstOrDefault(h =>
                    h.KbnNo == p.KbnNo &&
                    h.RndNo == p.RndNo &&
                    h.DGrpNo == p.DGrpNo &&
                    h.DncNo == p.DncNo &&
                    h.HeatNo == p.HeatNo);
            }
            else
            {
                // DSMc 由来の電文は Key1（PrgNo）＋種目順＋ヒート番号で照合
                heat = _assignService.CurrentData.HeatAssignments.FirstOrDefault(h =>
                    h.PrgNo == p.PrgNo &&
                    h.DncNo == p.DncNo &&
                    h.HeatNo == p.HeatNo);
            }

            if (heat != null)
            {
                // グリッド上の該当行を選択
                foreach (DataGridViewRow row in dgvHeats.Rows)
                {
                    if (row.Tag is HeatAssignment h && h == heat)
                    {
                        row.Selected = true;
                        dgvHeats.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }

                _selectedHeat = heat;
                UpdateSelectedHeatUi();
                _playController.StartPlay(heat);
            }
        });
    }

    private void OnNotifyStopReceived()
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() => _playController.EmergencyStop());
    }

    private void OnNotifyResumeReceived(MuNotifyResumePayload p)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() =>
        {
            bool fromStart = p.ResumeMode == "FromStart";
            _playController.ResumeFromPause(fromStart);
        });
    }

    private void OnNotifyEndReceived(MuNotifyEndPayload? payload)
    {
        // payload は将来的な拡張用（現状はペイロード内容を使用しない）
        if (this.IsDisposed) return;
        this.BeginInvoke(() => _playController.EndWithFadeOut());
    }

    private void OnNotifyBgmPlayReceived(MuNotifyBgmPayload p)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() => PlayBgmSe(p.BgmKey));
    }

    private void OnNotifyBgmStopReceived(MuNotifyBgmPayload p)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() => StopBgmSe(p.BgmKey));
    }

    private void OnHeatNotifyReceived()
    {
        // 再度 DS 状態を要求
        if (this.IsDisposed) return;
        this.BeginInvoke(async () =>
        {
            await _webSocketService.SendGmAskDsAsync(_dsStatus.OrgCd, _dsStatus.CompNo);
        });
    }

    private void OnNotifyPlayTimeReceived(MuNotifyPlayTimePayload p)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() =>
        {
            var settings = p.PlayTimeSettings.Select(s => new PlayTimeSetting
            {
                KbnNo      = s.KbnNo,
                RndNo      = s.RndNo,
                DanceCode  = s.DanceCode,
                PlaySeconds    = s.PlaySeconds,
                PasoPattern    = s.PasoPattern,
                Paso2ndSeconds = s.Paso2ndSeconds,
                PasoFullSeconds = s.PasoFullSeconds,
            }).ToList();

            _settingsService.Current.PlayTimeSettings = settings;
            _settingsService.Save();
            RefreshGrid();
        });
    }

    private void OnPlayStateChanged(PlayState state)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() =>
        {
            UpdatePlayControlUi();

            // 演奏状態をグリッド行の色・② 日本語テキストに反映
            foreach (DataGridViewRow row in dgvHeats.Rows)
            {
                if (row.Tag is HeatAssignment h)
                {
                    row.Cells["Status"].Value = StatusDisplayName(h.PlayStatus);
                    ApplyRowColor(row, h.PlayStatus);
                }
            }

            // ① 演奏完了後は次のヒートへ行選択を移動
            if (state == PlayState.Idle &&
                _playController.CurrentHeat != null)
            {
                MoveSelectionToNextHeat(_playController.CurrentHeat);
            }
        });
    }

    /// <summary>
    /// 指定ヒートの次の行を選択する。
    /// 次行が存在しない場合は選択を変更しない。
    /// </summary>
    private void MoveSelectionToNextHeat(HeatAssignment completedHeat)
    {
        for (int i = 0; i < dgvHeats.Rows.Count - 1; i++)
        {
            if (dgvHeats.Rows[i].Tag is HeatAssignment h && h == completedHeat)
            {
                var nextRow = dgvHeats.Rows[i + 1];
                dgvHeats.ClearSelection();
                nextRow.Selected = true;
                dgvHeats.FirstDisplayedScrollingRowIndex = nextRow.Index;
                _selectedHeat = nextRow.Tag as HeatAssignment;
                UpdateSelectedHeatUi();
                break;
            }
        }
    }
}
