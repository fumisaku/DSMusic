using Newtonsoft.Json;

namespace DSMusic.Core;

// DS_Status
public class DsStatus
{
    [JsonProperty("DS_OrgCD")] public string OrgCd { get; set; } = "";
    [JsonProperty("DS_CompNo")] public string CompNo { get; set; } = "";
    [JsonProperty("DS_FLOORs")] public List<DsFloor> Floors { get; set; } = new();
}

public class DsFloor
{
    [JsonProperty("DS_FlrCd")] public string FlrCd { get; set; } = "";
    [JsonProperty("DS_CurPrgNo")] public string? CurPrgNo { get; set; }
    [JsonProperty("DS_PRGRSs")] public List<DsProgress> Progresses { get; set; } = new();
}

public class DsProgress
{
    [JsonProperty("DS_PrgNo")] public string PrgNo { get; set; } = "";
    [JsonProperty("DS_PrgSubNo")] public string PrgSubNo { get; set; } = "";
    [JsonProperty("DS_KbnNo")] public string KbnNo { get; set; } = "";
    [JsonProperty("DS_RndNo")] public string RndNo { get; set; } = "";
    [JsonProperty("DS_DGrpNo")] public string DGrpNo { get; set; } = "";
    [JsonProperty("DS_SortOrder")] public int SortOrder { get; set; }
    [JsonProperty("DS_PrgSts")] public string PrgSts { get; set; } = "0";
    [JsonProperty("DS_CurDanNo")] public string? CurDanNo { get; set; }
    [JsonProperty("DS_CurHeatId")] public string? CurHeatId { get; set; }
    [JsonProperty("DS_PRGDANCEs")] public List<DsPrgDance> Dances { get; set; } = new();
}

public class DsPrgDance
{
    [JsonProperty("DS_DncNo")] public int DncNo { get; set; }
    [JsonProperty("DS_DncSts")] public string DncSts { get; set; } = "0";
    [JsonProperty("DS_PRGHEATs")] public List<DsPrgHeat> Heats { get; set; } = new();
}

public class DsPrgHeat
{
    [JsonProperty("DS_HeatId")] public string HeatId { get; set; } = "";
    [JsonProperty("DS_HeatNo")] public int HeatNo { get; set; }
    [JsonProperty("DS_HeatSts")] public string HeatSts { get; set; } = "0";
}

// DA_Master（必要最小限）
public class DaMaster
{
    [JsonProperty("DA_OrgCD")] public string OrgCd { get; set; } = "";
    [JsonProperty("DA_CompNo")] public string CompNo { get; set; } = "";
    [JsonProperty("DA_CompName")] public string CompName { get; set; } = "";
    [JsonProperty("DB_KUBUNs")] public List<DaKubun> Kubuns { get; set; } = new();
}

public class DaKubun
{
    [JsonProperty("DB_KbnNo")] public string KbnNo { get; set; } = "";
    [JsonProperty("DB_KbnName")] public string KbnName { get; set; } = "";
    [JsonProperty("DB_KbnDispName")] public string KbnDispName { get; set; } = "";
    [JsonProperty("DC_ROUNDs")] public List<DaRound> Rounds { get; set; } = new();
}

public class DaRound
{
    [JsonProperty("DC_RndNo")] public string RndNo { get; set; } = "";
    [JsonProperty("DC_RndName_J")] public string RndNameJ { get; set; } = "";
    [JsonProperty("DD_DGRPs")] public List<DaDGroup> DGroups { get; set; } = new();
}

public class DaDGroup
{
    [JsonProperty("DD_DGrpNo")] public string DGrpNo { get; set; } = "";
    [JsonProperty("DD_DGrpName")] public string DGrpName { get; set; } = "";
    [JsonProperty("DE_DANCEs")] public List<DaDance> Dances { get; set; } = new();
}

public class DaDance
{
    [JsonProperty("DE_DncNo")] public int DncNo { get; set; }
    [JsonProperty("DE_DncCd")] public string DncCd { get; set; } = "";
    [JsonProperty("DE_DncNm_J")] public string DncNameJ { get; set; } = "";
}
