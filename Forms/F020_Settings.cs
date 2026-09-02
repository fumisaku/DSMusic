using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DSMusic.Core;

namespace DSMusic.Forms;

public partial class F020_Settings : Form
{
    private readonly SettingsService _settingsService;

    private BindingList<FolderSetting> _folders = new();
    private BindingList<PlayTimeSetting> _playTimes = new();
    private BindingList<AssignRule> _assignRules = new();
    private BindingList<BgmSeEditRow> _bgmSeRows = new();

    public F020_Settings(SettingsService settingsService)
    {
        InitializeComponent();
        _settingsService = settingsService;
    }

    private void F020_Settings_Load(object sender, EventArgs e)
    {
        var app = _settingsService.Current;

        // Connection
        txtIP.Text = app.Connection.ServerIP;
        txtPort.Text = app.Connection.ServerPort.ToString();

        // Volume & Fading
        numVolume.Value = (decimal)app.TargetVolume_dBFS;
        numFadeOut.Value = (decimal)app.FadeOutSeconds;
        numFadeIn.Value = (decimal)app.FadeInSeconds;

        // Intro Skip
        chkIntroSkip.Checked = app.IntroSkipEnabled;
        numIntroSkipDb.Value = Math.Max(numIntroSkipDb.Minimum, Math.Min(numIntroSkipDb.Maximum, (decimal)app.IntroSkipThresholdDbfs));
        numIntroPreRoll.Value = Math.Max(numIntroPreRoll.Minimum, Math.Min(numIntroPreRoll.Maximum, (decimal)app.IntroSkipPreRollSeconds));

        // Grid 1: Folders
        _folders = new BindingList<FolderSetting>(app.FolderSettings.ToList());
        dgvFolders.DataSource = _folders;
        SetupFoldersGrid();

        // Grid 2: Play Time
        _playTimes = new BindingList<PlayTimeSetting>(app.PlayTimeSettings.ToList());
        dgvPlayTime.DataSource = _playTimes;
        SetupPlayTimeGrid();

        // Grid 3: Assign Rules
        _assignRules = new BindingList<AssignRule>(app.AssignRules.ToList());
        dgvAssignRules.DataSource = _assignRules;
        SetupAssignRulesGrid();

        // Grid 4: BGM / SE Settings
        var tempBgm = new List<BgmSeEditRow>();
        foreach (var key in BgmKeys.All)
        {
            if (app.BgmSeSettings.TryGetValue(key, out var row))
            {
                tempBgm.Add(new BgmSeEditRow
                {
                    Key = key,
                    FilePath = row.FilePath,
                    Repeat = row.Repeat,
                    Channel = row.Channel
                });
            }
            else
            {
                tempBgm.Add(new BgmSeEditRow
                {
                    Key = key,
                    FilePath = "",
                    Repeat = BgmKeys.IsRepeat(key),
                    Channel = BgmKeys.IsSeChannel(key) ? "C" : "B"
                });
            }
        }
        _bgmSeRows = new BindingList<BgmSeEditRow>(tempBgm);
        dgvBgmSe.DataSource = _bgmSeRows;
        SetupBgmSeGrid();
    }

    private void SetupFoldersGrid()
    {
        dgvFolders.AutoGenerateColumns = false;
        dgvFolders.Columns.Clear();

        dgvFolders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DanceCode", HeaderText = "種目", Width = 50 });
        dgvFolders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KbnNo", HeaderText = "区分", Width = 50 });
        dgvFolders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RndNo", HeaderText = "ラウンド", Width = 60 });
        dgvFolders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FolderPath", HeaderText = "フォルダパス (P以外)", Width = 160 });
        dgvFolders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FolderPath_2ndHighlight", HeaderText = "2nd Highlight (P用)", Width = 160 });
        dgvFolders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FolderPath_FullChorus", HeaderText = "Full Chorus (P用)", Width = 160 });
    }

    private void SetupPlayTimeGrid()
    {
        dgvPlayTime.AutoGenerateColumns = false;
        dgvPlayTime.Columns.Clear();

        dgvPlayTime.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PatternName", HeaderText = "パターン名", Width = 100 });
        dgvPlayTime.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DanceCode", HeaderText = "種目", Width = 50 });
        dgvPlayTime.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KbnNo", HeaderText = "区分", Width = 50 });
        dgvPlayTime.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RndNo", HeaderText = "ラウンド", Width = 60 });
        dgvPlayTime.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PlaySeconds", HeaderText = "演奏秒数 (P以外)", Width = 120 });
        dgvPlayTime.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PasoPattern", HeaderText = "Pasoパターン (2nd/Full)", Width = 150 });
    }

    private void SetupAssignRulesGrid()
    {
        dgvAssignRules.AutoGenerateColumns = false;
        dgvAssignRules.Columns.Clear();

        dgvAssignRules.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DanceCode", HeaderText = "種目", Width = 60 });
        dgvAssignRules.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Mode", HeaderText = "モード (Fixed/Random)", Width = 150 });
        dgvAssignRules.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "AvoidConsecutive", HeaderText = "連続回避", Width = 100 });
    }

    private void SetupBgmSeGrid()
    {
        dgvBgmSe.AutoGenerateColumns = false;
        dgvBgmSe.Columns.Clear();

        dgvBgmSe.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Key", HeaderText = "BGM/SEキー", Width = 120, ReadOnly = true });
        dgvBgmSe.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FilePath", HeaderText = "ファイルパス", Width = 350 });

        var btnCol = new DataGridViewButtonColumn
        {
            Name = "Browse",
            HeaderText = "参照",
            Text = "...",
            UseColumnTextForButtonValue = true,
            Width = 40
        };
        dgvBgmSe.Columns.Add(btnCol);

        dgvBgmSe.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Repeat", HeaderText = "リピート", Width = 60 });
        dgvBgmSe.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Channel", HeaderText = "Ch (B/C)", Width = 60 });
    }

    private void dgvBgmSe_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= dgvBgmSe.Rows.Count) return;

        if (dgvBgmSe.Columns[e.ColumnIndex].Name == "Browse")
        {
            var row = _bgmSeRows[e.RowIndex];

            // オナーダンスはフォルダ選択、それ以外はファイル選択
            if (row.Key.StartsWith("オナーダンス_"))
            {
                using var fbd = new FolderBrowserDialog();
                fbd.Description = $"「{row.Key}」の曲フォルダを選択してください";
                if (!string.IsNullOrEmpty(row.FilePath) && System.IO.Directory.Exists(row.FilePath))
                    fbd.InitialDirectory = row.FilePath;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    row.FilePath = fbd.SelectedPath;
                    dgvBgmSe.InvalidateRow(e.RowIndex);
                }
            }
            else
            {
                using var ofd = new OpenFileDialog();
                ofd.Filter = "音楽ファイル|*.mp3;*.wav|すべてのファイル|*.*";
                if (!string.IsNullOrEmpty(row.FilePath) && System.IO.File.Exists(row.FilePath))
                    ofd.InitialDirectory = System.IO.Path.GetDirectoryName(row.FilePath);
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    row.FilePath = ofd.FileName;
                    dgvBgmSe.InvalidateRow(e.RowIndex);
                }
            }
        }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtPort.Text, out int port))
        {
            MessageBox.Show("ポート番号には有効な数値を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var app = _settingsService.Current;

        // Save scalars
        app.Connection.ServerIP = txtIP.Text.Trim();
        app.Connection.ServerPort = port;
        app.TargetVolume_dBFS = (double)numVolume.Value;
        app.FadeOutSeconds = (double)numFadeOut.Value;
        app.FadeInSeconds = (double)numFadeIn.Value;
        app.IntroSkipEnabled = chkIntroSkip.Checked;
        app.IntroSkipThresholdDbfs = (double)numIntroSkipDb.Value;
        app.IntroSkipPreRollSeconds = (double)numIntroPreRoll.Value;

        // Save grids
        app.FolderSettings = _folders.ToList();
        app.PlayTimeSettings = _playTimes.ToList();
        app.AssignRules = _assignRules.ToList();

        // Save BgmSeSettings
        app.BgmSeSettings.Clear();
        foreach (var r in _bgmSeRows)
        {
            app.BgmSeSettings[r.Key] = new BgmSeSetting
            {
                FilePath = r.FilePath,
                Repeat = r.Repeat,
                Channel = r.Channel
            };
        }

        _settingsService.Save();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}

public class BgmSeEditRow
{
    public string Key { get; set; } = "";
    public string FilePath { get; set; } = "";
    public bool Repeat { get; set; }
    public string Channel { get; set; } = "B";
}
