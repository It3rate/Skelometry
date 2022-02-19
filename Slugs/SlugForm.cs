using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Slugs.Entities;
using Slugs.Renderer;
using Slugs.UI;

namespace Slugs
{
    public partial class SlugForm : Form
    {
	    private readonly SlugRenderer _renderer;
	    private readonly Control _control;

	    private readonly Agents.Agent _agent;

        public SlugForm()
        {
			DoubleBuffered = true;
            InitializeComponent();

            _renderer = new SlugRenderer();
            _control = _renderer.AddAsControl(slugPanel, false);
            _control.MouseDown += OnMouseDown;
            _control.MouseMove += OnMouseMove;
            _control.MouseUp += OnMouseUp;
            KeyDown += OnKeyDown;
            //KeyPress += OnKeyPress;
            KeyUp += OnKeyUp;
            KeyPreview = true;

            _agent = new Agents.Agent(_renderer);
            scTop.Value = (int)_agent.ScrollLeft;
            lbTop.Text = (scTop.Value).ToString();
            scBottom.Value = (int)_agent.ScrollRight;
            lbBottom.Text = (scBottom.Value).ToString();
            scTop.Scroll += ScTop_Scroll;
            scBottom.Scroll += ScBottom_Scroll;

            _agent.OnModeChange += _agent_OnModeChange;
            _agent.OnDisplayModeChange += _agent_OnDisplayModeChange;
            _agent.OnSelectionChange += _agent_OnSelectionChange;
        }

        private void ScTop_Scroll(object sender, ScrollEventArgs e)
        {
	        _agent.ScrollLeft = scTop.Value;
	        lbTop.Text = (scTop.Value).ToString();
	        Redraw();
        }
        private void ScBottom_Scroll(object sender, ScrollEventArgs e)
        {
	        _agent.ScrollRight = scBottom.Value;
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
	        //_renderer.Agent = Agent;
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
        private void btEntity_Click(object sender, EventArgs e)
        {
	        _agent.UIMode = UIMode.CreateEntity;
        }
        private void btTrait_Click(object sender, EventArgs e)
        {
	        _agent.UIMode = UIMode.CreateTrait;
        }
        private void btFocal_Click(object sender, EventArgs e)
        {
	        _agent.UIMode = UIMode.CreateFocal;
        }
        private void btDoubleBond_Click(object sender, EventArgs e)
        {
	        _agent.UIMode = UIMode.CreateDoubleBond;
        }
        private void btBond_Click(object sender, EventArgs e)
        {
	        _agent.UIMode = UIMode.CreateBond;
        }
        private void btUnit_Click(object sender, EventArgs e)
        {
	        _agent.UIMode = UIMode.SetUnit;
        }
        private void btEqual_Click(object sender, EventArgs e)
        {
	        _agent.UIMode = UIMode.Equal;
        }
        private void _agent_OnModeChange(object sender, EventArgs e)
        {
            btEntity.Enabled = true;
	        btTrait.Enabled = true;
	        btFocal.Enabled = true;
	        btDoubleBond.Enabled = true;
	        btBond.Enabled = true;
	        btUnit.Enabled = true;
	        btEqual.Enabled = true;
            switch (_agent.UIMode)
            {
	            case UIMode.CreateTrait:
		            btTrait.Enabled = false;
		            break;
	            case UIMode.CreateFocal:
                    btFocal.Enabled = false;
                    break;
	            case UIMode.CreateDoubleBond:
		            btDoubleBond.Enabled = false;
		            break;
	            case UIMode.CreateBond:
		            btBond.Enabled = false;
		            break;
	            case UIMode.SetUnit:
		            btUnit.Enabled = false;
		            break;
	            case UIMode.Equal:
		            btEqual.Enabled = false;
		            break;
                default:
                    btEntity.Enabled = false;
                    break;
            }
            _control.Select();
        }
        private void _agent_OnDisplayModeChange(object sender, EventArgs e)
        {
            // todo: Use icons for toggle button visual states.
            //var dm = _agent.DisplayMode;
            //btInformation.BackgroundImage = ...;
        }

        private void btInformation_Click(object sender, EventArgs e)
        {
	        _agent.ToggleShowNumbers();
	        Redraw();
        }

        private void _agent_OnSelectionChange(object sender, EventArgs e)
        {
	        if (_agent.Data.Selected.FirstElement is Focal focal)
	        {
		        var slug = focal.Slug;
                lbValue.Text = slug.Img.ToString("0.###") + " : " + slug.Real.ToString("0.###") + " len:" + (focal.LengthT).ToString("0.###");
	        }
        }

    }
}
