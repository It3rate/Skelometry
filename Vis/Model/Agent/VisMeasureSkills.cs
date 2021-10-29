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
        public void Point(IAgent agent, VisPoint p, bool permanent = false)
        {
            var pad = permanent ? agent.FocusPad : agent.WorkingPad;
            pad.Paths.Add(p);
        }
        public IPath Line(IAgent agent, VisPoint start, VisPoint end, bool permanent = false)
        {
	        IPath result = null;
            VisLine line = VisLine.ByEndpoints(start, end);
	        if (permanent)
	        {
		        agent.FocusPad.Paths.Add(line);
		        var nodeStart = new VisNode(line, 0);
		        var nodeEnd = new VisNode(line, 1);
		        var stroke = new VisStroke(nodeStart, nodeEnd);
                agent.ViewPad.Paths.Add(stroke);
		        result = stroke;
            }
	        else
	        {
		        var pad = agent.WorkingPad;
		        pad.Paths.Add(line);
		        pad.Paths.Add(start);
		        pad.Paths.Add(end);
	        }
	        return result;
        }
        public IPath Circle(IAgent agent, VisPoint start, VisPoint end, bool permanent = false)
        {
	        IPath result = null;
	        var circ = new VisCircle(start, end);
	        if (permanent)
	        {
		        agent.FocusPad.Paths.Add(circ);
                //agent.ViewPad.Paths.Add(new VisStroke());
                var tn = new TangentNode(circ, ClockDirection.CW);
                var stroke = new VisStroke(circ.StartNode, tn, circ.StartNode);
                agent.ViewPad.Paths.Add(stroke);
                result = stroke;
	        }
            else
	        {
		        var line = VisLine.ByEndpoints(start, end);
                var pad = agent.WorkingPad;
                pad.Paths.Add(circ);
                pad.Paths.Add(line);
                pad.Paths.Add(start);
		        pad.Paths.Add(end);
	        }

	        return result;
        }
    }
}
