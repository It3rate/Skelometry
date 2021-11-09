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
	    IRenderer _renderer;
	    Control _control;
        public VisForm()
        {
            InitializeComponent();
            DoubleBuffered = true;

            //_renderer = new VisRenderer(panel);
            _renderer = new SkiaRenderer();
            _control = _renderer.AddAsControl(panel, false);
            _agent = new VisAgent(_renderer);
            Redraw();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
	        base.OnVisibleChanged(e);
	        Redraw();
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
		        Redraw();
	        }
        }

        private void slLayout_Scroll(object sender, ScrollEventArgs e)
        {
	        if (_agent != null)
	        {
		        _agent.Skills.bTopCenter = e.NewValue / 100f;
		        lbVariationB.Text = "Value: " + e.NewValue;
		        Redraw();
            }

        }

        private void Redraw()
        {
	        _agent.Draw();
            //_renderer.Agent = _agent;
            _control.Invalidate();
        }
    }
}
