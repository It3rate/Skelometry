using SkiaSharp;
using SkiaSharp.Views.Desktop;
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
    public partial class VisDragForm : Form
    {
        VisDragAgent _agent;
        IRenderer _renderer;

        public VisDragForm()
        {
            DoubleBuffered = true;
            InitializeComponent();

            //_renderer = (IRenderer)visPanel;// new VisRenderer();
            var skia = new SkiaRenderer(visPanel);
            visPanel.MouseDown -= OnMouseDown;
            visPanel.MouseMove -= OnMouseMove;
            visPanel.MouseUp -= OnMouseUp;
            skia.MouseDown += OnMouseDown;
            skia.MouseMove += OnMouseMove;
            skia.MouseUp += OnMouseUp;
            _renderer = skia;// new SkiaRenderer(visPanel);
            _agent = new VisDragAgent(_renderer);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
	        if (_agent.MouseDown(e))
	        {
		        Redraw();
	        }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
	        if (_agent.MouseMove(e))
	        {
		        Redraw();
	        }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
	        if (_agent.MouseUp(e))
	        {
		        Redraw();
	        }
        }

        private void Redraw()
        {
	        _renderer.Agent = _agent;
	        _renderer.Invalidate();
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            Program.NextForm();
        }
        private void _formClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void VisDragForm_KeyDown(object sender, KeyEventArgs e)
        {

        }

    }
}
