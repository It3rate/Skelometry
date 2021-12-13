
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
            // SlugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1167, 797);
            this.Controls.Add(this.btNext);
            this.Controls.Add(this.slugPanel);
            this.Name = "SlugForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel slugPanel;
        private System.Windows.Forms.Button btNext;
    }
}

