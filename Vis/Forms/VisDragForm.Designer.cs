﻿
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
            this.components = new System.ComponentModel.Container();
            this.lbVariationB = new System.Windows.Forms.Label();
            this.lbVariationA = new System.Windows.Forms.Label();
            this.slLayout = new System.Windows.Forms.HScrollBar();
            this.slColor = new System.Windows.Forms.HScrollBar();
            this.btNext = new System.Windows.Forms.Button();
            this.lbTitleX = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.visPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // lbVariationB
            // 
            this.lbVariationB.AutoSize = true;
            this.lbVariationB.Location = new System.Drawing.Point(813, 575);
            this.lbVariationB.Name = "lbVariationB";
            this.lbVariationB.Size = new System.Drawing.Size(87, 20);
            this.lbVariationB.TabIndex = 23;
            this.lbVariationB.Text = "Variation B";
            // 
            // lbVariationA
            // 
            this.lbVariationA.AutoSize = true;
            this.lbVariationA.Location = new System.Drawing.Point(14, 575);
            this.lbVariationA.Name = "lbVariationA";
            this.lbVariationA.Size = new System.Drawing.Size(87, 20);
            this.lbVariationA.TabIndex = 22;
            this.lbVariationA.Text = "Variation A";
            // 
            // slLayout
            // 
            this.slLayout.Location = new System.Drawing.Point(792, 598);
            this.slLayout.Maximum = 120;
            this.slLayout.Minimum = 40;
            this.slLayout.Name = "slLayout";
            this.slLayout.Size = new System.Drawing.Size(328, 26);
            this.slLayout.TabIndex = 21;
            this.slLayout.Value = 90;
            // 
            // slColor
            // 
            this.slColor.Location = new System.Drawing.Point(10, 598);
            this.slColor.Maximum = 106;
            this.slColor.Minimum = 1;
            this.slColor.Name = "slColor";
            this.slColor.Size = new System.Drawing.Size(328, 26);
            this.slColor.TabIndex = 20;
            this.slColor.Value = 50;
            // 
            // btNext
            // 
            this.btNext.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btNext.Location = new System.Drawing.Point(1035, 5);
            this.btNext.Name = "btNext";
            this.btNext.Size = new System.Drawing.Size(75, 35);
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
            this.lbTitleX.Location = new System.Drawing.Point(368, 563);
            this.lbTitleX.Name = "lbTitleX";
            this.lbTitleX.Size = new System.Drawing.Size(400, 55);
            this.lbTitleX.TabIndex = 17;
            this.lbTitleX.Text = "VIS DRAG";
            this.lbTitleX.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // visPanel
            // 
            this.visPanel.Location = new System.Drawing.Point(37, 52);
            this.visPanel.Name = "visPanel";
            this.visPanel.Size = new System.Drawing.Size(1036, 508);
            this.visPanel.TabIndex = 24;
            // 
            // VisDragForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 645);
            this.Controls.Add(this.visPanel);
            this.Controls.Add(this.lbVariationB);
            this.Controls.Add(this.lbVariationA);
            this.Controls.Add(this.slLayout);
            this.Controls.Add(this.slColor);
            this.Controls.Add(this.btNext);
            this.Controls.Add(this.lbTitleX);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "VisDragForm";
            this.Text = "VisDragForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this._formClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VisDragForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbVariationB;
        private System.Windows.Forms.Label lbVariationA;
        private System.Windows.Forms.HScrollBar slLayout;
        private System.Windows.Forms.HScrollBar slColor;
        private System.Windows.Forms.Button btNext;
        private System.Windows.Forms.Label lbTitleX;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Panel visPanel;
    }
}