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

	    public abstract void DrawSpot(VisPoint pos, PadAttributes attributes = null, float scale = 1f);
	    public abstract void DrawTick(VisPoint pos, PadAttributes attributes = null, float scale = 1f);
	    public abstract void DrawCircle(VisCircle circ, PadAttributes attributes = null);
	    public abstract void DrawOval(VisRectangle rect, PadAttributes attributes = null);
	    public abstract void DrawRect(VisRectangle rect, PadAttributes attributes = null);
	    public abstract void DrawLine(VisLine line, PadAttributes attributes = null);
	    public abstract void DrawLine(VisPoint p0, VisPoint p1, PadAttributes attributes = null);
	    public abstract void DrawLines(VisPoint[] points, PadAttributes attributes = null);
	    public abstract void DrawPolyline(VisPolyline polyline, PadAttributes attributes = null);

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

	    public void DrawElement(PadAttributes padAttributes)
	    {
		    var path = padAttributes.Element;

		    if (path is VisStroke stroke)
		    {
                DrawStroke(padAttributes);
		    }
		    else if (path is VisLine line)
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
		    else if (path is VisPolyline poly)
		    {
			    DrawPolyline(poly, padAttributes);
		    }
            else if (path is RenderPoint rp)
		    {
			    DrawSpot(rp, padAttributes, rp.Scale);
            }
		    else if (path is VisPoint p)
		    {
			    DrawSpot(p, padAttributes);
		    }
	    }

        public void DrawStroke(PadAttributes padAttributes)
        {
	        if (padAttributes.Element is VisStroke stroke)
	        {
		        foreach (var segment in stroke.Segments)
		        {
			        if (segment is VisLine line)
			        {
				        DrawLine(line, padAttributes);
			        }
			        else if (segment is VisArc arc)
			        {
				        DrawLines(arc.GetPolylinePoints(), padAttributes);
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
