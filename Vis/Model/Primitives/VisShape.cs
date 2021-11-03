using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Connections;

namespace Vis.Model.Primitives
{
    public class VisShape
    {
        public float Length => 0;
        public VisPoint Anchor { get; } = null;

        public List<VisStroke> Strokes { get; } = new List<VisStroke>();

        // computed
        public List<VisJoint> ComputedJoints { get; } = new List<VisJoint>();

        public float IsInside(IPath element) => 0;

        public VisShape(params VisStroke[] strokes)
        {
            Strokes.AddRange(strokes);
        }
    }
}
