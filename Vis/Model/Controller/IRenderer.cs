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

    public abstract class RendererBase : IRenderer
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

	    protected readonly Control Control;

	    protected RendererBase(Control parent, int width = -1, int height = -1)
	    {
		    Control = CreateControl();
		    Control.Width = width == -1 ? parent.Width : width;
		    Control.Height = height == -1 ? parent.Height : height;
		    parent.Controls.Add(Control);

            GeneratePens();
	    }

	    protected abstract Control CreateControl();

        public abstract void GeneratePens();
	    public abstract void BeginDraw();
	    public abstract void EndDraw();
	    public abstract void Flush();
	    public abstract void DrawSpot(VisPoint pos, int penIndex = 0, float scale = 1f);
	    public abstract void DrawCircle(VisCircle circ, int penIndex = 0);
	    public abstract void DrawOval(VisRectangle rect, int penIndex = 0);
	    public abstract void DrawRect(VisRectangle rect, int penIndex = 0);
	    public abstract void DrawLine(VisLine line, int penIndex = 0);
	    public abstract void DrawLine(VisPoint p0, VisPoint p1, int penIndex = 0);
	    public abstract void DrawPolyline(VisPoint[] points, int penIndex = 0);

	    public int Width { get => Control.Width; set => Control.Width = value; }
	    public int Height { get => Control.Height; set => Control.Height = value; }

        public event MouseEventHandler MouseDown
	    {
		    add => Control.MouseDown += value;
		    remove => Control.MouseDown -= value;
	    }
	    public event MouseEventHandler MouseMove
	    {
		    add => Control.MouseMove += value;
		    remove => Control.MouseMove -= value;
	    }
	    public event MouseEventHandler MouseUp
	    {
		    add => Control.MouseUp += value;
		    remove => Control.MouseUp -= value;
        }

	    public void Invalidate()
	    {
		    Control.Invalidate();
	    }

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
			    DrawStroke(path, 1);
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
                DrawRulerTicks(line, line.Length / 8);
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

        public void DrawStroke(VisStroke stroke, int penIndex = 0)
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

	    public void DrawIPath(IPath path, int penIndex = 0)
	    {
		    if (path is IPrimitive primitive)
		    {
                DrawPrimitive(primitive, penIndex);
		    }
            else if (path is VisStroke stroke)
		    {
                DrawStroke(stroke, penIndex);
		    }
	    }

	    public void DrawShape(VisStroke shape, int penIndex = 0)
	    {
	    }

	    public void DrawRulerTicks(VisStroke stroke, IPath unitPath, int penIndex = 0)
	    {
		    DrawRulerTicks(stroke, unitPath.Length, penIndex);
        }

	    public void DrawRulerTicks(IPath path, float unitLength, int penIndex = 0)
	    {
		    if (unitLength > 0 && path.Length > 0)
		    {
			    var strokeLen = path.Length;
	            if (unitLength > 0 && strokeLen > unitLength)
	            {
		            for (var total = 0f; total <= strokeLen; total += unitLength)
		            {
			            var pt = path.GetPoint(total / strokeLen);
	                    DrawSpot(pt, 5, 1f);
		            }
	            }
		    }
		    Flush();
	    }
    }
}
