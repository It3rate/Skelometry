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

namespace Vis.Model.Controller
{
    public interface IRenderer
    {
        //IAgent Agent { get; set; }
        List<IPad> Pads { get; set; }
        int Width { get; }
        int Height { get; }
        float UnitPixels { get; set; }
        int PenIndex { get; set; }

        Control AddAsControl(Control parent, bool useGL = false);
        //void GenerateBitmap(int width, int height);
        event EventHandler DrawingComplete;
    }

    public abstract class RendererBase : IRenderer
    {
	    public event EventHandler DrawingComplete;

	    public List<IPad> Pads { get; set; }

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
        public bool HasBackingBitmap { get; }

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
	    public abstract void DrawPolyline(VisPoint[] points, PadAttributes attributes = null);

	    protected void OnDrawingComplete()
	    {
		    DrawingComplete?.Invoke(this, EventArgs.Empty);
        }

	    public void Draw()
	    {
		    foreach (var pad in Pads)
		    {
			    if (pad is VisPad<VisPoint> ppad)
			    {
				    foreach (var prim in ppad.Paths)
				    {
					    DrawPrimitive(prim);
				    }
                }
			    else if (pad is VisPad<VisStroke> spad)
			    {
				    foreach (var prim in spad.Paths)
				    {
					    DrawStroke(prim);
				    }
                }
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
	                    DrawTick(pt, null, 0.7f);
		            }
	            }
		    }
		    Flush();
	    }
    }
}
