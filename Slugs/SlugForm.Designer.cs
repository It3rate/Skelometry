
namespace Slugs
{
    partial class SlugForm
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
            this.slugPanel = new System.Windows.Forms.Panel();
            this.btNext = new System.Windows.Forms.Button();
            this.scBottom = new System.Windows.Forms.HScrollBar();
            this.scTop = new System.Windows.Forms.HScrollBar();
            this.lbTop = new System.Windows.Forms.Label();
            this.lbBottom = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // slugPanel
            // 
            this.slugPanel.Location = new System.Drawing.Point(26, 82);
            this.slugPanel.Name = "slugPanel";
            this.slugPanel.Size = new System.Drawing.Size(1114, 627);
            this.slugPanel.TabIndex = 0;
            // 
            // btNext
            // 
            this.btNext.Location = new System.Drawing.Point(1065, 23);
            this.btNext.Name = "btNext";
            this.btNext.Size = new System.Drawing.Size(75, 35);
            this.btNext.TabIndex = 1;
            this.btNext.Text = "Next";
            this.btNext.UseVisualStyleBackColor = true;
            // 
            // scBottom
            // 
            this.scBottom.LargeChange = 1;
            this.scBottom.Location = new System.Drawing.Point(105, 756);
            this.scBottom.Maximum = 21;
            this.scBottom.Minimum = -21;
            this.scBottom.Name = "scBottom";
            this.scBottom.Size = new System.Drawing.Size(966, 26);
            this.scBottom.TabIndex = 0;
            this.scBottom.Value = 10;
            // 
            // scTop
            // 
            this.scTop.LargeChange = 1;
            this.scTop.Location = new System.Drawing.Point(105, 722);
            this.scTop.Maximum = 21;
            this.scTop.Minimum = -21;
            this.scTop.Name = "scTop";
            this.scTop.Size = new System.Drawing.Size(966, 26);
            this.scTop.TabIndex = 1;
            this.scTop.Value = -10;
            // 
            // lbTop
            // 
            this.lbTop.AutoSize = true;
            this.lbTop.Location = new System.Drawing.Point(1075, 727);
            this.lbTop.Name = "lbTop";
            this.lbTop.Size = new System.Drawing.Size(51, 20);
            this.lbTop.TabIndex = 2;
            this.lbTop.Text = "label1";
            // 
            // lbBottom
            // 
            this.lbBottom.AutoSize = true;
            this.lbBottom.Location = new System.Drawing.Point(1075, 762);
            this.lbBottom.Name = "lbBottom";
            this.lbBottom.Size = new System.Drawing.Size(51, 20);
            this.lbBottom.TabIndex = 3;
            this.lbBottom.Text = "label2";
            // 
            // SlugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1167, 797);
            this.Controls.Add(this.lbBottom);
            this.Controls.Add(this.lbTop);
            this.Controls.Add(this.scTop);
            this.Controls.Add(this.scBottom);
            this.Controls.Add(this.btNext);
            this.Controls.Add(this.slugPanel);
            this.Name = "SlugForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel slugPanel;
        private System.Windows.Forms.Button btNext;
        private System.Windows.Forms.HScrollBar scBottom;
        private System.Windows.Forms.HScrollBar scTop;
        private System.Windows.Forms.Label lbTop;
        private System.Windows.Forms.Label lbBottom;
    }
}

