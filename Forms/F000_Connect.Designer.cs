namespace DSMusic.Forms
{
    partial class F000_Connect
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
            this.lblIP = new System.Windows.Forms.Label();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lstCompetitions = new System.Windows.Forms.ListBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStandalone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(20, 20);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(107, 15);
            this.lblIP.TabIndex = 0;
            this.lblIP.Text = "DSServerアドレス:";
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(140, 17);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(150, 23);
            this.txtServerIP.TabIndex = 1;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(310, 20);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(37, 15);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "ポート:";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(360, 17);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(80, 23);
            this.txtServerPort.TabIndex = 3;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(460, 15);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 27);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "接続";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(20, 50);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(82, 15);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "状態: 未接続";
            // 
            // lstCompetitions
            // 
            this.lstCompetitions.FormattingEnabled = true;
            this.lstCompetitions.ItemHeight = 15;
            this.lstCompetitions.Location = new System.Drawing.Point(20, 80);
            this.lstCompetitions.Name = "lstCompetitions";
            this.lstCompetitions.Size = new System.Drawing.Size(540, 244);
            this.lstCompetitions.TabIndex = 6;
            // 
            // btnStart
            //
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(20, 340);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(540, 35);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "競技会を選択して開始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // btnStandalone（⑥ オフライン起動）
            //
            this.btnStandalone.Enabled = false;
            this.btnStandalone.Location = new System.Drawing.Point(20, 385);
            this.btnStandalone.Name = "btnStandalone";
            this.btnStandalone.Size = new System.Drawing.Size(540, 30);
            this.btnStandalone.TabIndex = 8;
            this.btnStandalone.Text = "前回の競技会データでオフライン起動（サーバー未接続）";
            this.btnStandalone.ForeColor = System.Drawing.Color.DarkBlue;
            this.btnStandalone.UseVisualStyleBackColor = true;
            this.btnStandalone.Click += new System.EventHandler(this.btnStandalone_Click);
            //
            // F000_Connect
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 430);
            this.Controls.Add(this.btnStandalone);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lstCompetitions);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtServerPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.lblIP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "F000_Connect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DSMusic - サーバー接続";
            this.Load += new System.EventHandler(this.F000_Connect_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListBox lstCompetitions;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStandalone;
    }
}
