namespace SQLTest
{
    partial class SQLForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtFBAccount = new System.Windows.Forms.TextBox();
            this.chkAllow = new System.Windows.Forms.CheckBox();
            this.btnInsertInto = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnShowAll = new System.Windows.Forms.Button();
            this.lstFBAccount = new System.Windows.Forms.ListBox();
            this.btnScanAll = new System.Windows.Forms.Button();
            this.btnScanAllCPUid = new System.Windows.Forms.Button();
            this.lstCPUid = new System.Windows.Forms.ListBox();
            this.btnCheckCPUid = new System.Windows.Forms.Button();
            this.btnDelCPUid = new System.Windows.Forms.Button();
            this.btnInserInToCPUid = new System.Windows.Forms.Button();
            this.chkCPUid = new System.Windows.Forms.CheckBox();
            this.txtCPUid = new System.Windows.Forms.TextBox();
            this.btnGetHDDNum = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnClearFB = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtCookie = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnRefreshCookies = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.lstCookieServer = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFBAccount
            // 
            this.txtFBAccount.Location = new System.Drawing.Point(6, 21);
            this.txtFBAccount.Name = "txtFBAccount";
            this.txtFBAccount.Size = new System.Drawing.Size(578, 22);
            this.txtFBAccount.TabIndex = 0;
            // 
            // chkAllow
            // 
            this.chkAllow.AutoSize = true;
            this.chkAllow.Location = new System.Drawing.Point(7, 49);
            this.chkAllow.Name = "chkAllow";
            this.chkAllow.Size = new System.Drawing.Size(72, 16);
            this.chkAllow.TabIndex = 1;
            this.chkAllow.Text = "允許使用";
            this.chkAllow.UseVisualStyleBackColor = true;
            // 
            // btnInsertInto
            // 
            this.btnInsertInto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInsertInto.Location = new System.Drawing.Point(7, 71);
            this.btnInsertInto.Name = "btnInsertInto";
            this.btnInsertInto.Size = new System.Drawing.Size(117, 30);
            this.btnInsertInto.TabIndex = 2;
            this.btnInsertInto.Text = "修改";
            this.btnInsertInto.UseVisualStyleBackColor = true;
            this.btnInsertInto.Click += new System.EventHandler(this.btnInsertInto_Click);
            // 
            // btnDel
            // 
            this.btnDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDel.Location = new System.Drawing.Point(6, 107);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(118, 30);
            this.btnDel.TabIndex = 3;
            this.btnDel.Text = "刪除";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnShowAll
            // 
            this.btnShowAll.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnShowAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowAll.Location = new System.Drawing.Point(618, 21);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(118, 30);
            this.btnShowAll.TabIndex = 4;
            this.btnShowAll.Text = "查詢帳號使用狀態";
            this.btnShowAll.UseVisualStyleBackColor = true;
            this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
            // 
            // lstFBAccount
            // 
            this.lstFBAccount.Font = new System.Drawing.Font("新細明體", 8F);
            this.lstFBAccount.FormattingEnabled = true;
            this.lstFBAccount.ItemHeight = 11;
            this.lstFBAccount.Location = new System.Drawing.Point(141, 73);
            this.lstFBAccount.Name = "lstFBAccount";
            this.lstFBAccount.Size = new System.Drawing.Size(790, 136);
            this.lstFBAccount.TabIndex = 6;
            this.lstFBAccount.SelectedIndexChanged += new System.EventHandler(this.lstFBAccount_SelectedIndexChanged);
            this.lstFBAccount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstFBAccount_KeyPress);
            // 
            // btnScanAll
            // 
            this.btnScanAll.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnScanAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScanAll.Font = new System.Drawing.Font("微軟正黑體", 13F, System.Drawing.FontStyle.Bold);
            this.btnScanAll.Location = new System.Drawing.Point(6, 143);
            this.btnScanAll.Name = "btnScanAll";
            this.btnScanAll.Size = new System.Drawing.Size(118, 66);
            this.btnScanAll.TabIndex = 7;
            this.btnScanAll.Text = "刷新 →";
            this.btnScanAll.UseVisualStyleBackColor = false;
            this.btnScanAll.Click += new System.EventHandler(this.btnScanAll_Click);
            // 
            // btnScanAllCPUid
            // 
            this.btnScanAllCPUid.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnScanAllCPUid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScanAllCPUid.Font = new System.Drawing.Font("微軟正黑體", 13F, System.Drawing.FontStyle.Bold);
            this.btnScanAllCPUid.Location = new System.Drawing.Point(6, 143);
            this.btnScanAllCPUid.Name = "btnScanAllCPUid";
            this.btnScanAllCPUid.Size = new System.Drawing.Size(118, 66);
            this.btnScanAllCPUid.TabIndex = 15;
            this.btnScanAllCPUid.Text = "刷新→";
            this.btnScanAllCPUid.UseVisualStyleBackColor = false;
            this.btnScanAllCPUid.Click += new System.EventHandler(this.btnScanAllCPUid_Click);
            // 
            // lstCPUid
            // 
            this.lstCPUid.Font = new System.Drawing.Font("新細明體", 8F);
            this.lstCPUid.FormattingEnabled = true;
            this.lstCPUid.ItemHeight = 11;
            this.lstCPUid.Location = new System.Drawing.Point(141, 60);
            this.lstCPUid.Name = "lstCPUid";
            this.lstCPUid.Size = new System.Drawing.Size(791, 191);
            this.lstCPUid.TabIndex = 14;
            this.lstCPUid.SelectedIndexChanged += new System.EventHandler(this.lstCPUid_SelectedIndexChanged);
            // 
            // btnCheckCPUid
            // 
            this.btnCheckCPUid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckCPUid.Location = new System.Drawing.Point(704, 21);
            this.btnCheckCPUid.Name = "btnCheckCPUid";
            this.btnCheckCPUid.Size = new System.Drawing.Size(118, 30);
            this.btnCheckCPUid.TabIndex = 12;
            this.btnCheckCPUid.Text = "查詢帳號使用狀態";
            this.btnCheckCPUid.UseVisualStyleBackColor = true;
            this.btnCheckCPUid.Click += new System.EventHandler(this.btnCheckCPUid_Click);
            // 
            // btnDelCPUid
            // 
            this.btnDelCPUid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelCPUid.Location = new System.Drawing.Point(6, 107);
            this.btnDelCPUid.Name = "btnDelCPUid";
            this.btnDelCPUid.Size = new System.Drawing.Size(118, 30);
            this.btnDelCPUid.TabIndex = 11;
            this.btnDelCPUid.Text = "刪除";
            this.btnDelCPUid.UseVisualStyleBackColor = true;
            this.btnDelCPUid.Click += new System.EventHandler(this.btnDelCPUid_Click);
            // 
            // btnInserInToCPUid
            // 
            this.btnInserInToCPUid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInserInToCPUid.Location = new System.Drawing.Point(7, 71);
            this.btnInserInToCPUid.Name = "btnInserInToCPUid";
            this.btnInserInToCPUid.Size = new System.Drawing.Size(117, 30);
            this.btnInserInToCPUid.TabIndex = 10;
            this.btnInserInToCPUid.Text = "修改";
            this.btnInserInToCPUid.UseVisualStyleBackColor = true;
            this.btnInserInToCPUid.Click += new System.EventHandler(this.btnInserInToCPUid_Click);
            // 
            // chkCPUid
            // 
            this.chkCPUid.AutoSize = true;
            this.chkCPUid.Location = new System.Drawing.Point(7, 49);
            this.chkCPUid.Name = "chkCPUid";
            this.chkCPUid.Size = new System.Drawing.Size(72, 16);
            this.chkCPUid.TabIndex = 9;
            this.chkCPUid.Text = "允許使用";
            this.chkCPUid.UseVisualStyleBackColor = true;
            // 
            // txtCPUid
            // 
            this.txtCPUid.Location = new System.Drawing.Point(6, 21);
            this.txtCPUid.Name = "txtCPUid";
            this.txtCPUid.Size = new System.Drawing.Size(578, 22);
            this.txtCPUid.TabIndex = 8;
            // 
            // btnGetHDDNum
            // 
            this.btnGetHDDNum.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetHDDNum.Location = new System.Drawing.Point(6, 215);
            this.btnGetHDDNum.Name = "btnGetHDDNum";
            this.btnGetHDDNum.Size = new System.Drawing.Size(117, 39);
            this.btnGetHDDNum.TabIndex = 16;
            this.btnGetHDDNum.Text = "取得本機硬體序號";
            this.btnGetHDDNum.UseVisualStyleBackColor = true;
            this.btnGetHDDNum.Click += new System.EventHandler(this.btnGetHDDNum_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.groupBox1.Controls.Add(this.btnClearFB);
            this.groupBox1.Controls.Add(this.txtFBAccount);
            this.groupBox1.Controls.Add(this.chkAllow);
            this.groupBox1.Controls.Add(this.btnInsertInto);
            this.groupBox1.Controls.Add(this.btnDel);
            this.groupBox1.Controls.Add(this.btnShowAll);
            this.groupBox1.Controls.Add(this.lstFBAccount);
            this.groupBox1.Controls.Add(this.btnScanAll);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(11, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(937, 220);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FB 帳號認證";
            // 
            // btnClearFB
            // 
            this.btnClearFB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearFB.Location = new System.Drawing.Point(796, 21);
            this.btnClearFB.Name = "btnClearFB";
            this.btnClearFB.Size = new System.Drawing.Size(118, 30);
            this.btnClearFB.TabIndex = 8;
            this.btnClearFB.Text = "清空";
            this.btnClearFB.UseVisualStyleBackColor = true;
            this.btnClearFB.Click += new System.EventHandler(this.btnClearFB_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.groupBox2.Controls.Add(this.txtCPUid);
            this.groupBox2.Controls.Add(this.chkCPUid);
            this.groupBox2.Controls.Add(this.btnGetHDDNum);
            this.groupBox2.Controls.Add(this.btnInserInToCPUid);
            this.groupBox2.Controls.Add(this.btnScanAllCPUid);
            this.groupBox2.Controls.Add(this.btnDelCPUid);
            this.groupBox2.Controls.Add(this.lstCPUid);
            this.groupBox2.Controls.Add(this.btnCheckCPUid);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(10, 238);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(938, 260);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "硬體序號認證";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.groupBox3.Controls.Add(this.txtTime);
            this.groupBox3.Controls.Add(this.txtEmail);
            this.groupBox3.Controls.Add(this.txtCookie);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.btnRefreshCookies);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Controls.Add(this.lstCookieServer);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(12, 504);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(938, 260);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "帳號集中管理";
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(763, 21);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(167, 22);
            this.txtTime.TabIndex = 17;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(141, 21);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(278, 22);
            this.txtEmail.TabIndex = 16;
            // 
            // txtCookie
            // 
            this.txtCookie.Location = new System.Drawing.Point(425, 21);
            this.txtCookie.Name = "txtCookie";
            this.txtCookie.Size = new System.Drawing.Size(332, 22);
            this.txtCookie.TabIndex = 8;
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(7, 71);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(117, 30);
            this.button2.TabIndex = 10;
            this.button2.Text = "修改";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnRefreshCookies
            // 
            this.btnRefreshCookies.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnRefreshCookies.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshCookies.Font = new System.Drawing.Font("微軟正黑體", 13F, System.Drawing.FontStyle.Bold);
            this.btnRefreshCookies.Location = new System.Drawing.Point(6, 143);
            this.btnRefreshCookies.Name = "btnRefreshCookies";
            this.btnRefreshCookies.Size = new System.Drawing.Size(118, 66);
            this.btnRefreshCookies.TabIndex = 15;
            this.btnRefreshCookies.Text = "刷新→";
            this.btnRefreshCookies.UseVisualStyleBackColor = false;
            this.btnRefreshCookies.Click += new System.EventHandler(this.btnRefreshCookies_Click);
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(6, 107);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(118, 30);
            this.button4.TabIndex = 11;
            this.button4.Text = "刪除";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // lstCookieServer
            // 
            this.lstCookieServer.Font = new System.Drawing.Font("新細明體", 8F);
            this.lstCookieServer.FormattingEnabled = true;
            this.lstCookieServer.ItemHeight = 11;
            this.lstCookieServer.Location = new System.Drawing.Point(141, 60);
            this.lstCookieServer.Name = "lstCookieServer";
            this.lstCookieServer.Size = new System.Drawing.Size(791, 191);
            this.lstCookieServer.TabIndex = 14;
            this.lstCookieServer.SelectedIndexChanged += new System.EventHandler(this.lstCookieServer_SelectedIndexChanged);
            // 
            // SQLForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(972, 750);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "SQLForm";
            this.Text = "拓元神器 認證開通工具 1.2 - 10/30";
            this.Load += new System.EventHandler(this.SQLForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtFBAccount;
        private System.Windows.Forms.CheckBox chkAllow;
        private System.Windows.Forms.Button btnInsertInto;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.ListBox lstFBAccount;
        private System.Windows.Forms.Button btnScanAll;
        private System.Windows.Forms.Button btnScanAllCPUid;
        private System.Windows.Forms.ListBox lstCPUid;
        private System.Windows.Forms.Button btnCheckCPUid;
        private System.Windows.Forms.Button btnDelCPUid;
        private System.Windows.Forms.Button btnInserInToCPUid;
        private System.Windows.Forms.CheckBox chkCPUid;
        private System.Windows.Forms.TextBox txtCPUid;
        private System.Windows.Forms.Button btnGetHDDNum;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtCookie;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnRefreshCookies;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListBox lstCookieServer;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Button btnClearFB;
    }
}