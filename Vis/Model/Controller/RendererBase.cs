using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
	    public abstract void DrawSpot(VisPoint pos, PadAttributes attributes = null, float scale = 1f);
	    public abstract void DrawCircle(VisCircle circ, PadAttributes attributes = null);
	    public abstract void DrawOval(VisRectangle rect, PadAttributes attributes = null);
	    public abstract void DrawRect(VisRectangle rect, PadAttributes attributes = null);
	    public abstract void DrawLine(VisLine line, PadAttributes attributes = null);
	    public abstract void DrawLine(VisPoint p0, VisPoint p1, PadAttributes attributes = null);
	    public abstract void DrawPolyline(VisPoint[] points, PadAttributes attributes = null);

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
	    public event KeyEventHandler KeyDown
	    {
		    add => Control.KeyDown += value;
		    remove => Control.KeyDown -= value;
	    }
	    public event KeyEventHandler KeyUp
        {
		    add => Control.KeyUp += value;
		    remove => Control.KeyUp -= value;
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
			    DrawPrimitive(prim);
		    }

		    foreach (var path in Agent.ViewPad.Paths)
		    {
			    DrawStroke(path);
		    }

		    foreach (var prim in Agent.WorkingPad.Paths)
		    {
			    DrawPrimitive(prim);
		    }
	    }

	    public void DrawPrimitive(PadAttributes<VisPoint> padAttributes)
	    {
		    var path = padAttributes.Element;
		    if (path is VisLine line)
		    {
                DrawLine(line.StartPoint, line.EndPoint, padAttributes);
                if (line.UnitReference != null)
                {
	                DrawRulerTicks(line, line.UnitReference);
                }
		    }
		    else if (path is VisCircle circ)
		    {
			    DrawSpot(circ.Center, padAttributes);
			    DrawCircle(circ, padAttributes);
			    if (circ.UnitReference != null)
			    {
				    DrawRulerTicks(circ, circ.UnitReference);
			    }
		    }
		    else if (path is VisRectangle rect)
		    {
			    DrawRect(rect, padAttributes);
		    }
		    else if (path is RenderPoint rp)
		    {
			    //DrawSpot(rp, rp.PenIndex, rp.Scale); // penIndex);
			    DrawSpot(rp, padAttributes, rp.Scale); // penIndex);
            }
		    else if (path is VisPoint p)
		    {
			    DrawSpot(p, padAttributes); // penIndex);
		    }
	    }

        public void DrawStroke(PadAttributes<VisStroke> padAttributes)
        {
	        var stroke = padAttributes.Element;
		    foreach (var segment in stroke.Segments)
		    {
			    if (segment is VisLine line)
			    {
				    DrawLine(line, padAttributes);
			    }
			    else if (segment is VisArc arc)
			    {
				    DrawPolyline(arc.GetPolylinePoints(), padAttributes);
                }
            }

		    if (stroke.UnitReference != null)
		    {
			    DrawRulerTicks(stroke, stroke.UnitReference);
		    }

		    Flush();
	    }

	    //public void DrawIPath(IPath path, int penIndex = 0)
	    //{
		   // if (path is IPrimitive primitive)
		   // {
     //           DrawPrimitive(primitive, penIndex);
		   // }
     //       else if (path is VisStroke stroke)
		   // {
     //           DrawStroke(stroke, penIndex);
		   // }

		   // if (path.UnitReference != null)
		   // {
			  //  DrawRulerTicks(path, path.UnitReference);
		   // }
	    //}
    

	    public void DrawShape(VisStroke shape, int penIndex = 0)
	    {
	    }

	    public void DrawRulerTicks(IPath stroke, IPath unitPath, int penIndex = 0)
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
	                    DrawSpot(pt, null, 1f);
		            }
	            }
		    }
		    Flush();
	    }
    }
}
