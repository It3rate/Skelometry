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
        public void Point(IAgent agent, VisPoint p)
        {
            agent.FocusPad.Paths.Add(p);
        }
        public void Line(IAgent agent, VisPoint start, VisPoint end, bool permanent = false)
        {
            var pad = permanent ? agent.FocusPad : agent.WorkingPad;
            VisLine line = VisLine.ByEndpoints(start, end);
            pad.Paths.Add(line);
            pad.Paths.Add(start);
            pad.Paths.Add(end);
        }
    }
}
