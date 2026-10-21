namespace DSMusic.Forms
{
    partial class F010_Main
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnDsRefresh = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.lblProgressInfo = new System.Windows.Forms.Label();
            this.lblCmpName = new System.Windows.Forms.Label();
            this.lblWsState = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.pnlPlayControl = new System.Windows.Forms.Panel();
            this.btnEndWithFadeOut = new System.Windows.Forms.Button();
            this.btnEmergencyStop = new System.Windows.Forms.Button();
            this.btnPlayStart = new System.Windows.Forms.Button();
            this.btnExtendPlus5 = new System.Windows.Forms.Button();
            this.btnExtendMinus5 = new System.Windows.Forms.Button();
            this.prgPlayback = new System.Windows.Forms.ProgressBar();
            this.lblTimeProgress = new System.Windows.Forms.Label();
            this.lblSongDetails = new System.Windows.Forms.Label();
            this.lblHeatDetails = new System.Windows.Forms.Label();
            this.vuMeter = new DSMusic.Forms.VuMeterPanel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.dgvHeats = new System.Windows.Forms.DataGridView();
            this.pnlBgmSe = new System.Windows.Forms.Panel();
            this.grpHonor = new System.Windows.Forms.GroupBox();
            this.flowHonor = new System.Windows.Forms.FlowLayoutPanel();
            this.grpSe = new System.Windows.Forms.GroupBox();
            this.flowSe = new System.Windows.Forms.FlowLayoutPanel();
            this.grpBgm = new System.Windows.Forms.GroupBox();
            this.flowBgm = new System.Windows.Forms.FlowLayoutPanel();
            this.timerPlayback = new System.Windows.Forms.Timer(this.components);
            this.pnlTop.SuspendLayout();
            this.pnlPlayControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeats)).BeginInit();
            this.pnlBgmSe.SuspendLayout();
            this.grpHonor.SuspendLayout();
            this.grpSe.SuspendLayout();
            this.grpBgm.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.LightGray;
            this.pnlTop.Controls.Add(this.btnDsRefresh);
            this.pnlTop.Controls.Add(this.btnSettings);
            this.pnlTop.Controls.Add(this.lblProgressInfo);
            this.pnlTop.Controls.Add(this.lblCmpName);
            this.pnlTop.Controls.Add(this.lblWsState);
            this.pnlTop.Controls.Add(this.lblVersion);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1264, 50);
            this.pnlTop.TabIndex = 0;
            // 
            // btnDsRefresh
            // 
            this.btnDsRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDsRefresh.Location = new System.Drawing.Point(1050, 10);
            this.btnDsRefresh.Name = "btnDsRefresh";
            this.btnDsRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnDsRefresh.TabIndex = 4;
            this.btnDsRefresh.Text = "DS更新";
            this.btnDsRefresh.UseVisualStyleBackColor = true;
            this.btnDsRefresh.Click += new System.EventHandler(this.btnDsRefresh_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.Location = new System.Drawing.Point(1156, 10);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(100, 30);
            this.btnSettings.TabIndex = 3;
            this.btnSettings.Text = "設定";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // lblProgressInfo
            // 
            this.lblProgressInfo.AutoSize = true;
            this.lblProgressInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblProgressInfo.Location = new System.Drawing.Point(400, 18);
            this.lblProgressInfo.Name = "lblProgressInfo";
            this.lblProgressInfo.Size = new System.Drawing.Size(102, 15);
            this.lblProgressInfo.TabIndex = 2;
            this.lblProgressInfo.Text = "現在進行情報: なし";
            // 
            // lblCmpName
            // 
            this.lblCmpName.AutoSize = true;
            this.lblCmpName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCmpName.Location = new System.Drawing.Point(130, 15);
            this.lblCmpName.Name = "lblCmpName";
            this.lblCmpName.Size = new System.Drawing.Size(81, 20);
            this.lblCmpName.TabIndex = 1;
            this.lblCmpName.Text = "競技会名...";
            // 
            // lblWsState
            //
            this.lblWsState.AutoSize = true;
            this.lblWsState.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblWsState.ForeColor = System.Drawing.Color.Green;
            this.lblWsState.Location = new System.Drawing.Point(12, 16);
            this.lblWsState.Name = "lblWsState";
            this.lblWsState.Size = new System.Drawing.Size(81, 19);
            this.lblWsState.TabIndex = 0;
            this.lblWsState.Text = "Connected";
            //
            // lblVersion
            //
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblVersion.ForeColor = System.Drawing.Color.DimGray;
            this.lblVersion.Location = new System.Drawing.Point(750, 34);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(0, 13);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "";
            // 
            // pnlPlayControl
            // 
            this.pnlPlayControl.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlPlayControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPlayControl.Controls.Add(this.vuMeter);
            this.pnlPlayControl.Controls.Add(this.btnExtendPlus5);
            this.pnlPlayControl.Controls.Add(this.btnExtendMinus5);
            this.pnlPlayControl.Controls.Add(this.btnEndWithFadeOut);
            this.pnlPlayControl.Controls.Add(this.btnEmergencyStop);
            this.pnlPlayControl.Controls.Add(this.btnPlayStart);
            this.pnlPlayControl.Controls.Add(this.prgPlayback);
            this.pnlPlayControl.Controls.Add(this.lblTimeProgress);
            this.pnlPlayControl.Controls.Add(this.lblSongDetails);
            this.pnlPlayControl.Controls.Add(this.lblHeatDetails);
            this.pnlPlayControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPlayControl.Location = new System.Drawing.Point(0, 50);
            this.pnlPlayControl.Name = "pnlPlayControl";
            this.pnlPlayControl.Size = new System.Drawing.Size(1264, 150);
            this.pnlPlayControl.TabIndex = 1;
            //
            // vuMeter
            //
            this.vuMeter.Location = new System.Drawing.Point(470, 95);
            this.vuMeter.Name = "vuMeter";
            this.vuMeter.Size = new System.Drawing.Size(620, 45);
            this.vuMeter.TabIndex = 10;
            this.vuMeter.BackColor = System.Drawing.Color.FromArgb(24, 24, 24);
            // 
            // btnEndWithFadeOut
            //
            this.btnEndWithFadeOut.Location = new System.Drawing.Point(330, 100);
            this.btnEndWithFadeOut.Name = "btnEndWithFadeOut";
            this.btnEndWithFadeOut.Size = new System.Drawing.Size(120, 35);
            this.btnEndWithFadeOut.TabIndex = 6;
            this.btnEndWithFadeOut.Text = "⏏ 曲終了";
            this.btnEndWithFadeOut.UseVisualStyleBackColor = true;
            this.btnEndWithFadeOut.Click += new System.EventHandler(this.btnEndWithFadeOut_Click);
            //
            // btnExtendPlus5
            //
            this.btnExtendPlus5.Enabled = false;
            this.btnExtendPlus5.Location = new System.Drawing.Point(1100, 100);
            this.btnExtendPlus5.Name = "btnExtendPlus5";
            this.btnExtendPlus5.Size = new System.Drawing.Size(80, 35);
            this.btnExtendPlus5.TabIndex = 7;
            this.btnExtendPlus5.Text = "+5秒";
            this.btnExtendPlus5.UseVisualStyleBackColor = true;
            this.btnExtendPlus5.Click += new System.EventHandler(this.btnExtendPlus5_Click);
            //
            // btnExtendMinus5
            //
            this.btnExtendMinus5.Enabled = false;
            this.btnExtendMinus5.Location = new System.Drawing.Point(1185, 100);
            this.btnExtendMinus5.Name = "btnExtendMinus5";
            this.btnExtendMinus5.Size = new System.Drawing.Size(80, 35);
            this.btnExtendMinus5.TabIndex = 8;
            this.btnExtendMinus5.Text = "-5秒";
            this.btnExtendMinus5.UseVisualStyleBackColor = true;
            this.btnExtendMinus5.Click += new System.EventHandler(this.btnExtendMinus5_Click);
            // 
            // btnEmergencyStop
            // 
            this.btnEmergencyStop.Location = new System.Drawing.Point(150, 100);
            this.btnEmergencyStop.Name = "btnEmergencyStop";
            this.btnEmergencyStop.Size = new System.Drawing.Size(170, 35);
            this.btnEmergencyStop.TabIndex = 5;
            this.btnEmergencyStop.Text = "⏹ 緊急停止";
            this.btnEmergencyStop.UseVisualStyleBackColor = true;
            this.btnEmergencyStop.Click += new System.EventHandler(this.btnEmergencyStop_Click);
            // 
            // btnPlayStart
            // 
            this.btnPlayStart.Location = new System.Drawing.Point(20, 100);
            this.btnPlayStart.Name = "btnPlayStart";
            this.btnPlayStart.Size = new System.Drawing.Size(120, 35);
            this.btnPlayStart.TabIndex = 4;
            this.btnPlayStart.Text = "▶ 演奏開始";
            this.btnPlayStart.UseVisualStyleBackColor = true;
            this.btnPlayStart.Click += new System.EventHandler(this.btnPlayStart_Click);
            // 
            // prgPlayback
            // 
            this.prgPlayback.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgPlayback.Location = new System.Drawing.Point(20, 70);
            this.prgPlayback.Name = "prgPlayback";
            this.prgPlayback.Size = new System.Drawing.Size(1220, 15);
            this.prgPlayback.TabIndex = 3;
            // 
            // lblTimeProgress
            // 
            this.lblTimeProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeProgress.AutoSize = true;
            this.lblTimeProgress.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTimeProgress.Location = new System.Drawing.Point(1110, 40);
            this.lblTimeProgress.Name = "lblTimeProgress";
            this.lblTimeProgress.Size = new System.Drawing.Size(130, 21);
            this.lblTimeProgress.TabIndex = 2;
            this.lblTimeProgress.Text = "00:00 / 00:00";
            this.lblTimeProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSongDetails
            // 
            this.lblSongDetails.AutoSize = true;
            this.lblSongDetails.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblSongDetails.ForeColor = System.Drawing.Color.DimGray;
            this.lblSongDetails.Location = new System.Drawing.Point(20, 42);
            this.lblSongDetails.Name = "lblSongDetails";
            this.lblSongDetails.Size = new System.Drawing.Size(41, 19);
            this.lblSongDetails.TabIndex = 1;
            this.lblSongDetails.Text = "曲名:";
            // 
            // lblHeatDetails
            // 
            this.lblHeatDetails.AutoSize = true;
            this.lblHeatDetails.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblHeatDetails.Location = new System.Drawing.Point(18, 12);
            this.lblHeatDetails.Name = "lblHeatDetails";
            this.lblHeatDetails.Size = new System.Drawing.Size(126, 25);
            this.lblHeatDetails.TabIndex = 0;
            this.lblHeatDetails.Text = "現在選択ヒート";
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 200);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.dgvHeats);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pnlBgmSe);
            this.splitContainer.Size = new System.Drawing.Size(1264, 561);
            this.splitContainer.SplitterDistance = 632;
            this.splitContainer.TabIndex = 2;
            // 
            // dgvHeats
            // 
            this.dgvHeats.AllowUserToAddRows = false;
            this.dgvHeats.AllowUserToDeleteRows = false;
            this.dgvHeats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHeats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHeats.Location = new System.Drawing.Point(0, 0);
            this.dgvHeats.MultiSelect = false;
            this.dgvHeats.Name = "dgvHeats";
            this.dgvHeats.ReadOnly = true;
            this.dgvHeats.RowHeadersVisible = false;
            this.dgvHeats.RowTemplate.Height = 25;
            this.dgvHeats.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHeats.Size = new System.Drawing.Size(632, 561);
            this.dgvHeats.TabIndex = 0;
            this.dgvHeats.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvHeats_CellContentClick);
            this.dgvHeats.SelectionChanged += new System.EventHandler(this.dgvHeats_SelectionChanged);
            // 
            // pnlBgmSe
            // 
            this.pnlBgmSe.AutoScroll = true;
            this.pnlBgmSe.Controls.Add(this.grpHonor);
            this.pnlBgmSe.Controls.Add(this.grpSe);
            this.pnlBgmSe.Controls.Add(this.grpBgm);
            this.pnlBgmSe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBgmSe.Location = new System.Drawing.Point(0, 0);
            this.pnlBgmSe.Name = "pnlBgmSe";
            this.pnlBgmSe.Padding = new System.Windows.Forms.Padding(10);
            this.pnlBgmSe.Size = new System.Drawing.Size(628, 561);
            this.pnlBgmSe.TabIndex = 0;
            // 
            // grpHonor
            // 
            this.grpHonor.Controls.Add(this.flowHonor);
            this.grpHonor.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpHonor.Location = new System.Drawing.Point(10, 310);
            this.grpHonor.Name = "grpHonor";
            this.grpHonor.Size = new System.Drawing.Size(608, 180);
            this.grpHonor.TabIndex = 2;
            this.grpHonor.TabStop = false;
            this.grpHonor.Text = "オナーダンス";
            // 
            // flowHonor
            // 
            this.flowHonor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowHonor.Location = new System.Drawing.Point(3, 19);
            this.flowHonor.Name = "flowHonor";
            this.flowHonor.Size = new System.Drawing.Size(602, 158);
            this.flowHonor.TabIndex = 0;
            // 
            // grpSe
            // 
            this.grpSe.Controls.Add(this.flowSe);
            this.grpSe.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpSe.Location = new System.Drawing.Point(10, 210);
            this.grpSe.Name = "grpSe";
            this.grpSe.Size = new System.Drawing.Size(608, 100);
            this.grpSe.TabIndex = 1;
            this.grpSe.TabStop = false;
            this.grpSe.Text = "SE";
            // 
            // flowSe
            // 
            this.flowSe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowSe.Location = new System.Drawing.Point(3, 19);
            this.flowSe.Name = "flowSe";
            this.flowSe.Size = new System.Drawing.Size(602, 78);
            this.flowSe.TabIndex = 0;
            // 
            // grpBgm
            // 
            this.grpBgm.Controls.Add(this.flowBgm);
            this.grpBgm.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpBgm.Location = new System.Drawing.Point(10, 10);
            this.grpBgm.Name = "grpBgm";
            this.grpBgm.Size = new System.Drawing.Size(608, 200);
            this.grpBgm.TabIndex = 0;
            this.grpBgm.TabStop = false;
            this.grpBgm.Text = "BGM";
            // 
            // flowBgm
            // 
            this.flowBgm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowBgm.Location = new System.Drawing.Point(3, 19);
            this.flowBgm.Name = "flowBgm";
            this.flowBgm.Size = new System.Drawing.Size(602, 178);
            this.flowBgm.TabIndex = 0;
            // 
            // timerPlayback
            // 
            this.timerPlayback.Interval = 100;
            this.timerPlayback.Tick += new System.EventHandler(this.timerPlayback_Tick);
            // 
            // F010_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.pnlPlayControl);
            this.Controls.Add(this.pnlTop);
            this.KeyPreview = true;
            this.Name = "F010_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DSMusic - 演奏管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.F010_Main_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.F010_Main_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.F010_Main_KeyDown);
            this.Load += new System.EventHandler(this.F010_Main_Load);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlPlayControl.ResumeLayout(false);
            this.pnlPlayControl.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeats)).EndInit();
            this.pnlBgmSe.ResumeLayout(false);
            this.grpHonor.ResumeLayout(false);
            this.grpSe.ResumeLayout(false);
            this.grpBgm.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblWsState;
        private System.Windows.Forms.Label lblCmpName;
        private System.Windows.Forms.Label lblProgressInfo;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnDsRefresh;
        private System.Windows.Forms.Panel pnlPlayControl;
        private System.Windows.Forms.Label lblHeatDetails;
        private System.Windows.Forms.Label lblSongDetails;
        private System.Windows.Forms.Label lblTimeProgress;
        private System.Windows.Forms.ProgressBar prgPlayback;
        private System.Windows.Forms.Button btnPlayStart;
        private System.Windows.Forms.Button btnEmergencyStop;
        private System.Windows.Forms.Button btnEndWithFadeOut;
        private System.Windows.Forms.Button btnExtendPlus5;
        private System.Windows.Forms.Button btnExtendMinus5;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridView dgvHeats;
        private System.Windows.Forms.Panel pnlBgmSe;
        private System.Windows.Forms.GroupBox grpBgm;
        private System.Windows.Forms.FlowLayoutPanel flowBgm;
        private System.Windows.Forms.GroupBox grpSe;
        private System.Windows.Forms.FlowLayoutPanel flowSe;
        private System.Windows.Forms.GroupBox grpHonor;
        private System.Windows.Forms.FlowLayoutPanel flowHonor;
        private System.Windows.Forms.Timer timerPlayback;
        private DSMusic.Forms.VuMeterPanel vuMeter;
    }
}
