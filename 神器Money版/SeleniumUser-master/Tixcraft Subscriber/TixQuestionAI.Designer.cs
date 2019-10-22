namespace Tixcraft_Subscriber
{
    partial class TixQuestionAI
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.txtQuestion = new System.Windows.Forms.TextBox();
            this.txtSystemInfo = new System.Windows.Forms.TextBox();
            this.txtSplitOption = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(13, 30);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(272, 484);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // txtQuestion
            // 
            this.txtQuestion.Location = new System.Drawing.Point(302, 30);
            this.txtQuestion.Multiline = true;
            this.txtQuestion.Name = "txtQuestion";
            this.txtQuestion.Size = new System.Drawing.Size(448, 478);
            this.txtQuestion.TabIndex = 1;
            // 
            // txtSystemInfo
            // 
            this.txtSystemInfo.Location = new System.Drawing.Point(1038, 30);
            this.txtSystemInfo.Multiline = true;
            this.txtSystemInfo.Name = "txtSystemInfo";
            this.txtSystemInfo.Size = new System.Drawing.Size(344, 478);
            this.txtSystemInfo.TabIndex = 2;
            // 
            // txtSplitOption
            // 
            this.txtSplitOption.Location = new System.Drawing.Point(770, 30);
            this.txtSplitOption.Multiline = true;
            this.txtSplitOption.Name = "txtSplitOption";
            this.txtSplitOption.Size = new System.Drawing.Size(262, 478);
            this.txtSplitOption.TabIndex = 3;
            // 
            // TixQuestionAI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1428, 626);
            this.Controls.Add(this.txtSplitOption);
            this.Controls.Add(this.txtSystemInfo);
            this.Controls.Add(this.txtQuestion);
            this.Controls.Add(this.listBox1);
            this.Name = "TixQuestionAI";
            this.Text = "TixQuestionAI";
            this.Load += new System.EventHandler(this.TixQuestionAI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox txtQuestion;
        private System.Windows.Forms.TextBox txtSystemInfo;
        private System.Windows.Forms.TextBox txtSplitOption;
    }
}