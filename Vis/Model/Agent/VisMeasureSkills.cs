using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Connections;
using Vis.Model.Controller;
using Vis.Model.Primitives;

namespace Vis.Model.Agent
{
    public class VisMeasureSkills
    {
        public IPath[] AddElements(UIMode mode, IAgent agent, VisPoint start, VisPoint end, bool permanent = false)
        {
            List<IPath> result = new List<IPath>();
            switch(mode)
            {
	            case UIMode.Line:
		            result.Add(Line(agent, start, end, permanent));
		            break;
	            case UIMode.Circle:
		            result.Add(Circle(agent, start, end, permanent));
		            break;
	            case UIMode.ParallelLines:
		            int lineCount = 4;
		            float midX = start.X + (end.X - start.X) / 2f;
		            float step = -0.08f;
                    for (int i = 0; i < lineCount; i++)
                    {
	                    var offset = step * i;
						var centerLine = Line(agent, new VisPoint(midX, start.Y + offset), new VisPoint(midX, start.Y + offset + step), permanent, true);
	                    result.Add(centerLine);
	                    var line = Line(agent, new VisPoint(start.X, start.Y + offset), new VisPoint(end.X, start.Y + offset), permanent, true);
	                    result.Add(line);
                    }
                    break;
            }

            return result.ToArray();
        }

        public void Point(IAgent agent, VisPoint p, bool permanent = false)
        {
            var pad = permanent ? agent.FocusPad : agent.WorkingPad;
            pad.Add(p);
        }


        public IPath Line(IAgent agent, VisPoint start, VisPoint end, bool permanent = false, bool focusOnly = false)
        {
	        IPath result = null;
            VisLine line = VisLine.ByEndpoints(start, end);
	        if (permanent)
	        {
		        agent.FocusPad.Add(line);
		        if (!focusOnly && (agent.Status.State & UIState.ViewPad) != 0)
		        {
			        var nodeStart = new VisNode(line, 0);
			        var nodeEnd = new VisNode(line, 1);
			        var stroke = new VisStroke(nodeStart, nodeEnd);
			        agent.ViewPad.Add(stroke);
			        result = stroke;
		        }
		        else
		        {
			        result = line;
		        }
	        }
	        else
	        {
		        var pad = agent.WorkingPad;
		        pad.Add(line);
		        pad.Add(start);
		        pad.Add(end);
		        result = line;
	        }
	        return result;
        }
        public IPath Circle(IAgent agent, VisPoint start, VisPoint end, bool permanent = false, bool focusOnly = false)
        {
	        IPath result = null;
	        var circ = new VisCircle(start, end);
	        if (permanent)
	        {
		        agent.FocusPad.Add(circ);
                //agent.ViewPad.Paths.Add(new VisStroke());
                if (!focusOnly && (agent.Status.State & UIState.ViewPad) != 0)
                {
	                var tn = new TangentNode(circ, ClockDirection.CW);
	                var stroke = new VisStroke(circ.StartNode, tn, circ.StartNode);
	                agent.ViewPad.Add(stroke);
	                result = stroke;
                }
	        }
            else
	        {
		        var line = VisLine.ByEndpoints(start, end);
                var pad = agent.WorkingPad;
                pad.Add(circ);
                pad.Add(line);
                pad.Add(start);
		        pad.Add(end);
		        result = circ;
	        }

	        return result;
        }
        public VisPolyline Polyline(IAgent agent, VisPolyline polyline, bool permanent = false)
        {
	        VisPolyline result = null;
	        if (permanent)
	        {
		        //agent.FocusPad.Add(line);
		        //var nodeStart = new VisNode(line, 0);
		        //var nodeEnd = new VisNode(line, 1);
		        //var stroke = new VisStroke(nodeStart, nodeEnd);
		        //agent.ViewPad.Add(stroke);
		        //result = stroke;
	        }
	        else
	        {
		        var pad = agent.WorkingPad;
		        pad.Add(polyline);
		        result = polyline;
	        }
	        return result;
        }
    }
}
