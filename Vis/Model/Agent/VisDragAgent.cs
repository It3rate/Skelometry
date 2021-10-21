using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vis.Model
{
    public class VisDragAgent
    {
        public VisPad<Point> FocusPad { get; private set; }
        public VisPad<Stroke> ViewPad { get; private set; }
        private VisRenderer _renderer;

        public VisDragAgent(VisRenderer renderer)
        {
            _renderer = renderer;

            //Skills = new VisSkills();
            FocusPad = new VisPad<Point>(250, 250);
            ViewPad = new VisPad<Stroke>(250, 250);
        }

        public void Clear()
        {
            FocusPad.Clear();
            ViewPad.Clear();
        }

        public int _unitPixels = 220;
        public void Draw(Graphics g)
        {
            var state = g.Save();
            g.Restore(state);
        }



    }
}
