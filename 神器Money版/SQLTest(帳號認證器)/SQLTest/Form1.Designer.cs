namespace SQLTest
{
    partial class Form1
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
            this.btnReadAll = new System.Windows.Forms.Button();
            this.btnInsertInto = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnReadAll
            // 
            this.btnReadAll.Location = new System.Drawing.Point(12, 12);
            this.btnReadAll.Name = "btnReadAll";
            this.btnReadAll.Size = new System.Drawing.Size(103, 38);
            this.btnReadAll.TabIndex = 0;
            this.btnReadAll.Text = "讀取全部資料";
            this.btnReadAll.UseVisualStyleBackColor = true;
            this.btnReadAll.Click += new System.EventHandler(this.btnReadAll_Click);
            // 
            // btnInsertInto
            // 
            this.btnInsertInto.Location = new System.Drawing.Point(12, 56);
            this.btnInsertInto.Name = "btnInsertInto";
            this.btnInsertInto.Size = new System.Drawing.Size(103, 38);
            this.btnInsertInto.TabIndex = 1;
            this.btnInsertInto.Text = "新增一筆資料";
            this.btnInsertInto.UseVisualStyleBackColor = true;
            this.btnInsertInto.Click += new System.EventHandler(this.btnInsertInto_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(131, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(103, 38);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "斷開連結";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Location = new System.Drawing.Point(12, 100);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(103, 38);
            this.btnDeleteAll.TabIndex = 3;
            this.btnDeleteAll.Text = "清除所有資料";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 262);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnInsertInto);
            this.Controls.Add(this.btnReadAll);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnReadAll;
        private System.Windows.Forms.Button btnInsertInto;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDeleteAll;
    }
}

