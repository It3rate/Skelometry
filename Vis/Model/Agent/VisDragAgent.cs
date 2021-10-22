using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vis.Model.Controller;
using Vis.Model.Primitives;

namespace Vis.Model.Agent
{
    public class VisDragAgent : IAgent
    {
        public VisPad<VisPoint> FocusPad { get; private set; }
        public VisPad<VisStroke> ViewPad { get; private set; }
        private VisRenderer _renderer;
        private VisMeasureSkills _skills;

        public int _unitPixels = 220;

        public VisDragAgent(VisRenderer renderer)
        {
            _renderer = renderer;

            _skills = new VisMeasureSkills();
            FocusPad = new VisPad<VisPoint>(_renderer.Width, _renderer.Height);
            ViewPad = new VisPad<VisStroke>(_renderer.Width, _renderer.Height);
        }

        bool _isDown = false;
        public void MouseDown(MouseEventArgs e)
        {
            _isDown = true;
            _skills.Line(FocusPad, ViewPad, e.X / (float)_unitPixels, e.Y / (float)_unitPixels);
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {

            }
        }

        public void MouseUp(object sender, MouseEventArgs e)
        {
            _isDown = false;
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {

        }

        public void Clear()
        {
            FocusPad.Clear();
            ViewPad.Clear();
        }

        public void Draw(Graphics g)
        {
            var state = g.Save();
            //g.TranslateTransform(10, 10);
            g.ScaleTransform(_unitPixels, _unitPixels);
            _renderer.Draw(g, this);
            g.Restore(state);
        }



    }
}
