using Newtonsoft.Json;

namespace DSMusic.Core;

public class AppSettings
{
    [JsonProperty("Connection")]
    public ConnectionSettings Connection { get; set; } = new();

    [JsonProperty("FadeOutSeconds")]
    public double FadeOutSeconds { get; set; } = 5.0;

    [JsonProperty("FadeInSeconds")]
    public double FadeInSeconds { get; set; } = 3.0;

    [JsonProperty("TargetVolume_dBFS")]
    public double TargetVolume_dBFS { get; set; } = -3.0;

    // ─── イントロスキップ設定 ───────────────────────────────────────────────
    /// <summary>イントロスキップを有効にするか（競技曲・オナーダンスに適用）</summary>
    [JsonProperty("IntroSkipEnabled")]
    public bool IntroSkipEnabled { get; set; } = false;

    /// <summary>この音量（dBFS）以上になる最初の位置をスキップ開始点とする</summary>
    [JsonProperty("IntroSkipThresholdDbfs")]
    public double IntroSkipThresholdDbfs { get; set; } = -40.0;

    /// <summary>スキップ開始点の何秒前から再生するか（プリロール）</summary>
    [JsonProperty("IntroSkipPreRollSeconds")]
    public double IntroSkipPreRollSeconds { get; set; } = 0.5;

    [JsonProperty("FolderSettings")]
    public List<FolderSetting> FolderSettings { get; set; } = new();

    [JsonProperty("PlayTimeSettings")]
    public List<PlayTimeSetting> PlayTimeSettings { get; set; } = new();

    [JsonProperty("AssignRules")]
    public List<AssignRule> AssignRules { get; set; } = new();

    [JsonProperty("BgmSeSettings")]
    public Dictionary<string, BgmSeSetting> BgmSeSettings { get; set; } = new();
}

public class ConnectionSettings
{
    [JsonProperty("ServerIP")]
    public string ServerIP { get; set; } = "127.0.0.1";

    [JsonProperty("ServerPort")]
    public int ServerPort { get; set; } = 7269;
}

public class FolderSetting
{
    [JsonProperty("DanceCode")]
    public string DanceCode { get; set; } = "";

    [JsonProperty("KbnNo")]
    public string KbnNo { get; set; } = "";

    [JsonProperty("RndNo")]
    public string RndNo { get; set; } = "";

    // P以外用
    [JsonProperty("FolderPath")]
    public string FolderPath { get; set; } = "";

    // P用
    [JsonProperty("FolderPath_2ndHighlight")]
    public string FolderPath_2ndHighlight { get; set; } = "";

    [JsonProperty("FolderPath_FullChorus")]
    public string FolderPath_FullChorus { get; set; } = "";
}

public class PlayTimeSetting
{
    [JsonProperty("PatternName")]
    public string PatternName { get; set; } = "";

    [JsonProperty("DanceCode")]
    public string DanceCode { get; set; } = "";

    [JsonProperty("KbnNo")]
    public string KbnNo { get; set; } = "";

    [JsonProperty("RndNo")]
    public string RndNo { get; set; } = "";

    // P以外: 秒数
    [JsonProperty("PlaySeconds")]
    public double? PlaySeconds { get; set; }

    // P用
    [JsonProperty("PasoPattern")]
    public string? PasoPattern { get; set; } // "2ndHighlight" or "FullChorus"

    [JsonProperty("Paso2ndSeconds")]
    public double? Paso2ndSeconds { get; set; }

    [JsonProperty("PasoFullSeconds")]
    public double? PasoFullSeconds { get; set; }
}

public class AssignRule
{
    [JsonProperty("DanceCode")]
    public string DanceCode { get; set; } = "";

    [JsonProperty("Mode")]
    public string Mode { get; set; } = "Random"; // "Fixed" or "Random"

    [JsonProperty("AvoidConsecutive")]
    public bool AvoidConsecutive { get; set; } = true;
}

public class BgmSeSetting
{
    [JsonProperty("FilePath")]
    public string FilePath { get; set; } = "";

    [JsonProperty("Repeat")]
    public bool Repeat { get; set; } = false;

    [JsonProperty("Channel")]
    public string Channel { get; set; } = "B"; // "B" or "C"

    [JsonProperty("ButtonLabel")]
    public string? ButtonLabel { get; set; }
}
