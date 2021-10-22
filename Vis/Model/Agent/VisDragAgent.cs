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
        public VisPad<VisPoint> WorkingPad { get; private set; }
        public VisPad<VisPoint> FocusPad { get; private set; }
        public VisPad<VisStroke> ViewPad { get; private set; }
        private VisRenderer _renderer;
        private VisMeasureSkills _skills;

        public int _unitPixels = 220;

        public VisDragAgent(VisRenderer renderer)
        {
            _renderer = renderer;

            _skills = new VisMeasureSkills();
            WorkingPad = new VisPad<VisPoint>(_renderer.Width, _renderer.Height);
            FocusPad = new VisPad<VisPoint>(_renderer.Width, _renderer.Height);
            ViewPad = new VisPad<VisStroke>(_renderer.Width, _renderer.Height);
        }

        bool _isDown = false;
        bool _isHighlighting = false;
        VisPoint _startPoint;
        public bool MouseDown(MouseEventArgs e)
        {
            _isDown = true;
            _startPoint = new VisPoint(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);
            _skills.Point(this, _startPoint);
            return true;
        }

        public bool MouseMove(MouseEventArgs e)
        {
            var result = false;
            var p = new VisPoint(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);
            if (_isDown)
            {
                _skills.Line(this, _startPoint, p);
                result = true; 
            }
            else
            {
                var similarPt = ViewPad.GetSimilar(p);
                if(similarPt is VisPoint sp)
                {
                    var rp = new RenderPoint(sp, 7, 2f);
                    _skills.Point(this, rp);
                    _isHighlighting = true;
                    result = true;
                }

                if(!result && _isHighlighting)
                {
                    _isHighlighting = false;
                    result = true;
                }
            }
            return result;
        }

        public bool MouseUp(MouseEventArgs e)
        {
            _isDown = false;
            var endPoint = new VisPoint(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);
            _skills.Line(this, _startPoint, endPoint, true);
            _startPoint = null;
            return true;
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {

        }

        public void Clear()
        {
            WorkingPad.Clear();
            //FocusPad.Clear();
            //ViewPad.Clear();
        }

        public void Draw(Graphics g)
        {
            var state = g.Save();
            //g.TranslateTransform(10, 10);
            g.ScaleTransform(_unitPixels, _unitPixels);
            int penIndex = _isDown ? 3 : 2;
            _renderer.Draw(g, this, penIndex);
            g.Restore(state);

            Clear();
        }



    }
}
