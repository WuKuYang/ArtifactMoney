namespace ImageViewer
{
    partial class EMImageViewer
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.PreviewImage = new System.Windows.Forms.PictureBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.ptbImageMap = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnView_1 = new System.Windows.Forms.Button();
            this.btnView_2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblPA = new System.Windows.Forms.Label();
            this.lblMA = new System.Windows.Forms.Label();
            this.lblMB = new System.Windows.Forms.Label();
            this.lblPB = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblAvgmm = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbImageMap)).BeginInit();
            this.SuspendLayout();
            // 
            // PreviewImage
            // 
            this.PreviewImage.BackColor = System.Drawing.Color.Black;
            this.PreviewImage.Location = new System.Drawing.Point(4, 4);
            this.PreviewImage.Name = "PreviewImage";
            this.PreviewImage.Size = new System.Drawing.Size(989, 720);
            this.PreviewImage.TabIndex = 0;
            this.PreviewImage.TabStop = false;
            this.PreviewImage.Click += new System.EventHandler(this.PreviewImage_Click);
            this.PreviewImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PreviewImage_MouseDown);
            this.PreviewImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PreviewImage_MouseMove);
            this.PreviewImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PreviewImage_MouseUp);
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(1145, 633);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(140, 91);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCancel.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCancel.Location = new System.Drawing.Point(999, 633);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(140, 91);
            this.BtnCancel.TabIndex = 1;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // ptbImageMap
            // 
            this.ptbImageMap.BackColor = System.Drawing.Color.YellowGreen;
            this.ptbImageMap.Location = new System.Drawing.Point(999, 4);
            this.ptbImageMap.Name = "ptbImageMap";
            this.ptbImageMap.Size = new System.Drawing.Size(286, 271);
            this.ptbImageMap.TabIndex = 2;
            this.ptbImageMap.TabStop = false;
            this.ptbImageMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ptbImageMap_MouseDown);
            this.ptbImageMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ptbImageMap_MouseMove);
            this.ptbImageMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ptbImageMap_MouseUp);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(999, 281);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(283, 35);
            this.button1.TabIndex = 3;
            this.button1.Text = "depth measurement";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnView_1
            // 
            this.btnView_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnView_1.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnView_1.Location = new System.Drawing.Point(999, 349);
            this.btnView_1.Name = "btnView_1";
            this.btnView_1.Size = new System.Drawing.Size(140, 57);
            this.btnView_1.TabIndex = 4;
            this.btnView_1.Text = "View 1";
            this.btnView_1.UseVisualStyleBackColor = true;
            this.btnView_1.Click += new System.EventHandler(this.btnView_1_Click);
            // 
            // btnView_2
            // 
            this.btnView_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnView_2.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnView_2.Location = new System.Drawing.Point(1142, 349);
            this.btnView_2.Name = "btnView_2";
            this.btnView_2.Size = new System.Drawing.Size(140, 57);
            this.btnView_2.TabIndex = 5;
            this.btnView_2.Text = "View 2";
            this.btnView_2.UseVisualStyleBackColor = true;
            this.btnView_2.Click += new System.EventHandler(this.btnView_2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 14F);
            this.label1.Location = new System.Drawing.Point(1000, 323);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 23);
            this.label1.TabIndex = 6;
            this.label1.Text = "Pixel :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 14F);
            this.label2.Location = new System.Drawing.Point(1000, 424);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(190, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "View 1 depth(mm)";
            // 
            // lblPA
            // 
            this.lblPA.AutoSize = true;
            this.lblPA.Font = new System.Drawing.Font("Verdana", 14F);
            this.lblPA.Location = new System.Drawing.Point(1069, 323);
            this.lblPA.Name = "lblPA";
            this.lblPA.Size = new System.Drawing.Size(58, 23);
            this.lblPA.TabIndex = 8;
            this.lblPA.Text = "lblPA";
            // 
            // lblMA
            // 
            this.lblMA.AutoSize = true;
            this.lblMA.Font = new System.Drawing.Font("Verdana", 14F);
            this.lblMA.Location = new System.Drawing.Point(1196, 424);
            this.lblMA.Name = "lblMA";
            this.lblMA.Size = new System.Drawing.Size(63, 23);
            this.lblMA.TabIndex = 9;
            this.lblMA.Text = "lblMA";
            // 
            // lblMB
            // 
            this.lblMB.AutoSize = true;
            this.lblMB.Font = new System.Drawing.Font("Verdana", 14F);
            this.lblMB.Location = new System.Drawing.Point(1195, 460);
            this.lblMB.Name = "lblMB";
            this.lblMB.Size = new System.Drawing.Size(63, 23);
            this.lblMB.TabIndex = 13;
            this.lblMB.Text = "lblMB";
            // 
            // lblPB
            // 
            this.lblPB.AutoSize = true;
            this.lblPB.Font = new System.Drawing.Font("Verdana", 14F);
            this.lblPB.Location = new System.Drawing.Point(1224, 323);
            this.lblPB.Name = "lblPB";
            this.lblPB.Size = new System.Drawing.Size(58, 23);
            this.lblPB.TabIndex = 12;
            this.lblPB.Text = "lblPB";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Verdana", 14F);
            this.label7.Location = new System.Drawing.Point(999, 460);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(190, 23);
            this.label7.TabIndex = 11;
            this.label7.Text = "View 2 depth(mm)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Verdana", 14F);
            this.label8.Location = new System.Drawing.Point(1155, 323);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 23);
            this.label8.TabIndex = 10;
            this.label8.Text = "Pixel :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 14F);
            this.label3.Location = new System.Drawing.Point(1021, 595);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 23);
            this.label3.TabIndex = 14;
            this.label3.Text = "average (mm)";
            // 
            // lblAvgmm
            // 
            this.lblAvgmm.AutoSize = true;
            this.lblAvgmm.Font = new System.Drawing.Font("Verdana", 14F);
            this.lblAvgmm.Location = new System.Drawing.Point(1173, 595);
            this.lblAvgmm.Name = "lblAvgmm";
            this.lblAvgmm.Size = new System.Drawing.Size(106, 23);
            this.lblAvgmm.TabIndex = 15;
            this.lblAvgmm.Text = "lblAvgmm";
            // 
            // EMImageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1294, 733);
            this.Controls.Add(this.lblAvgmm);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblMB);
            this.Controls.Add(this.lblPB);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblMA);
            this.Controls.Add(this.lblPA);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnView_2);
            this.Controls.Add(this.btnView_1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ptbImageMap);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.PreviewImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "EMImageViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ImageViewer";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ptbImageMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PreviewImage;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.PictureBox ptbImageMap;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnView_1;
        private System.Windows.Forms.Button btnView_2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblPA;
        private System.Windows.Forms.Label lblMA;
        private System.Windows.Forms.Label lblMB;
        private System.Windows.Forms.Label lblPB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblAvgmm;
    }
}

