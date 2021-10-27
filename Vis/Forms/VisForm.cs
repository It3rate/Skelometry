using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vis.Model.Agent;
using Vis.Model.Controller;

namespace Vis.Forms
{
    public partial class VisForm : Form
    {
	    VisAgent _agent;
        VisRenderer _renderer;
        Panel _panel;
        public VisForm()
        {
            DoubleBuffered = true;
            InitializeComponent();

            _panel = panel1;
            _renderer = new VisRenderer();
            _agent = new VisAgent(_renderer);
        }

        public void OnDraw(Graphics g)
        {
            _renderer.SetGraphicsContext(g);
            _agent.Draw();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
	        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
	        OnDraw(e.Graphics);
        }

        private void btNext_Click(object sender, EventArgs e)
        {
	        Program.NextForm();
        }
        private void _formClosed(object sender, FormClosedEventArgs e)
        {
	        Application.Exit();
        }

        private void slColor_Scroll(object sender, ScrollEventArgs e)
        {
	        if (_agent != null)
	        {
		        _agent.Skills.rTailStart = e.NewValue / 100f;
		        lbVariationA.Text = "Value: " + e.NewValue;
                _panel.Invalidate();
	        }
        }

        private void slLayout_Scroll(object sender, ScrollEventArgs e)
        {
	        if (_agent != null)
	        {
		        _agent.Skills.bTopCenter = e.NewValue / 100f;
		        lbVariationB.Text = "Value: " + e.NewValue;
                _panel.Invalidate();
	        }

        }
    }
}
