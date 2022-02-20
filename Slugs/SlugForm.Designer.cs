
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
            this.btDoubleBond = new System.Windows.Forms.Button();
            this.btEntity = new System.Windows.Forms.Button();
            this.btBond = new System.Windows.Forms.Button();
            this.btUnit = new System.Windows.Forms.Button();
            this.btEqual = new System.Windows.Forms.Button();
            this.lbValue = new System.Windows.Forms.Label();
            this.btInformation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // slugPanel
            // 
            this.slugPanel.Location = new System.Drawing.Point(26, 82);
            this.slugPanel.Name = "slugPanel";
            this.slugPanel.Size = new System.Drawing.Size(1482, 892);
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
            this.scBottom.Location = new System.Drawing.Point(230, 1033);
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
            this.scTop.Location = new System.Drawing.Point(230, 1000);
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
            this.lbTop.Location = new System.Drawing.Point(1201, 1006);
            this.lbTop.Name = "lbTop";
            this.lbTop.Size = new System.Drawing.Size(51, 20);
            this.lbTop.TabIndex = 2;
            this.lbTop.Text = "label1";
            // 
            // lbBottom
            // 
            this.lbBottom.AutoSize = true;
            this.lbBottom.Location = new System.Drawing.Point(1201, 1040);
            this.lbBottom.Name = "lbBottom";
            this.lbBottom.Size = new System.Drawing.Size(51, 20);
            this.lbBottom.TabIndex = 3;
            this.lbBottom.Text = "label2";
            // 
            // btTrait
            // 
            this.btTrait.Location = new System.Drawing.Point(76, 23);
            this.btTrait.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btTrait.Name = "btTrait";
            this.btTrait.Size = new System.Drawing.Size(34, 35);
            this.btTrait.TabIndex = 4;
            this.btTrait.Text = "T";
            this.btTrait.UseVisualStyleBackColor = true;
            this.btTrait.Click += new System.EventHandler(this.btTrait_Click);
            // 
            // btFocal
            // 
            this.btFocal.Location = new System.Drawing.Point(120, 23);
            this.btFocal.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btFocal.Name = "btFocal";
            this.btFocal.Size = new System.Drawing.Size(34, 35);
            this.btFocal.TabIndex = 5;
            this.btFocal.Text = "F";
            this.btFocal.UseVisualStyleBackColor = true;
            this.btFocal.Click += new System.EventHandler(this.btFocal_Click);
            // 
            // btDoubleBond
            // 
            this.btDoubleBond.Location = new System.Drawing.Point(164, 23);
            this.btDoubleBond.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btDoubleBond.Name = "btDoubleBond";
            this.btDoubleBond.Size = new System.Drawing.Size(34, 35);
            this.btDoubleBond.TabIndex = 6;
            this.btDoubleBond.Text = "D";
            this.btDoubleBond.UseVisualStyleBackColor = true;
            this.btDoubleBond.Click += new System.EventHandler(this.btDoubleBond_Click);
            // 
            // btEntity
            // 
            this.btEntity.Location = new System.Drawing.Point(33, 23);
            this.btEntity.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btEntity.Name = "btEntity";
            this.btEntity.Size = new System.Drawing.Size(34, 35);
            this.btEntity.TabIndex = 7;
            this.btEntity.Text = "E";
            this.btEntity.UseVisualStyleBackColor = true;
            this.btEntity.Click += new System.EventHandler(this.btEntity_Click);
            // 
            // btBond
            // 
            this.btBond.Location = new System.Drawing.Point(206, 23);
            this.btBond.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btBond.Name = "btBond";
            this.btBond.Size = new System.Drawing.Size(34, 35);
            this.btBond.TabIndex = 8;
            this.btBond.Text = "B";
            this.btBond.UseVisualStyleBackColor = true;
            this.btBond.Click += new System.EventHandler(this.btBond_Click);
            // 
            // btUnit
            // 
            this.btUnit.Location = new System.Drawing.Point(287, 23);
            this.btUnit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btUnit.Name = "btUnit";
            this.btUnit.Size = new System.Drawing.Size(34, 35);
            this.btUnit.TabIndex = 9;
            this.btUnit.Text = "U";
            this.btUnit.UseVisualStyleBackColor = true;
            this.btUnit.Click += new System.EventHandler(this.btUnit_Click);
            // 
            // btEqual
            // 
            this.btEqual.Location = new System.Drawing.Point(329, 23);
            this.btEqual.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btEqual.Name = "btEqual";
            this.btEqual.Size = new System.Drawing.Size(34, 35);
            this.btEqual.TabIndex = 10;
            this.btEqual.Text = "=";
            this.btEqual.UseVisualStyleBackColor = true;
            this.btEqual.Click += new System.EventHandler(this.btEqual_Click);
            // 
            // lbValue
            // 
            this.lbValue.AutoSize = true;
            this.lbValue.Location = new System.Drawing.Point(756, 38);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(18, 20);
            this.lbValue.TabIndex = 11;
            this.lbValue.Text = "0";
            // 
            // btInformation
            // 
            this.btInformation.Location = new System.Drawing.Point(526, 23);
            this.btInformation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btInformation.Name = "btInformation";
            this.btInformation.Size = new System.Drawing.Size(34, 35);
            this.btInformation.TabIndex = 9;
            this.btInformation.Text = "i";
            this.btInformation.UseVisualStyleBackColor = true;
            this.btInformation.Click += new System.EventHandler(this.btInformation_Click);
            // 
            // SlugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1538, 1071);
            this.Controls.Add(this.lbValue);
            this.Controls.Add(this.btEqual);
            this.Controls.Add(this.btInformation);
            this.Controls.Add(this.btUnit);
            this.Controls.Add(this.btBond);
            this.Controls.Add(this.btEntity);
            this.Controls.Add(this.btDoubleBond);
            this.Controls.Add(this.btFocal);
            this.Controls.Add(this.btTrait);
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
        private System.Windows.Forms.Button btTrait;
        private System.Windows.Forms.Button btFocal;
        private System.Windows.Forms.Button btDoubleBond;
        private System.Windows.Forms.Button btEntity;
        private System.Windows.Forms.Button btBond;
        private System.Windows.Forms.Button btUnit;
        private System.Windows.Forms.Button btEqual;
        private System.Windows.Forms.Label lbValue;
        private System.Windows.Forms.Button btInformation;
    }
}

