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
        private VisDragAgent _agent;
        private SkiaRenderer _renderer;
        private Control _control;

        public VisDragForm()
        {
            DoubleBuffered = true;
            InitializeComponent();

            //_renderer = (IRenderer)visPanel;// new VisRenderer();
            var skia = new SkiaRenderer();
            _control = skia.AddAsControl(visPanel, false);
            _control.MouseDown += OnMouseDown;
            _control.MouseDown += OnMouseDown;
            _control.MouseMove += OnMouseMove;
            _control.MouseUp += OnMouseUp;
            _control.KeyDown += OnKeyDown;
            _control.KeyUp += OnKeyUp;
            
            _renderer = skia;
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

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
	        if (_agent.KeyDown(e))
	        {
		        Redraw();
	        }
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
	        if (_agent.KeyUp(e))
	        {
		        Redraw();
	        }
        }

        private void Redraw()
        {
	        //_renderer.Agent = _agent;
	        _control.Invalidate();
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
