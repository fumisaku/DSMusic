using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DSMusic.Core;

public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Reconnecting
}

public class WebSocketService : IDisposable
{
    // イベント定義
    public event Action<MuAnsLoginPayload>? LoginReceived;
    public event Action<MuNotifyPlayPayload>? NotifyPlayReceived;
    public event Action? NotifyStopReceived;
    public event Action<MuNotifyResumePayload>? NotifyResumeReceived;
    public event Action<MuNotifyEndPayload?>? NotifyEndReceived;
    public event Action<MuNotifyBgmPayload>? NotifyBgmPlayReceived;
    public event Action<MuNotifyBgmPayload>? NotifyBgmStopReceived;
    public event Action<string, string>? DsStatusReceived; // (orgCd, jsonPayload)
    public event Action<string, string>? DaMasterReceived; // (orgCd, jsonPayload)
    public event Action<ConnectionState>? StateChanged;
    public event Action? HeatNotifyReceived;
    public event Action<MuNotifyPlayTimePayload>? NotifyPlayTimeReceived;

    private readonly SettingsService _settingsService;
    private ClientWebSocket? _ws;
    private CancellationTokenSource _cts = new();
    private bool _disposed;

    private ConnectionState _state = ConnectionState.Disconnected;
    public ConnectionState State
    {
        get => _state;
        private set
        {
            if (_state == value) return;
            _state = value;
            StateChanged?.Invoke(_state);
        }
    }

    public string OrgCd { get; set; } = "";
    public string CmpNo { get; set; } = "";
    public string AuthId { get; set; } = "MU_DSMusic";

    public WebSocketService(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void StartConnect()
    {
        _cts = new CancellationTokenSource();
        _ = ConnectLoopAsync(_cts.Token);
    }

    public async Task DisconnectAsync()
    {
        _cts.Cancel();
        if (_ws is { State: WebSocketState.Open })
        {
            try
            {
                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "disconnect", CancellationToken.None);
            }
            catch { /* 無視 */ }
        }
        State = ConnectionState.Disconnected;
    }

    public async Task SendLoginAsync(string orgCd = "", string cmpNo = "")
    {
        // ログイン送信時は AuthId を MU_DSMusic にリセットするか、保持する
        var payload = new
        {
            OrgCd = orgCd,
            CmpNo = cmpNo
        };
        var json = JsonConvert.SerializeObject(payload);
        // フォーマット: ,,[AuthId],MU_LOGIN,[json]
        await SendRawAsync($",,{AuthId},MU_LOGIN,{json}");
    }

    public async Task SendGmAskDsAsync(string orgCd, string cmpNo)
    {
        // フォーマット: {orgCd},{cmpNo},{authId},GM_ASK_DS,{}
        await SendRawAsync($"{orgCd},{cmpNo},{AuthId},GM_ASK_DS,{{}}");
    }

    public async Task SendGmAskDaAsync(string orgCd, string cmpNo)
    {
        // フォーマット: {orgCd},{cmpNo},{authId},GM_ASK_DA,{}
        await SendRawAsync($"{orgCd},{cmpNo},{AuthId},GM_ASK_DA,{{}}");
    }

    public async Task SendMuStatusAsync(MuStatusPayload payload)
    {
        var json = JsonConvert.SerializeObject(payload);
        // フォーマット: {orgCd},{cmpNo},{authId},MU_STATUS,{json}
        await SendRawAsync($"{payload.OrgCd},{payload.CmpNo},{AuthId},MU_STATUS,{json}");
    }

    /// <summary>
    /// 演奏終了をサーバーに通知する（MU_NOTIFY_END）。Key1情報を含めて送信する。
    /// </summary>
    public async Task SendMuNotifyEndAsync(HeatAssignment? heat)
    {
        if (string.IsNullOrEmpty(OrgCd) || string.IsNullOrEmpty(CmpNo)) return;

        var payload = new MuNotifyEndPayload
        {
            OrgCd    = OrgCd,
            CmpNo    = CmpNo,
            FloorCd  = heat?.FloorCd,
            PrgNo    = heat?.PrgNo,
            PrgSubNo = heat?.PrgSubNo,
            DncNo    = heat?.DncNo ?? 0,
            HeatNo   = heat?.HeatNo ?? 0,
        };
        var json = JsonConvert.SerializeObject(payload);
        // フォーマット: {orgCd},{cmpNo},{authId},MU_NOTIFY_END,{json}
        await SendRawAsync($"{OrgCd},{CmpNo},{AuthId},MU_NOTIFY_END,{json}");
    }

    private async Task SendRawAsync(string message)
    {
        if (_ws is not { State: WebSocketState.Open })
        {
            return;
        }
        try
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch
        {
            // 送信失敗時の例外処理
        }
    }

    private async Task ConnectLoopAsync(CancellationToken ct)
    {
        const int retryDelay = 5; // 秒（固定）

        while (!ct.IsCancellationRequested)
        {
            State = ConnectionState.Connecting;
            try
            {
                _ws?.Dispose();
                _ws = new ClientWebSocket();

                var ip = _settingsService.Current.Connection.ServerIP;
                var port = _settingsService.Current.Connection.ServerPort;
                var uri = new Uri($"ws://{ip}:{port}/");

                await _ws.ConnectAsync(uri, ct);
                State = ConnectionState.Connected;

                // 自動でログイン送信（すでに選択されている情報がある場合はそれを使用）
                await SendLoginAsync(OrgCd, CmpNo);

                // 受信ループ
                await ReceiveLoopAsync(ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                State = ConnectionState.Reconnecting;
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(retryDelay), ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        State = ConnectionState.Disconnected;
    }

    private async Task ReceiveLoopAsync(CancellationToken ct)
    {
        var buffer = new byte[64 * 1024];
        var sb = new StringBuilder();

        while (!ct.IsCancellationRequested && _ws is { State: WebSocketState.Open })
        {
            sb.Clear();
            WebSocketReceiveResult result;
            do
            {
                result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                if (result.MessageType == WebSocketMessageType.Close) return;
                sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            } while (!result.EndOfMessage);

            var raw = sb.ToString();
            ParseAndDispatch(raw);
        }
    }

    private void ParseAndDispatch(string raw)
    {
        var parts = raw.Split(',', 5);
        if (parts.Length < 5) return;

        var orgCd = parts[0];
        var cmpNo = parts[1];
        // parts[2] is "SVR" or sender
        var command = parts[3].Trim();
        var jsonPayload = parts[4];

        try
        {
            switch (command)
            {
                case "MU_ANS_LOGIN":
                    {
                        var p = JsonConvert.DeserializeObject<MuAnsLoginPayload>(jsonPayload);
                        if (p != null)
                        {
                            AuthId = p.AuthId;
                            LoginReceived?.Invoke(p);
                        }
                        break;
                    }
                case "MU_NOTIFY_PLAY":
                    {
                        var p = JsonConvert.DeserializeObject<MuNotifyPlayPayload>(jsonPayload);
                        if (p != null) NotifyPlayReceived?.Invoke(p);
                        break;
                    }
                case "MU_NOTIFY_STOP":
                    NotifyStopReceived?.Invoke();
                    break;
                case "MU_NOTIFY_RESUME":
                    {
                        var p = JsonConvert.DeserializeObject<MuNotifyResumePayload>(jsonPayload);
                        if (p != null) NotifyResumeReceived?.Invoke(p);
                        break;
                    }
                case "MU_NOTIFY_END":
                    {
                        // Key1情報付きペイロードをデシリアライズして渡す
                        var p = JsonConvert.DeserializeObject<MuNotifyEndPayload>(jsonPayload);
                        NotifyEndReceived?.Invoke(p);
                    }
                    break;
                case "MU_NOTIFY_BGM_PLAY":
                    {
                        var p = JsonConvert.DeserializeObject<MuNotifyBgmPayload>(jsonPayload);
                        if (p != null) NotifyBgmPlayReceived?.Invoke(p);
                        break;
                    }
                case "MU_NOTIFY_BGM_STOP":
                    {
                        var p = JsonConvert.DeserializeObject<MuNotifyBgmPayload>(jsonPayload);
                        if (p != null) NotifyBgmStopReceived?.Invoke(p);
                        break;
                    }
                case "GM_ANS_DS":
                    DsStatusReceived?.Invoke(orgCd, jsonPayload);
                    break;
                case "GM_ANS_DA":
                    DaMasterReceived?.Invoke(orgCd, jsonPayload);
                    break;
                case "MC_HEAT_NOTIFY":
                    HeatNotifyReceived?.Invoke();
                    break;
                case "MU_NOTIFY_PLAYTIME":
                    {
                        var p = JsonConvert.DeserializeObject<MuNotifyPlayTimePayload>(jsonPayload);
                        if (p != null) NotifyPlayTimeReceived?.Invoke(p);
                        break;
                    }
            }
        }
        catch
        {
            // 解析エラー時の例外処理
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _cts.Cancel();
        _ws?.Dispose();
        GC.SuppressFinalize(this);
    }
}
