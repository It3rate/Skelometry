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
        Panel _panel;
        public VisDragForm()
        {
            DoubleBuffered = true;
            InitializeComponent();

            _panel = visPanel;
            _renderer = (IRenderer)_panel;// new VisRenderer();
            //_renderer = new SkiaRenderer(panel1, panel1.Width/2, panel1.Height);
            _agent = new VisDragAgent(_renderer);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            _renderer.SetGraphicsContext(e.Graphics);
            _agent.Draw();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_agent.MouseDown(e))
            {
                _renderer.Invalidate();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_agent.MouseMove(e))
            {
                _renderer.Invalidate();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_agent.MouseUp(e))
            {
                _renderer.Invalidate();
            }
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
