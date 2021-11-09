using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vis.Model.Agent;
using Vis.Model.Primitives;

namespace Vis.Model.Controller
{
	public class VisRenderer : RendererBase
    {
	    private VisPens Pens { get; set;  }

	    public VisRenderer(Control parent, int width = -1, int height = -1) : base(parent, width, height)
	    {
	    }

	    protected override Control CreateControl()
	    {
		    var result = new Panel();
            result.Paint += OnPaint;
		    return result;
	    }

	    protected override void GenerateBitmap(int width, int height)
	    {
		    throw new NotImplementedException();
	    }
        public override void DrawOnBitmap()
        {
	        throw new NotImplementedException();
        }

	    private void OnPaint(object sender, PaintEventArgs e)
        {
	        if (Pads != null)
	        {
		        _graphics = e.Graphics;
		        BeginDraw();
		        Draw();
		        EndDraw();
	        }
        }

        private Graphics _graphics;
        private GraphicsState _graphicsState;

        public void TranslateContext(float x, float y)
        {
            _graphics.TranslateTransform(x, y);
        }


        public override void BeginDraw()
        {
            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _graphicsState = _graphics.Save();
            _graphics.TranslateTransform(10, 10);
            _graphics.ScaleTransform(UnitPixels, UnitPixels);

        }

        public override void EndDraw()
        {
	        _graphics.Restore(_graphicsState);
	        _graphicsState = null;
	        _graphics = null;
	        OnDrawingComplete();
        }

        public override void Flush()
        {
            _graphics.Flush();
        }

        public override void DrawSpot(VisPoint pos, PadAttributes attributes = null, float scale = 1f)
        {
	        var pen = Pens.GetPenForUIType(UIType.HighlightSpot);
	        var r = pen.Width * scale;
            _graphics.DrawEllipse(pen, pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }

        public override void DrawTick(VisPoint pos, PadAttributes attributes = null, float scale = 1f)
        {
	        var pen = Pens.GetPenForUIType(UIType.MeasureTick);
	        var r = pen.Width * scale;
	        _graphics.DrawEllipse(pen, pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }

        public override void DrawCircle(VisCircle circ, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            var pos = circ.Center;
            var r = circ.Radius;
            _graphics.DrawEllipse(pen, pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }
        public override void DrawOval(VisRectangle rect, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _graphics.DrawEllipse(pen, rect.RectF());
        }
        public override void DrawRect(VisRectangle rect, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _graphics.DrawRectangle(pen, rect.TopLeft.X, rect.TopLeft.Y, rect.Size.X, rect.Size.Y);
        }

        public override void DrawLine(VisLine line, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _graphics.DrawLine(pen, line.StartPoint.X, line.StartPoint.Y, line.EndPoint.X, line.EndPoint.Y);
        }
        public override void DrawLine(VisPoint p0, VisPoint p1, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _graphics.DrawLine(pen, p0.X, p0.Y, p1.X, p1.Y);
        }
        public override void DrawPolyline(VisPoint[] points, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _graphics.DrawLines(pen, points.PointFs());
        }

        public void DrawShape(VisStroke shape)
        {
	        //foreach (var stroke in shape.Strokes)
	        //{
	        //    DrawStroke(stroke, (int)shape.StructuralType);
	        //}
        }

        public void DrawPath(VisStroke stroke, PadAttributes attributes = null)
        {
	        foreach (var segment in stroke.Segments)
	        {
		        if (segment is VisLine line)
		        {
			        DrawLine(line, attributes);
		        }
		        else if (segment is VisArc arc)
		        {
			        DrawPolyline(arc.GetPolylinePoints(), attributes);
		        }
	        }
	        Flush();
        }


        public override void GeneratePens()
        {
	        Pens = new VisPens(UnitPixels * 4);
        }

    }

    public static class GdiExtensions
    {
	    public static PointF[] PointFs(this VisPoint[] points)
	    {
		    var result = new PointF[points.Length];
		    for(int i = 0; i < points.Length; i++)
		    {
			    result[i] = points[i].PointF();
		    }
		    return result;
	    }
	    public static PointF PointF(this VisPoint point) => new PointF(point.X, point.Y);
	    public static RectangleF RectF(this VisRectangle rect) => new RectangleF(rect.Left, rect.Top, rect.Width, rect.Height);
    }
}
