using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vis.Model.Agent;
using Vis.Model.Primitives;
using Vis.Model.UI;

namespace Vis.Model.Controller
{
    public interface IRenderer
    {
        UIStatus Status { get; set; }
        int Width { get; }
        int Height { get; }
        float UnitPixels { get; set; }
        int PenIndex { get; set; }

        Control AddAsControl(Control parent, bool useGL = false);
        event EventHandler DrawingComplete;
    }

    public abstract class RendererBase : IRenderer
    {
	    public event EventHandler DrawingComplete;

	    public UIStatus Status { get; set; }

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

        public int Width { get; protected set; }
	    public int Height { get; protected set; }
	    public bool DrawTicks { get; set; } = true;

        protected RendererBase()
        {
		    GeneratePens();
	    }

	    public abstract Control AddAsControl(Control parent, bool useGL = false);

	    public abstract void GeneratePens();

        public abstract void DrawOnBitmap();
	    public abstract void BeginDraw();
	    public abstract void EndDraw();
	    public abstract void Flush();

	    public abstract void DrawSpot(VisPoint pos, ElementRecord attributes = null, float scale = 1f);
	    public abstract void DrawTick(VisPoint pos, ElementRecord attributes = null, float scale = 1f);
	    public abstract void DrawCircle(VisCircle circ, ElementRecord attributes = null);
	    public abstract void DrawOval(VisRectangle rect, ElementRecord attributes = null);
	    public abstract void DrawRect(VisRectangle rect, ElementRecord attributes = null);
	    public abstract void DrawLine(VisLine line, ElementRecord attributes = null);
	    public abstract void DrawLine(VisPoint p0, VisPoint p1, ElementRecord attributes = null);
	    public abstract void DrawLines(VisPoint[] points, ElementRecord attributes = null);
	    public abstract void DrawPolyline(VisPolyline polyline, ElementRecord attributes = null);

        protected void OnDrawingComplete()
	    {
		    DrawingComplete?.Invoke(this, EventArgs.Empty);
        }

	    public void Draw()
	    {
		    foreach (var pad in Status.Pads)
		    {
			    if (pad.PadStyle != PadStyle.Hidden)
			    {
				    foreach (var path in pad.Paths)
				    {
					    DrawElement(path);
				    }
			    }
		    }
	    }

	    public void DrawElement(ElementRecord elementRecord)
	    {
		    var path = elementRecord.Element;

		    if (path is VisStroke stroke)
		    {
                DrawStroke(elementRecord);
		    }
		    else if (path is VisLine line)
		    {
                DrawLine(line.StartPoint, line.EndPoint, elementRecord);
                if (line.UnitReference != null)
                {
	                DrawRulerTicks(line, line.UnitReference);
                }
		    }
		    else if (path is VisCircle circ)
		    {
			    DrawSpot(circ.Center, elementRecord);
			    DrawCircle(circ, elementRecord);
			    if (circ.UnitReference != null)
			    {
				    DrawRulerTicks(circ, circ.UnitReference);
			    }
		    }
		    else if (path is VisRectangle rect)
		    {
			    DrawRect(rect, elementRecord);
		    }
		    else if (path is VisPolyline poly)
		    {
			    DrawPolyline(poly, elementRecord);
		    }
            else if (path is RenderPoint rp)
		    {
			    DrawSpot(rp, elementRecord, rp.Scale);
            }
		    else if (path is VisPoint p)
		    {
			    DrawSpot(p, elementRecord);
		    }
	    }

        public void DrawStroke(ElementRecord elementRecord)
        {
	        if (elementRecord.Element is VisStroke stroke)
	        {
		        foreach (var segment in stroke.Segments)
		        {
			        if (segment is VisLine line)
			        {
				        DrawLine(line, elementRecord);
			        }
			        else if (segment is VisArc arc)
			        {
				        DrawLines(arc.GetPolylinePoints(), elementRecord);
			        }
		        }

		        if (stroke.UnitReference != null)
		        {
			        DrawRulerTicks(stroke, stroke.UnitReference);
		        }

		        Flush();
	        }
        }

	    public void DrawShape(VisStroke shape, int penIndex = 0)
	    {
	    }

	    public void DrawRulerTicks(IPath stroke, IPath unitPath, int penIndex = 0)
	    {
		    DrawRulerTicks(stroke, unitPath.Length, penIndex);
	    }

	    public void DrawRulerTicks(IPath path, float unitLength, int penIndex = 0)
	    {
		    if (DrawTicks)
		    {
			    if (unitLength > 0 && path.Length > 0)
			    {
				    var strokeLen = path.Length;
				    float scale = 1.0f; // first tick larger
				    if (unitLength > 0 && strokeLen > unitLength)
				    {
					    for (var total = 0f; total <= strokeLen; total += unitLength)
					    {
						    var pt = path.GetPoint(total / strokeLen);
						    DrawTick(pt, null, scale);
						    scale = 0.6f;

					    }
				    }
			    }

			    Flush();
		    }
	    }
    }
}
