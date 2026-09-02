using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DSMusic.Core;

public class AssignService
{
    private readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
    private AssignData _currentData = new();
    public AssignData CurrentData => _currentData;

    private string GetFilePath(string orgCd, string cmpNo)
    {
        return Path.Combine(_baseDir, $"DSMusic_Assign_{orgCd}_{cmpNo}.json");
    }

    public void Load(string orgCd, string cmpNo)
    {
        var path = GetFilePath(orgCd, cmpNo);
        if (!File.Exists(path))
        {
            _currentData = new AssignData { OrgCd = orgCd, CmpNo = cmpNo };
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            _currentData = JsonConvert.DeserializeObject<AssignData>(json) ?? new AssignData { OrgCd = orgCd, CmpNo = cmpNo };
        }
        catch
        {
            _currentData = new AssignData { OrgCd = orgCd, CmpNo = cmpNo };
        }
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(_currentData.OrgCd) || string.IsNullOrEmpty(_currentData.CmpNo)) return;
        var path = GetFilePath(_currentData.OrgCd, _currentData.CmpNo);
        _currentData.LastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var json = JsonConvert.SerializeObject(_currentData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public void SyncWithDsStatus(DsStatus dsStatus, DaMaster daMaster, AppSettings settings)
    {
        var newAssignments = new List<HeatAssignment>();

        foreach (var floor in dsStatus.Floors)
        {
            foreach (var prog in floor.Progresses)
            {
                foreach (var dnc in prog.Dances)
                {
                    var dncCd = FindDanceCode(daMaster, prog.KbnNo, prog.RndNo, prog.DGrpNo, dnc.DncNo);
                    if (string.IsNullOrEmpty(dncCd)) continue;

                    foreach (var heat in dnc.Heats)
                    {
                        var existing = _currentData.HeatAssignments.FirstOrDefault(h =>
                            h.KbnNo == prog.KbnNo &&
                            h.RndNo == prog.RndNo &&
                            h.DGrpNo == prog.DGrpNo &&
                            h.DncNo == dnc.DncNo &&
                            h.HeatNo == heat.HeatNo);

                        if (existing != null)
                        {
                            existing.DncCd = dncCd;
                            // Key1情報を更新（DS_Statusの内容が変わった場合に対応）
                            existing.FloorCd  = floor.FlrCd;
                            existing.PrgNo    = prog.PrgNo;
                            existing.PrgSubNo = prog.PrgSubNo;
                            newAssignments.Add(existing);
                        }
                        else
                        {
                            newAssignments.Add(new HeatAssignment
                            {
                                // Key1
                                FloorCd  = floor.FlrCd,
                                PrgNo    = prog.PrgNo,
                                PrgSubNo = prog.PrgSubNo,
                                // Key2
                                KbnNo    = prog.KbnNo,
                                RndNo    = prog.RndNo,
                                DGrpNo   = prog.DGrpNo,
                                DncNo    = dnc.DncNo,
                                DncCd    = dncCd,
                                HeatNo   = heat.HeatNo,
                                PlayStatus = "Pending"
                            });
                        }
                    }
                }
            }
        }

        _currentData.HeatAssignments = newAssignments;
        AutoAssign(settings);
        Save();
    }

    private string FindDanceCode(DaMaster daMaster, string kbnNo, string rndNo, string dGrpNo, int dncNo)
    {
        var kubun = daMaster.Kubuns.FirstOrDefault(k => k.KbnNo == kbnNo);
        var round = kubun?.Rounds.FirstOrDefault(r => r.RndNo == rndNo);
        var dgrp = round?.DGroups.FirstOrDefault(d => d.DGrpNo == dGrpNo);
        var dance = dgrp?.Dances.FirstOrDefault(d => d.DncNo == dncNo);
        return dance?.DncCd ?? "";
    }

    public void AutoAssign(AppSettings settings)
    {
        var rng = new Random();
        
        var grouped = _currentData.HeatAssignments
            .GroupBy(h => new { h.DncCd, h.KbnNo, h.RndNo })
            .ToList();

        foreach (var group in grouped)
            {
            var dncCd = group.Key.DncCd;
            var kbnNo = group.Key.KbnNo;
            var rndNo = group.Key.RndNo;

            var folderSetting = FindFolderSetting(settings, dncCd, kbnNo, rndNo);
            if (folderSetting == null) continue;

            // Get play time setting to determine Paso pattern if P
            var playTime = FindPlayTimeSetting(settings, dncCd, kbnNo, rndNo);
            var pasoPattern = playTime?.PasoPattern ?? "2ndHighlight";

            var files = GetMusicFiles(folderSetting, dncCd, pasoPattern);
            if (files.Count == 0) continue;

            var rule = settings.AssignRules.FirstOrDefault(r => r.DanceCode == dncCd) 
                ?? new AssignRule { DanceCode = dncCd, Mode = "Random", AvoidConsecutive = true };

            var heats = group.OrderBy(h => h.HeatNo).ToList();
            string lastAssignedFile = "";

            for (int i = 0; i < heats.Count; i++)
            {
                var heat = heats[i];
                if (heat.ManuallyAssigned && !string.IsNullOrEmpty(heat.FilePath) && File.Exists(heat.FilePath))
                {
                    lastAssignedFile = heat.FilePath;
                    continue;
                }

                string selectedFile = "";
                if (rule.Mode == "Fixed")
                {
                    int index = i % files.Count;
                    selectedFile = files[index];
                }
                else
                {
                    if (rule.AvoidConsecutive && files.Count > 1)
                    {
                        var candidates = files.Where(f => f != lastAssignedFile).ToList();
                        if (candidates.Count == 0) candidates = files;
                        selectedFile = candidates[rng.Next(candidates.Count)];
                    }
                    else
                    {
                        selectedFile = files[rng.Next(files.Count)];
                    }
                }

                if (!heat.ManuallyAssigned)
                {
                    heat.FilePath = selectedFile;
                }
                lastAssignedFile = heat.FilePath;
            }
        }
    }

    public FolderSetting? FindFolderSetting(AppSettings settings, string dncCd, string kbnNo, string rndNo)
    {
        return settings.FolderSettings.FirstOrDefault(f => f.DanceCode == dncCd && f.KbnNo == kbnNo && f.RndNo == rndNo)
            ?? settings.FolderSettings.FirstOrDefault(f => f.DanceCode == dncCd && f.KbnNo == kbnNo && string.IsNullOrEmpty(f.RndNo))
            ?? settings.FolderSettings.FirstOrDefault(f => f.DanceCode == dncCd && string.IsNullOrEmpty(f.KbnNo) && string.IsNullOrEmpty(f.RndNo));
    }

    public PlayTimeSetting? FindPlayTimeSetting(AppSettings settings, string dncCd, string kbnNo, string rndNo)
    {
        // ① 種目 + 区分 + ラウンド 完全一致
        return settings.PlayTimeSettings.FirstOrDefault(f => f.DanceCode == dncCd && f.KbnNo == kbnNo && f.RndNo == rndNo)
            // ② 種目 + 区分一致（ラウンド空欄 = 区分共通）
            ?? settings.PlayTimeSettings.FirstOrDefault(f => f.DanceCode == dncCd && f.KbnNo == kbnNo && string.IsNullOrEmpty(f.RndNo))
            // ③ 種目のみ一致（区分・ラウンド空欄 = 種目共通）
            ?? settings.PlayTimeSettings.FirstOrDefault(f => f.DanceCode == dncCd && string.IsNullOrEmpty(f.KbnNo) && string.IsNullOrEmpty(f.RndNo))
            // ④ 種目・区分空欄 + ラウンド一致（= ラウンド共通デフォルト。例:「1次予選は全種目 1:10」）
            ?? settings.PlayTimeSettings.FirstOrDefault(f => string.IsNullOrEmpty(f.DanceCode) && string.IsNullOrEmpty(f.KbnNo) && f.RndNo == rndNo)
            // ⑤ すべて空欄（= 全体デフォルト）
            ?? settings.PlayTimeSettings.FirstOrDefault(f => string.IsNullOrEmpty(f.DanceCode) && string.IsNullOrEmpty(f.KbnNo) && string.IsNullOrEmpty(f.RndNo));
    }

    public List<string> GetMusicFiles(FolderSetting folderSetting, string dncCd, string pasoPattern = "2ndHighlight")
    {
        var files = new List<string>();
        if (dncCd == "P")
        {
            if (pasoPattern == "FullChorus")
            {
                if (Directory.Exists(folderSetting.FolderPath_FullChorus))
                {
                    files.AddRange(Directory.GetFiles(folderSetting.FolderPath_FullChorus, "*.mp3"));
                    files.AddRange(Directory.GetFiles(folderSetting.FolderPath_FullChorus, "*.wav"));
                }
            }
            else
            {
                if (Directory.Exists(folderSetting.FolderPath_2ndHighlight))
                {
                    files.AddRange(Directory.GetFiles(folderSetting.FolderPath_2ndHighlight, "*.mp3"));
                    files.AddRange(Directory.GetFiles(folderSetting.FolderPath_2ndHighlight, "*.wav"));
                }
            }
        }
        else
        {
            if (Directory.Exists(folderSetting.FolderPath))
            {
                files.AddRange(Directory.GetFiles(folderSetting.FolderPath, "*.mp3"));
                files.AddRange(Directory.GetFiles(folderSetting.FolderPath, "*.wav"));
            }
        }
        return files.Distinct().ToList();
    }

    public List<string> GetMusicFilesForFolder(string folderPath)
    {
        var files = new List<string>();
        if (Directory.Exists(folderPath))
        {
            files.AddRange(Directory.GetFiles(folderPath, "*.mp3"));
            files.AddRange(Directory.GetFiles(folderPath, "*.wav"));
        }
        return files;
    }
}
