using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DSMusic.Core;
using Newtonsoft.Json;

namespace DSMusic.Forms;

public partial class F000_Connect : Form
{
    private readonly SettingsService _settingsService;
    private WebSocketService? _webSocketService;
    private List<CompetitionInfo> _competitions = new();

    private string? _receivedDsJson;
    private string? _receivedDaJson;
    private readonly object _syncLock = new();

    public F000_Connect(SettingsService settingsService)
    {
        InitializeComponent();
        _settingsService = settingsService;
    }

    private void F000_Connect_Load(object sender, EventArgs e)
    {
        txtServerIP.Text = _settingsService.Current.Connection.ServerIP;
        txtServerPort.Text = _settingsService.Current.Connection.ServerPort.ToString();

        // ⑥ キャッシュが存在する場合はスタンドアロン起動ボタンを有効化
        btnStandalone.Enabled = _settingsService.TryLoadCachedData(out _, out _, out _, out _);
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtServerIP.Text) || !int.TryParse(txtServerPort.Text, out int port))
        {
            MessageBox.Show("有効なサーバーIPとポートを入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnConnect.Enabled = false;

        if (_webSocketService != null)
        {
            _webSocketService.LoginReceived -= OnLoginReceived;
            _webSocketService.StateChanged -= OnStateChanged;
            _webSocketService.DsStatusReceived -= OnDsStatusReceived;
            _webSocketService.DaMasterReceived -= OnDaMasterReceived;
            await _webSocketService.DisconnectAsync();
            _webSocketService.Dispose();
        }

        _settingsService.Current.Connection.ServerIP = txtServerIP.Text.Trim();
        _settingsService.Current.Connection.ServerPort = port;
        _settingsService.Save();

        _webSocketService = new WebSocketService(_settingsService);
        _webSocketService.LoginReceived += OnLoginReceived;
        _webSocketService.StateChanged += OnStateChanged;
        _webSocketService.DsStatusReceived += OnDsStatusReceived;
        _webSocketService.DaMasterReceived += OnDaMasterReceived;

        lblStatus.Text = "状態: 接続中...";
        _webSocketService.StartConnect();
        btnConnect.Enabled = true;
    }

    private void OnStateChanged(ConnectionState state)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() =>
        {
            switch (state)
            {
                case ConnectionState.Connected:
                    lblStatus.Text = "状態: 接続完了（ログイン待ち）";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    break;
                case ConnectionState.Connecting:
                    lblStatus.Text = "状態: 接続中...";
                    lblStatus.ForeColor = System.Drawing.Color.Orange;
                    break;
                case ConnectionState.Reconnecting:
                    lblStatus.Text = "状態: 再接続中...";
                    lblStatus.ForeColor = System.Drawing.Color.DarkOrange;
                    break;
                case ConnectionState.Disconnected:
                    lblStatus.Text = "状態: 未接続";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    break;
            }
        });
    }

    private void OnLoginReceived(MuAnsLoginPayload payload)
    {
        if (this.IsDisposed) return;
        this.BeginInvoke(() =>
        {
            lblStatus.Text = "状態: ログイン完了";
            lblStatus.ForeColor = System.Drawing.Color.Green;

            _competitions = payload.Competitions;
            lstCompetitions.Items.Clear();
            foreach (var cmp in _competitions)
            {
                lstCompetitions.Items.Add($"{cmp.CmpName} ({cmp.CmpDate} @ {cmp.CmpPlace}) [{cmp.OrgCd}-{cmp.CmpNo}]");
            }

            if (lstCompetitions.Items.Count > 0)
            {
                lstCompetitions.SelectedIndex = 0;
                btnStart.Enabled = true;
            }
            else
            {
                btnStart.Enabled = false;
            }
        });
    }

    private async void btnStart_Click(object sender, EventArgs e)
    {
        int idx = lstCompetitions.SelectedIndex;
        if (idx < 0 || idx >= _competitions.Count || _webSocketService == null) return;

        var selectedCmp = _competitions[idx];
        btnStart.Enabled = false;
        lblStatus.Text = "状態: 競技会データ取得中...";

        _webSocketService.OrgCd = selectedCmp.OrgCd;
        _webSocketService.CmpNo = selectedCmp.CmpNo;

        lock (_syncLock)
        {
            _receivedDsJson = null;
            _receivedDaJson = null;
        }

        // DS & DA 要求送信
        await _webSocketService.SendGmAskDsAsync(selectedCmp.OrgCd, selectedCmp.CmpNo);
        await _webSocketService.SendGmAskDaAsync(selectedCmp.OrgCd, selectedCmp.CmpNo);
    }

    private void OnDsStatusReceived(string orgCd, string json)
    {
        lock (_syncLock)
        {
            _receivedDsJson = json;
            CheckAndOpenMain();
        }
    }

    private void OnDaMasterReceived(string orgCd, string json)
    {
        lock (_syncLock)
        {
            _receivedDaJson = json;
            CheckAndOpenMain();
        }
    }

    private void CheckAndOpenMain()
    {
        if (_receivedDsJson != null && _receivedDaJson != null && _webSocketService != null)
        {
            try
            {
                var dsStatus = JsonConvert.DeserializeObject<DsStatus>(_receivedDsJson);
                var daMaster = JsonConvert.DeserializeObject<DaMaster>(_receivedDaJson);

                if (dsStatus != null && daMaster != null)
                {
                    string dsJson = _receivedDsJson;
                    string daJson = _receivedDaJson;

                    // 重複オープンを防ぐため初期化
                    _receivedDsJson = null;
                    _receivedDaJson = null;

                    // ⑥ キャッシュ保存（次回スタンドアロン起動に使用）
                    _settingsService.SaveCachedDsStatus(dsStatus.OrgCd, dsStatus.CompNo, dsJson, daJson);

                    this.BeginInvoke(() =>
                    {
                        // メイン画面の起動
                        var mainForm = new F010_Main(_settingsService, _webSocketService, dsStatus, daMaster);
                        mainForm.FormClosed += (s, ev) => this.Close();
                        this.Hide();
                        mainForm.Show();
                    });
                }
            }
            catch (Exception ex)
            {
                this.BeginInvoke(() =>
                {
                    MessageBox.Show("競技会データの解析に失敗しました。\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnStart.Enabled = true;
                    lblStatus.Text = "状態: ログイン完了";
                });
            }
        }
    }

    // ⑥ スタンドアロン起動（前回キャッシュから起動）
    private void btnStandalone_Click(object sender, EventArgs e)
    {
        if (!_settingsService.TryLoadCachedData(out string dsJson, out string daJson, out string orgCd, out string cmpNo))
        {
            MessageBox.Show("前回の競技会データが見つかりません。\nサーバーに接続して一度データを取得してください。", "データなし", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var dsStatus = JsonConvert.DeserializeObject<DsStatus>(dsJson);
            var daMaster = JsonConvert.DeserializeObject<DaMaster>(daJson);

            if (dsStatus == null || daMaster == null) throw new InvalidOperationException("データ解析失敗");

            // スタンドアロン用: WebSocketService を生成するが接続はしない（OrgCd/CmpNoのみ設定）
            var ws = new WebSocketService(_settingsService);
            ws.OrgCd = orgCd;
            ws.CmpNo = cmpNo;

            var mainForm = new F010_Main(_settingsService, ws, dsStatus, daMaster);
            mainForm.FormClosed += (s, ev) => this.Close();
            this.Hide();
            mainForm.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show("キャッシュデータの読み込みに失敗しました。\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
