using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Primitives;

namespace Vis.Model.Agent
{
    public interface IAgent
    {
        VisPad<VisPoint> WorkingPad { get; }
        VisPad<VisPoint> FocusPad { get; }
        VisPad<VisStroke> ViewPad { get; }

        void Clear();
        void Draw(Graphics g);
    }
}
