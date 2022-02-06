
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
            this.btTrait = new System.Windows.Forms.Button();
            this.btFocal = new System.Windows.Forms.Button();
            this.btBond = new System.Windows.Forms.Button();
            this.btEntity = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // slugPanel
            // 
            this.slugPanel.Location = new System.Drawing.Point(17, 53);
            this.slugPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.slugPanel.Name = "slugPanel";
            this.slugPanel.Size = new System.Drawing.Size(743, 408);
            this.slugPanel.TabIndex = 0;
            // 
            // btNext
            // 
            this.btNext.Location = new System.Drawing.Point(710, 15);
            this.btNext.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btNext.Name = "btNext";
            this.btNext.Size = new System.Drawing.Size(50, 23);
            this.btNext.TabIndex = 1;
            this.btNext.Text = "Next";
            this.btNext.UseVisualStyleBackColor = true;
            // 
            // scBottom
            // 
            this.scBottom.LargeChange = 1;
            this.scBottom.Location = new System.Drawing.Point(70, 491);
            this.scBottom.Maximum = 21;
            this.scBottom.Minimum = -21;
            this.scBottom.Name = "scBottom";
            this.scBottom.Size = new System.Drawing.Size(644, 26);
            this.scBottom.TabIndex = 0;
            this.scBottom.Value = 10;
            // 
            // scTop
            // 
            this.scTop.LargeChange = 1;
            this.scTop.Location = new System.Drawing.Point(70, 469);
            this.scTop.Maximum = 21;
            this.scTop.Minimum = -21;
            this.scTop.Name = "scTop";
            this.scTop.Size = new System.Drawing.Size(644, 26);
            this.scTop.TabIndex = 1;
            this.scTop.Value = -10;
            // 
            // lbTop
            // 
            this.lbTop.AutoSize = true;
            this.lbTop.Location = new System.Drawing.Point(717, 473);
            this.lbTop.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbTop.Name = "lbTop";
            this.lbTop.Size = new System.Drawing.Size(35, 13);
            this.lbTop.TabIndex = 2;
            this.lbTop.Text = "label1";
            // 
            // lbBottom
            // 
            this.lbBottom.AutoSize = true;
            this.lbBottom.Location = new System.Drawing.Point(717, 495);
            this.lbBottom.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbBottom.Name = "lbBottom";
            this.lbBottom.Size = new System.Drawing.Size(35, 13);
            this.lbBottom.TabIndex = 3;
            this.lbBottom.Text = "label2";
            // 
            // btTrait
            // 
            this.btTrait.Location = new System.Drawing.Point(51, 15);
            this.btTrait.Name = "btTrait";
            this.btTrait.Size = new System.Drawing.Size(23, 23);
            this.btTrait.TabIndex = 4;
            this.btTrait.Text = "T";
            this.btTrait.UseVisualStyleBackColor = true;
            this.btTrait.Click += new System.EventHandler(this.btTrait_Click);
            // 
            // btFocal
            // 
            this.btFocal.Location = new System.Drawing.Point(80, 15);
            this.btFocal.Name = "btFocal";
            this.btFocal.Size = new System.Drawing.Size(23, 23);
            this.btFocal.TabIndex = 5;
            this.btFocal.Text = "F";
            this.btFocal.UseVisualStyleBackColor = true;
            this.btFocal.Click += new System.EventHandler(this.btFocal_Click);
            // 
            // btBond
            // 
            this.btBond.Location = new System.Drawing.Point(109, 15);
            this.btBond.Name = "btBond";
            this.btBond.Size = new System.Drawing.Size(23, 23);
            this.btBond.TabIndex = 6;
            this.btBond.Text = "B";
            this.btBond.UseVisualStyleBackColor = true;
            this.btBond.Click += new System.EventHandler(this.btBond_Click);
            // 
            // btEntity
            // 
            this.btEntity.Location = new System.Drawing.Point(22, 15);
            this.btEntity.Name = "btEntity";
            this.btEntity.Size = new System.Drawing.Size(23, 23);
            this.btEntity.TabIndex = 7;
            this.btEntity.Text = "E";
            this.btEntity.UseVisualStyleBackColor = true;
            this.btEntity.Click += new System.EventHandler(this.btEntity_Click);
            // 
            // SlugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 518);
            this.Controls.Add(this.btEntity);
            this.Controls.Add(this.btBond);
            this.Controls.Add(this.btFocal);
            this.Controls.Add(this.btTrait);
            this.Controls.Add(this.lbBottom);
            this.Controls.Add(this.lbTop);
            this.Controls.Add(this.scTop);
            this.Controls.Add(this.scBottom);
            this.Controls.Add(this.btNext);
            this.Controls.Add(this.slugPanel);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
        private System.Windows.Forms.Button btTrait;
        private System.Windows.Forms.Button btFocal;
        private System.Windows.Forms.Button btBond;
        private System.Windows.Forms.Button btEntity;
    }
}

