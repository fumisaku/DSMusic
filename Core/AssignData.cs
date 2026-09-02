using Newtonsoft.Json;

namespace DSMusic.Core;

public class AssignData
{
    [JsonProperty("OrgCd")]
    public string OrgCd { get; set; } = "";

    [JsonProperty("CmpNo")]
    public string CmpNo { get; set; } = "";

    [JsonProperty("LastUpdated")]
    public string LastUpdated { get; set; } = "";

    [JsonProperty("HeatAssignments")]
    public List<HeatAssignment> HeatAssignments { get; set; } = new();
}

public class HeatAssignment
{
    // Key1（フロア・進行番号）: MU_NOTIFY_PLAY/END および MU_STATUS でサーバーに送信
    [JsonProperty("FloorCd")]
    public string? FloorCd { get; set; }

    [JsonProperty("PrgNo")]
    public string? PrgNo { get; set; }

    [JsonProperty("PrgSubNo")]
    public string? PrgSubNo { get; set; }

    // Key2（区分・ラウンド等）
    [JsonProperty("KbnNo")]
    public string KbnNo { get; set; } = "";

    [JsonProperty("RndNo")]
    public string RndNo { get; set; } = "";

    [JsonProperty("DGrpNo")]
    public string DGrpNo { get; set; } = "";

    [JsonProperty("DncNo")]
    public int DncNo { get; set; }

    [JsonProperty("DncCd")]
    public string DncCd { get; set; } = "";

    [JsonProperty("HeatNo")]
    public int HeatNo { get; set; }

    [JsonProperty("FilePath")]
    public string FilePath { get; set; } = "";

    [JsonProperty("ManuallyAssigned")]
    public bool ManuallyAssigned { get; set; } = false;

    [JsonProperty("PlayStatus")]
    public string PlayStatus { get; set; } = "Pending"; // Pending/Playing/Completed/Skipped

    [JsonProperty("PlayStartTime")]
    public string? PlayStartTime { get; set; }

    [JsonProperty("PlayEndTime")]
    public string? PlayEndTime { get; set; }

    [JsonProperty("ActualPlaySeconds")]
    public double? ActualPlaySeconds { get; set; }
}
