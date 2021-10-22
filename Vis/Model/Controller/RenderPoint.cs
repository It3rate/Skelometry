using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Primitives;

namespace Vis.Model.Controller
{
    public class RenderPoint : VisPoint
    {
        public int PenIndex { get; set; }
        public float Scale { get; set; }
        public RenderPoint(VisPoint p, int penIndex, float scale) : base(p)
        {
            PenIndex = penIndex;
            Scale = scale;
        }
    }
}
