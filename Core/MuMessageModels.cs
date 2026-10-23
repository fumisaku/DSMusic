using Newtonsoft.Json;

namespace DSMusic.Core;

// サーバーからの受信電文

public class MuAnsLoginPayload
{
    [JsonProperty("authId")]
    public string AuthId { get; set; } = "";

    [JsonProperty("competitions")]
    public List<CompetitionInfo> Competitions { get; set; } = new();
}

public class CompetitionInfo
{
    [JsonProperty("orgCd")]
    public string OrgCd { get; set; } = "";

    [JsonProperty("cmpNo")]
    public string CmpNo { get; set; } = "";

    [JsonProperty("cmpName")]
    public string CmpName { get; set; } = "";

    [JsonProperty("cmpDate")]
    public string CmpDate { get; set; } = "";

    [JsonProperty("cmpPlace")]
    public string CmpPlace { get; set; } = "";
}

// MU_NOTIFY_PLAY ペイロード
// DSMc の MC_MuPlayPayload と英語キーを統一（FloorCd/PrgNo/DncNo/HeatNo）
public class MuNotifyPlayPayload
{
    [JsonProperty("OrgCd")]    public string OrgCd    { get; set; } = "";
    [JsonProperty("CmpNo")]    public string CmpNo    { get; set; } = "";
    // Key1（フロア・進行番号）: サーバーが DS_Status 更新・GM_TIMER 生成に使用
    [JsonProperty("FloorCd")]  public string? FloorCd  { get; set; }
    [JsonProperty("PrgNo")]    public string? PrgNo    { get; set; }
    [JsonProperty("PrgSubNo")] public string? PrgSubNo { get; set; }
    // Key2（区分・ラウンド等）: 曲ファイル検索に使用（HeatAssignment と照合）
    [JsonProperty("KbnNo")]    public string KbnNo    { get; set; } = "";
    [JsonProperty("RndNo")]    public string RndNo    { get; set; } = "";
    [JsonProperty("DGrpNo")]   public string DGrpNo   { get; set; } = "";
    [JsonProperty("DncNo")]    public int    DncNo    { get; set; }
    [JsonProperty("HeatNo")]   public int    HeatNo   { get; set; }
}

// MU_NOTIFY_END ペイロード（Key1情報付き）
public class MuNotifyEndPayload
{
    [JsonProperty("OrgCd")]    public string OrgCd    { get; set; } = "";
    [JsonProperty("CmpNo")]    public string CmpNo    { get; set; } = "";
    [JsonProperty("FloorCd")]  public string? FloorCd  { get; set; }
    [JsonProperty("PrgNo")]    public string? PrgNo    { get; set; }
    [JsonProperty("PrgSubNo")] public string? PrgSubNo { get; set; }
    [JsonProperty("DncNo")]    public int    DncNo    { get; set; }
    [JsonProperty("HeatNo")]   public int    HeatNo   { get; set; }
}

// MU_NOTIFY_RESUME ペイロード
public class MuNotifyResumePayload
{
    [JsonProperty("OrgCd")] public string OrgCd { get; set; } = "";
    [JsonProperty("CmpNo")] public string CmpNo { get; set; } = "";
    [JsonProperty("ResumeMode")] public string ResumeMode { get; set; } = "FromPause";
}

// MU_NOTIFY_BGM_PLAY ペイロード
public class MuNotifyBgmPayload
{
    [JsonProperty("OrgCd")] public string OrgCd { get; set; } = "";
    [JsonProperty("CmpNo")] public string CmpNo { get; set; } = "";
    [JsonProperty("BgmKey")] public string BgmKey { get; set; } = "";
}

// MU_NOTIFY_PLAYTIME ペイロード
public class MuNotifyPlayTimePayload
{
    [JsonProperty("OrgCd")]    public string OrgCd { get; set; } = "";
    [JsonProperty("CmpNo")]    public string CmpNo { get; set; } = "";
    [JsonProperty("PlayTimeSettings")] public List<MuPlayTimeSetting> PlayTimeSettings { get; set; } = new();
}

public class MuPlayTimeSetting
{
    [JsonProperty("KbnNo")]           public string KbnNo { get; set; } = "";
    [JsonProperty("RndNo")]           public string RndNo { get; set; } = "";
    [JsonProperty("DanceCode")]       public string DanceCode { get; set; } = "";
    [JsonProperty("PlaySeconds")]     public double? PlaySeconds { get; set; }
    [JsonProperty("PasoPattern")]     public string? PasoPattern { get; set; }
    [JsonProperty("Paso2ndSeconds")]  public double? Paso2ndSeconds { get; set; }
    [JsonProperty("PasoFullSeconds")] public double? PasoFullSeconds { get; set; }
}

// MU_NOTIFY_EXTEND ペイロード（演奏時間延長・短縮）
public class MuNotifyExtendPayload
{
    [JsonProperty("OrgCd")]       public string OrgCd       { get; set; } = "";
    [JsonProperty("CmpNo")]       public string CmpNo       { get; set; } = "";
    [JsonProperty("DeltaSeconds")] public int   DeltaSeconds { get; set; }
}

// MU_STATUS 送信ペイロード
public class MuStatusPayload
{
    [JsonProperty("OrgCd")]          public string  OrgCd          { get; set; } = "";
    [JsonProperty("CmpNo")]          public string  CmpNo          { get; set; } = "";
    [JsonProperty("PlayState")]      public string  PlayState      { get; set; } = "Idle";
    [JsonProperty("Channel")]        public string  Channel        { get; set; } = "Competition";
    [JsonProperty("BgmKey")]         public string? BgmKey         { get; set; }
    // Key1（フロア・進行番号）: サーバーの DS_HeatAEndTM 更新に使用
    [JsonProperty("FloorCd")]        public string? FloorCd        { get; set; }
    [JsonProperty("PrgNo")]          public string? PrgNo          { get; set; }
    [JsonProperty("PrgSubNo")]       public string? PrgSubNo       { get; set; }
    // Key2（区分・ラウンド等）
    [JsonProperty("KbnNo")]          public string? KbnNo          { get; set; }
    [JsonProperty("RndNo")]          public string? RndNo          { get; set; }
    [JsonProperty("DGrpNo")]         public string? DGrpNo         { get; set; }
    [JsonProperty("DncNo")]          public int?    DncNo          { get; set; }
    [JsonProperty("DncCd")]          public string? DncCd          { get; set; }
    [JsonProperty("HeatNo")]         public int?    HeatNo         { get; set; }
    [JsonProperty("ElapsedSeconds")] public double? ElapsedSeconds { get; set; }
    [JsonProperty("TotalSeconds")]   public double? TotalSeconds   { get; set; }
    /// <summary>演奏時間のテキスト表示（例: "P:2ndHL", "P:Full"）。TotalSecondsが0の種目用。</summary>
    [JsonProperty("PlayTimeLabel")]  public string? PlayTimeLabel  { get; set; }
    [JsonProperty("ErrorMessage")]   public string? ErrorMessage   { get; set; }
}
