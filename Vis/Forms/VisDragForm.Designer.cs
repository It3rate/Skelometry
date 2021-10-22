
namespace Vis.Forms
{
    partial class VisDragForm
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
            this.lbVariationB = new System.Windows.Forms.Label();
            this.lbVariationA = new System.Windows.Forms.Label();
            this.slLayout = new System.Windows.Forms.HScrollBar();
            this.slColor = new System.Windows.Forms.HScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btNext = new System.Windows.Forms.Button();
            this.lbTitleX = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbVariationB
            // 
            this.lbVariationB.AutoSize = true;
            this.lbVariationB.Location = new System.Drawing.Point(542, 374);
            this.lbVariationB.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbVariationB.Name = "lbVariationB";
            this.lbVariationB.Size = new System.Drawing.Size(58, 13);
            this.lbVariationB.TabIndex = 23;
            this.lbVariationB.Text = "Variation B";
            // 
            // lbVariationA
            // 
            this.lbVariationA.AutoSize = true;
            this.lbVariationA.Location = new System.Drawing.Point(9, 374);
            this.lbVariationA.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbVariationA.Name = "lbVariationA";
            this.lbVariationA.Size = new System.Drawing.Size(58, 13);
            this.lbVariationA.TabIndex = 22;
            this.lbVariationA.Text = "Variation A";
            // 
            // slLayout
            // 
            this.slLayout.Location = new System.Drawing.Point(528, 389);
            this.slLayout.Maximum = 120;
            this.slLayout.Minimum = 40;
            this.slLayout.Name = "slLayout";
            this.slLayout.Size = new System.Drawing.Size(219, 26);
            this.slLayout.TabIndex = 21;
            this.slLayout.Value = 90;
            // 
            // slColor
            // 
            this.slColor.Location = new System.Drawing.Point(7, 389);
            this.slColor.Maximum = 106;
            this.slColor.Minimum = 1;
            this.slColor.Name = "slColor";
            this.slColor.Size = new System.Drawing.Size(219, 26);
            this.slColor.TabIndex = 20;
            this.slColor.Value = 50;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panel1.Location = new System.Drawing.Point(1, 30);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(750, 334);
            this.panel1.TabIndex = 19;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // btNext
            // 
            this.btNext.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btNext.Location = new System.Drawing.Point(690, 3);
            this.btNext.Margin = new System.Windows.Forms.Padding(2);
            this.btNext.Name = "btNext";
            this.btNext.Size = new System.Drawing.Size(50, 23);
            this.btNext.TabIndex = 18;
            this.btNext.TabStop = false;
            this.btNext.Text = "Next";
            this.btNext.UseVisualStyleBackColor = true;
            this.btNext.Click += new System.EventHandler(this.btNext_Click);
            // 
            // lbTitleX
            // 
            this.lbTitleX.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lbTitleX.Font = new System.Drawing.Font("Berlin Sans FB", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitleX.Location = new System.Drawing.Point(245, 366);
            this.lbTitleX.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbTitleX.Name = "lbTitleX";
            this.lbTitleX.Size = new System.Drawing.Size(267, 36);
            this.lbTitleX.TabIndex = 17;
            this.lbTitleX.Text = "VIS DRAG";
            this.lbTitleX.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // VisDragForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 419);
            this.Controls.Add(this.lbVariationB);
            this.Controls.Add(this.lbVariationA);
            this.Controls.Add(this.slLayout);
            this.Controls.Add(this.slColor);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btNext);
            this.Controls.Add(this.lbTitleX);
            this.Name = "VisDragForm";
            this.Text = "VisDragForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this._formClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VisDragForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbVariationB;
        private System.Windows.Forms.Label lbVariationA;
        private System.Windows.Forms.HScrollBar slLayout;
        private System.Windows.Forms.HScrollBar slColor;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btNext;
        private System.Windows.Forms.Label lbTitleX;
    }
}