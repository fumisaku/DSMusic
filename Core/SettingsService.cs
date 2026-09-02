using Newtonsoft.Json;

namespace DSMusic.Core;

public class SettingsService
{
    private static readonly string SettingsPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DSMusic_Settings.json");

    // ⑥ 前回受信した DS_Status / DA_Master をローカルに保持するパス
    private static readonly string CachedDsStatusPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DSMusic_LastDsStatus.json");
    private static readonly string CachedDaMasterPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DSMusic_LastDaMaster.json");
    private static readonly string CachedOrgInfoPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DSMusic_LastOrgInfo.json");

    private AppSettings _current = new();
    public AppSettings Current => _current;

    public void Load()
    {
        if (!File.Exists(SettingsPath)) { _current = CreateDefault(); return; }
        try
        {
            var json = File.ReadAllText(SettingsPath);
            _current = JsonConvert.DeserializeObject<AppSettings>(json) ?? CreateDefault();
        }
        catch { _current = CreateDefault(); }
    }

    // ⑥ DS_Status / DA_Master キャッシュの読み書き
    public void SaveCachedDsStatus(string orgCd, string cmpNo, string dsJson, string daJson)
    {
        try
        {
            File.WriteAllText(CachedDsStatusPath, dsJson, System.Text.Encoding.UTF8);
            File.WriteAllText(CachedDaMasterPath, daJson, System.Text.Encoding.UTF8);
            var info = JsonConvert.SerializeObject(new { orgCd, cmpNo });
            File.WriteAllText(CachedOrgInfoPath, info, System.Text.Encoding.UTF8);
        }
        catch { }
    }

    /// <summary>前回保存した DS_Status / DA_Master が存在すれば読み込む。</summary>
    public bool TryLoadCachedData(out string dsJson, out string daJson, out string orgCd, out string cmpNo)
    {
        dsJson = daJson = orgCd = cmpNo = "";
        if (!File.Exists(CachedDsStatusPath) || !File.Exists(CachedDaMasterPath) || !File.Exists(CachedOrgInfoPath))
            return false;
        try
        {
            dsJson = File.ReadAllText(CachedDsStatusPath, System.Text.Encoding.UTF8);
            daJson = File.ReadAllText(CachedDaMasterPath, System.Text.Encoding.UTF8);
            dynamic? info = JsonConvert.DeserializeObject(File.ReadAllText(CachedOrgInfoPath));
            orgCd = (string?)info?.orgCd ?? "";
            cmpNo = (string?)info?.cmpNo ?? "";
            return !string.IsNullOrEmpty(dsJson) && !string.IsNullOrEmpty(daJson);
        }
        catch { return false; }
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(_current, Formatting.Indented);
        File.WriteAllText(SettingsPath, json);
    }

    private AppSettings CreateDefault()
    {
        // BgmSeSettings のデフォルトを設定
        var s = new AppSettings();
        foreach (var key in BgmKeys.All)
        {
            s.BgmSeSettings[key] = new BgmSeSetting
            {
                Channel = BgmKeys.IsSeChannel(key) ? "C" : "B",
                Repeat = BgmKeys.IsRepeat(key)
            };
        }
        return s;
    }
}

// BGM/SE キー定義
public static class BgmKeys
{
    public static readonly string[] All = new[]
    {
        "表彰式","ドラムロール","ファンファーレ長","ファンファーレ短","君が代",
        "結果発表待機BGM","休憩中BGM","競技会終了BGM","予備1","予備2","予備3",
        "オナーダンス_W","オナーダンス_T","オナーダンス_V","オナーダンス_F","オナーダンス_Q",
        "オナーダンス_S","オナーダンス_R","オナーダンス_C","オナーダンス_P","オナーダンス_J"
    };

    public static bool IsSeChannel(string key) =>
        key is "ドラムロール" or "ファンファーレ長" or "ファンファーレ短";

    public static bool IsRepeat(string key) =>
        key is "表彰式" or "結果発表待機BGM" or "休憩中BGM";

    public static readonly string[] DanceOrder = new[] { "W","T","V","F","Q","S","R","C","P","J" };
}
