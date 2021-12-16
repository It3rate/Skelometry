﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Slugs.Agent;
using Slugs.Renderer;

namespace Slugs
{
    public partial class SlugForm : Form
    {
	    private SlugRenderer _renderer;
	    private Control _control;

	    private IAgent _agent;

        public SlugForm()
        {
			DoubleBuffered = true;
            InitializeComponent();

            _renderer = new SlugRenderer();
            _control = _renderer.AddAsControl(slugPanel, false);
            _control.MouseDown += OnMouseDown;
            _control.MouseMove += OnMouseMove;
            _control.MouseUp += OnMouseUp;
            _control.KeyDown += OnKeyDown;
            _control.KeyUp += OnKeyUp;

            _agent = new SlugAgent(_renderer);
            scTop.Value = (int)_renderer.unitPull;
            lbTop.Text = (scTop.Value).ToString();
            scBottom.Value = (int)_renderer.unitPush;
            lbBottom.Text = (scBottom.Value).ToString();
            scTop.Scroll += ScTop_Scroll;
            scBottom.Scroll += ScBottom_Scroll;
        }


        private void ScTop_Scroll(object sender, ScrollEventArgs e)
        {
	        _renderer.unitPull = scTop.Value;
	        lbTop.Text = (scTop.Value).ToString();
	        Redraw();
        }
        private void ScBottom_Scroll(object sender, ScrollEventArgs e)
        {
	        _renderer.unitPush = scBottom.Value;
	        lbBottom.Text = (scBottom.Value).ToString();
            Redraw();
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

    }
}
