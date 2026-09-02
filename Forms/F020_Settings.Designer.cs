namespace DSMusic.Forms
{
    partial class F020_Settings
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabConnection = new System.Windows.Forms.TabPage();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblIP = new System.Windows.Forms.Label();
            this.tabFolders = new System.Windows.Forms.TabPage();
            this.dgvFolders = new System.Windows.Forms.DataGridView();
            this.tabPlayTime = new System.Windows.Forms.TabPage();
            this.dgvPlayTime = new System.Windows.Forms.DataGridView();
            this.tabVolume = new System.Windows.Forms.TabPage();
            this.numVolume = new System.Windows.Forms.NumericUpDown();
            this.lblVolume = new System.Windows.Forms.Label();
            this.tabAssignRules = new System.Windows.Forms.TabPage();
            this.dgvAssignRules = new System.Windows.Forms.DataGridView();
            this.tabBgmSe = new System.Windows.Forms.TabPage();
            this.dgvBgmSe = new System.Windows.Forms.DataGridView();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.numFadeIn = new System.Windows.Forms.NumericUpDown();
            this.lblFadeIn = new System.Windows.Forms.Label();
            this.numFadeOut = new System.Windows.Forms.NumericUpDown();
            this.lblFadeOut = new System.Windows.Forms.Label();
            this.chkIntroSkip = new System.Windows.Forms.CheckBox();
            this.numIntroSkipDb = new System.Windows.Forms.NumericUpDown();
            this.lblIntroSkipDb = new System.Windows.Forms.Label();
            this.numIntroPreRoll = new System.Windows.Forms.NumericUpDown();
            this.lblIntroPreRoll = new System.Windows.Forms.Label();
            this.lblIntroSkipNote = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabConnection.SuspendLayout();
            this.tabFolders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolders)).BeginInit();
            this.tabPlayTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayTime)).BeginInit();
            this.tabVolume.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVolume)).BeginInit();
            this.tabAssignRules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssignRules)).BeginInit();
            this.tabBgmSe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBgmSe)).BeginInit();
            this.tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFadeIn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFadeOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntroSkipDb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntroPreRoll)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.tabConnection);
            this.tabControl.Controls.Add(this.tabFolders);
            this.tabControl.Controls.Add(this.tabPlayTime);
            this.tabControl.Controls.Add(this.tabVolume);
            this.tabControl.Controls.Add(this.tabAssignRules);
            this.tabControl.Controls.Add(this.tabBgmSe);
            this.tabControl.Controls.Add(this.tabGeneral);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(760, 480);
            this.tabControl.TabIndex = 0;
            // 
            // tabConnection
            // 
            this.tabConnection.Controls.Add(this.txtPort);
            this.tabConnection.Controls.Add(this.lblPort);
            this.tabConnection.Controls.Add(this.txtIP);
            this.tabConnection.Controls.Add(this.lblIP);
            this.tabConnection.Location = new System.Drawing.Point(4, 24);
            this.tabConnection.Name = "tabConnection";
            this.tabConnection.Padding = new System.Windows.Forms.Padding(3);
            this.tabConnection.Size = new System.Drawing.Size(752, 452);
            this.tabConnection.TabIndex = 0;
            this.tabConnection.Text = "接続";
            this.tabConnection.UseVisualStyleBackColor = true;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(130, 70);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 23);
            this.txtPort.TabIndex = 3;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(30, 73);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(37, 15);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "ポート:";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(130, 30);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(200, 23);
            this.txtIP.TabIndex = 1;
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(30, 33);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(86, 15);
            this.lblIP.TabIndex = 0;
            this.lblIP.Text = "Server IPアドレス:";
            // 
            // tabFolders
            // 
            this.tabFolders.Controls.Add(this.dgvFolders);
            this.tabFolders.Location = new System.Drawing.Point(4, 24);
            this.tabFolders.Name = "tabFolders";
            this.tabFolders.Padding = new System.Windows.Forms.Padding(3);
            this.tabFolders.Size = new System.Drawing.Size(752, 452);
            this.tabFolders.TabIndex = 1;
            this.tabFolders.Text = "フォルダ設定";
            this.tabFolders.UseVisualStyleBackColor = true;
            // 
            // dgvFolders
            // 
            this.dgvFolders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFolders.Location = new System.Drawing.Point(3, 3);
            this.dgvFolders.Name = "dgvFolders";
            this.dgvFolders.RowTemplate.Height = 25;
            this.dgvFolders.Size = new System.Drawing.Size(746, 446);
            this.dgvFolders.TabIndex = 0;
            // 
            // tabPlayTime
            // 
            this.tabPlayTime.Controls.Add(this.dgvPlayTime);
            this.tabPlayTime.Location = new System.Drawing.Point(4, 24);
            this.tabPlayTime.Name = "tabPlayTime";
            this.tabPlayTime.Size = new System.Drawing.Size(752, 452);
            this.tabPlayTime.TabIndex = 2;
            this.tabPlayTime.Text = "演奏時間";
            this.tabPlayTime.UseVisualStyleBackColor = true;
            // 
            // dgvPlayTime
            // 
            this.dgvPlayTime.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPlayTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPlayTime.Location = new System.Drawing.Point(0, 0);
            this.dgvPlayTime.Name = "dgvPlayTime";
            this.dgvPlayTime.RowTemplate.Height = 25;
            this.dgvPlayTime.Size = new System.Drawing.Size(752, 452);
            this.dgvPlayTime.TabIndex = 0;
            // 
            // tabVolume
            // 
            this.tabVolume.Controls.Add(this.numVolume);
            this.tabVolume.Controls.Add(this.lblVolume);
            this.tabVolume.Location = new System.Drawing.Point(4, 24);
            this.tabVolume.Name = "tabVolume";
            this.tabVolume.Size = new System.Drawing.Size(752, 452);
            this.tabVolume.TabIndex = 3;
            this.tabVolume.Text = "音量設定";
            this.tabVolume.UseVisualStyleBackColor = true;
            // 
            // numVolume
            // 
            this.numVolume.DecimalPlaces = 1;
            this.numVolume.Location = new System.Drawing.Point(170, 30);
            this.numVolume.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numVolume.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            -2147483648});
            this.numVolume.Name = "numVolume";
            this.numVolume.Size = new System.Drawing.Size(120, 23);
            this.numVolume.TabIndex = 1;
            this.numVolume.Value = new decimal(new int[] {
            3,
            0,
            0,
            -2147483648});
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(30, 32);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(121, 15);
            this.lblVolume.TabIndex = 0;
            this.lblVolume.Text = "目標音量 (dBFS):";
            // 
            // tabAssignRules
            // 
            this.tabAssignRules.Controls.Add(this.dgvAssignRules);
            this.tabAssignRules.Location = new System.Drawing.Point(4, 24);
            this.tabAssignRules.Name = "tabAssignRules";
            this.tabAssignRules.Size = new System.Drawing.Size(752, 452);
            this.tabAssignRules.TabIndex = 4;
            this.tabAssignRules.Text = "アサインルール";
            this.tabAssignRules.UseVisualStyleBackColor = true;
            // 
            // dgvAssignRules
            // 
            this.dgvAssignRules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAssignRules.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAssignRules.Location = new System.Drawing.Point(0, 0);
            this.dgvAssignRules.Name = "dgvAssignRules";
            this.dgvAssignRules.RowTemplate.Height = 25;
            this.dgvAssignRules.Size = new System.Drawing.Size(752, 452);
            this.dgvAssignRules.TabIndex = 0;
            // 
            // tabBgmSe
            // 
            this.tabBgmSe.Controls.Add(this.dgvBgmSe);
            this.tabBgmSe.Location = new System.Drawing.Point(4, 24);
            this.tabBgmSe.Name = "tabBgmSe";
            this.tabBgmSe.Size = new System.Drawing.Size(752, 452);
            this.tabBgmSe.TabIndex = 5;
            this.tabBgmSe.Text = "BGM/SE設定";
            this.tabBgmSe.UseVisualStyleBackColor = true;
            // 
            // dgvBgmSe
            // 
            this.dgvBgmSe.AllowUserToAddRows = false;
            this.dgvBgmSe.AllowUserToDeleteRows = false;
            this.dgvBgmSe.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBgmSe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBgmSe.Location = new System.Drawing.Point(0, 0);
            this.dgvBgmSe.Name = "dgvBgmSe";
            this.dgvBgmSe.RowTemplate.Height = 25;
            this.dgvBgmSe.Size = new System.Drawing.Size(752, 452);
            this.dgvBgmSe.TabIndex = 0;
            this.dgvBgmSe.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBgmSe_CellContentClick);
            // 
            // tabGeneral
            //
            this.tabGeneral.Controls.Add(this.lblIntroSkipNote);
            this.tabGeneral.Controls.Add(this.numIntroPreRoll);
            this.tabGeneral.Controls.Add(this.lblIntroPreRoll);
            this.tabGeneral.Controls.Add(this.numIntroSkipDb);
            this.tabGeneral.Controls.Add(this.lblIntroSkipDb);
            this.tabGeneral.Controls.Add(this.chkIntroSkip);
            this.tabGeneral.Controls.Add(this.numFadeIn);
            this.tabGeneral.Controls.Add(this.lblFadeIn);
            this.tabGeneral.Controls.Add(this.numFadeOut);
            this.tabGeneral.Controls.Add(this.lblFadeOut);
            this.tabGeneral.Location = new System.Drawing.Point(4, 24);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(752, 452);
            this.tabGeneral.TabIndex = 6;
            this.tabGeneral.Text = "全般";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // numFadeIn
            // 
            this.numFadeIn.DecimalPlaces = 1;
            this.numFadeIn.Location = new System.Drawing.Point(170, 70);
            this.numFadeIn.Name = "numFadeIn";
            this.numFadeIn.Size = new System.Drawing.Size(120, 23);
            this.numFadeIn.TabIndex = 3;
            // 
            // lblFadeIn
            // 
            this.lblFadeIn.AutoSize = true;
            this.lblFadeIn.Location = new System.Drawing.Point(30, 72);
            this.lblFadeIn.Name = "lblFadeIn";
            this.lblFadeIn.Size = new System.Drawing.Size(107, 15);
            this.lblFadeIn.TabIndex = 2;
            this.lblFadeIn.Text = "フェードイン時間 (秒):";
            // 
            // numFadeOut
            // 
            this.numFadeOut.DecimalPlaces = 1;
            this.numFadeOut.Location = new System.Drawing.Point(170, 30);
            this.numFadeOut.Name = "numFadeOut";
            this.numFadeOut.Size = new System.Drawing.Size(120, 23);
            this.numFadeOut.TabIndex = 1;
            // 
            // lblFadeOut
            //
            this.lblFadeOut.AutoSize = true;
            this.lblFadeOut.Location = new System.Drawing.Point(30, 32);
            this.lblFadeOut.Name = "lblFadeOut";
            this.lblFadeOut.Size = new System.Drawing.Size(117, 15);
            this.lblFadeOut.TabIndex = 0;
            this.lblFadeOut.Text = "フェードアウト時間 (秒):";
            //
            // chkIntroSkip
            //
            this.chkIntroSkip.AutoSize = true;
            this.chkIntroSkip.Location = new System.Drawing.Point(30, 130);
            this.chkIntroSkip.Name = "chkIntroSkip";
            this.chkIntroSkip.Size = new System.Drawing.Size(220, 19);
            this.chkIntroSkip.TabIndex = 10;
            this.chkIntroSkip.Text = "イントロスキップを有効にする（競技曲・オナーダンス）";
            //
            // numIntroSkipDb
            //
            this.numIntroSkipDb.DecimalPlaces = 1;
            this.numIntroSkipDb.Minimum = new decimal(new int[] { 80, 0, 0, -2147483648 });
            this.numIntroSkipDb.Maximum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numIntroSkipDb.Location = new System.Drawing.Point(230, 160);
            this.numIntroSkipDb.Name = "numIntroSkipDb";
            this.numIntroSkipDb.Size = new System.Drawing.Size(100, 23);
            this.numIntroSkipDb.TabIndex = 11;
            this.numIntroSkipDb.Value = new decimal(new int[] { 40, 0, 0, -2147483648 });
            //
            // lblIntroSkipDb
            //
            this.lblIntroSkipDb.AutoSize = true;
            this.lblIntroSkipDb.Location = new System.Drawing.Point(30, 162);
            this.lblIntroSkipDb.Name = "lblIntroSkipDb";
            this.lblIntroSkipDb.TabIndex = 12;
            this.lblIntroSkipDb.Text = "スキップ閾値 (dBFS):";
            //
            // numIntroPreRoll
            //
            this.numIntroPreRoll.DecimalPlaces = 1;
            this.numIntroPreRoll.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numIntroPreRoll.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            this.numIntroPreRoll.Location = new System.Drawing.Point(230, 195);
            this.numIntroPreRoll.Name = "numIntroPreRoll";
            this.numIntroPreRoll.Size = new System.Drawing.Size(100, 23);
            this.numIntroPreRoll.TabIndex = 13;
            this.numIntroPreRoll.Value = new decimal(new int[] { 5, 0, 0, 65536 }); // 0.5
            //
            // lblIntroPreRoll
            //
            this.lblIntroPreRoll.AutoSize = true;
            this.lblIntroPreRoll.Location = new System.Drawing.Point(30, 197);
            this.lblIntroPreRoll.Name = "lblIntroPreRoll";
            this.lblIntroPreRoll.TabIndex = 14;
            this.lblIntroPreRoll.Text = "スキップ位置のプリロール (秒):";
            //
            // lblIntroSkipNote
            //
            this.lblIntroSkipNote.AutoSize = true;
            this.lblIntroSkipNote.ForeColor = System.Drawing.Color.DimGray;
            this.lblIntroSkipNote.Location = new System.Drawing.Point(30, 230);
            this.lblIntroSkipNote.Name = "lblIntroSkipNote";
            this.lblIntroSkipNote.TabIndex = 15;
            this.lblIntroSkipNote.Text = "※ 閾値: -40dBFS = 小さいイントロをスキップ / -20dBFS = はっきりした音まで飛ばす";
            //
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(606, 505);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 30);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(692, 505);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // F020_Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 547);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "F020_Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "設定";
            this.Load += new System.EventHandler(this.F020_Settings_Load);
            this.tabControl.ResumeLayout(false);
            this.tabConnection.ResumeLayout(false);
            this.tabConnection.PerformLayout();
            this.tabFolders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolders)).EndInit();
            this.tabPlayTime.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayTime)).EndInit();
            this.tabVolume.ResumeLayout(false);
            this.tabVolume.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVolume)).EndInit();
            this.tabAssignRules.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssignRules)).EndInit();
            this.tabBgmSe.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBgmSe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntroSkipDb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntroPreRoll)).EndInit();
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFadeIn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFadeOut)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabConnection;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TabPage tabFolders;
        private System.Windows.Forms.DataGridView dgvFolders;
        private System.Windows.Forms.TabPage tabPlayTime;
        private System.Windows.Forms.DataGridView dgvPlayTime;
        private System.Windows.Forms.TabPage tabVolume;
        private System.Windows.Forms.NumericUpDown numVolume;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.TabPage tabAssignRules;
        private System.Windows.Forms.DataGridView dgvAssignRules;
        private System.Windows.Forms.TabPage tabBgmSe;
        private System.Windows.Forms.DataGridView dgvBgmSe;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.NumericUpDown numFadeIn;
        private System.Windows.Forms.Label lblFadeIn;
        private System.Windows.Forms.NumericUpDown numFadeOut;
        private System.Windows.Forms.Label lblFadeOut;
        private System.Windows.Forms.CheckBox chkIntroSkip;
        private System.Windows.Forms.NumericUpDown numIntroSkipDb;
        private System.Windows.Forms.Label lblIntroSkipDb;
        private System.Windows.Forms.NumericUpDown numIntroPreRoll;
        private System.Windows.Forms.Label lblIntroPreRoll;
        private System.Windows.Forms.Label lblIntroSkipNote;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
