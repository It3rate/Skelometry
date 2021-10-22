using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Primitives;

namespace Vis.Model.Agent
{
    public class VisMeasureSkills
    {
        public void Line(VisPad<VisPoint> focusPad, VisPad<VisStroke> viewPad, float x, float y)
        {
            //var letterbox = new Rectangle(0.5f, 0.5f, 0f, 0f);
            //focusPad.Paths.Add(letterbox);

            var p = new VisPoint(x, y);
            focusPad.Paths.Add(p);
        }
    }
}
