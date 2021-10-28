using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vis.Model.Agent;
using Vis.Model.Primitives;

namespace Vis.Model.Controller
{
    public interface IRenderer
    {
        IAgent Agent { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        float UnitPixels { get; set; }
        int PenIndex { get; set; }

        void Invalidate();
        event EventHandler DrawingComplete;
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseUp;
    }

    public abstract class RendererBase
    {
	    public event EventHandler DrawingComplete;
	    public IAgent Agent { get; set; }
	    public int PenIndex { get; set; }
	    private float _unitPixels = 220;
	    public float UnitPixels
	    {
		    get => _unitPixels;
		    set
		    {
			    _unitPixels = value;
                GeneratePens();
            }
	    }

        public abstract void GeneratePens();
	    public abstract void BeginDraw();
	    public abstract void EndDraw();
	    public abstract void Flush();
	    public abstract void DrawSpot(VisPoint pos, int penIndex = 0, float scale = 1f);
	    public abstract void DrawCircle(VisCircle circ, int penIndex = 0);
	    public abstract void DrawRect(VisRectangle rect, int penIndex = 0);
	    public abstract void DrawLine(VisLine line, int penIndex = 0);
	    public abstract void DrawLine(VisPoint p0, VisPoint p1, int penIndex = 0);
	    public abstract void DrawPolyline(VisPoint[] points, int penIndex = 0);


	    protected void OnDrawingComplete()
	    {
		    DrawingComplete?.Invoke(this, EventArgs.Empty);
        }

	    public void Draw()
	    {
		    foreach (var prim in Agent.FocusPad.Paths)
		    {
			    DrawPrimitive(prim, PenIndex);
		    }

		    foreach (var path in Agent.ViewPad.Paths)
		    {
			    DrawPath(path, 1);
		    }

		    foreach (var prim in Agent.WorkingPad.Paths)
		    {
			    DrawPrimitive(prim, PenIndex);
		    }
	    }

	    public void DrawPrimitive(IPrimitive path, int penIndex = 0)
	    {
		    if (path is VisLine line)
		    {
			    DrawLine(line.StartPoint, line.EndPoint, penIndex);
		    }
		    else if (path is VisCircle circ)
		    {
			    DrawSpot(circ.Center, penIndex);
			    DrawCircle(circ, penIndex);
		    }
		    else if (path is VisRectangle rect)
		    {
			    DrawRect(rect, penIndex);
		    }
		    else if (path is RenderPoint rp)
		    {
			    DrawSpot(rp, rp.PenIndex, rp.Scale); // penIndex);
		    }
		    else if (path is VisPoint p)
		    {
			    DrawSpot(p, 6); // penIndex);
		    }
	    }

	    public void DrawShape(VisStroke shape)
	    {
	    }

	    public void DrawPath(VisStroke stroke, int penIndex = 0)
	    {
		    foreach (var segment in stroke.Segments)
		    {
			    if (segment is VisLine line)
			    {
				    DrawLine(line, penIndex);
			    }
			    else if (segment is VisArc arc)
			    {
				    DrawPolyline(arc.GetPolylinePoints(), penIndex);
			    }
		    }

		    Flush();
	    }
    }
}
