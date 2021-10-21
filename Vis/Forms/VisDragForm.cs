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
using Vis.Model;

namespace Vis.Forms
{
    public partial class VisDragForm : Form
    {
        VisDragAgent _agent;
        VisRenderer _renderer;
        public VisDragForm()
        {
            DoubleBuffered = true;
            InitializeComponent();

            _renderer = new VisRenderer(panel1.Width, panel1.Height);
            _agent = new VisDragAgent(_renderer);
        }

        public void OnDraw(Graphics g)
        {
            _agent.Draw(g);
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

    }
}
