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

        private void OnPaint(object sender, PaintEventArgs e)
        {
	        if (Agent != null)
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

        public override void DrawSpot(VisPoint pos, int penIndex = 0, float scale = 1f)
        {
            var r = Pens[penIndex].Width * scale;
            _graphics.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }

        public override void DrawCircle(VisCircle circ, int penIndex = 0)
        {
            var pos = circ.Center;
            var r = circ.Radius;
            _graphics.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }
        public override void DrawRect(VisRectangle rect, int penIndex = 0)
        {
            _graphics.DrawRectangle(Pens[penIndex], rect.TopLeft.X, rect.TopLeft.Y, rect.Size.X, rect.Size.Y);
        }

        public override void DrawLine(VisLine line, int penIndex = 0)
        {
            _graphics.DrawLine(Pens[penIndex], line.StartPoint.X, line.StartPoint.Y, line.EndPoint.X, line.EndPoint.Y);
        }
        public override void DrawLine(VisPoint p0, VisPoint p1, int penIndex = 0)
        {
            _graphics.DrawLine(Pens[penIndex], p0.X, p0.Y, p1.X, p1.Y);
        }
        public override void DrawPolyline(VisPoint[] points, int penIndex = 0)
        {
            _graphics.DrawLines(Pens[penIndex], ToPoints(points));
        }

        public void DrawShape(VisStroke shape)
        {
	        //foreach (var stroke in shape.Strokes)
	        //{
	        //    DrawStroke(stroke, (int)shape.StructuralType);
	        //}
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


        public override void GeneratePens()
        {
	        Pens = new VisPens(UnitPixels * 4);
        }

        private PointF[] ToPoints(VisPoint[] points)
        {
            var result = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                result[i] = points[i].PointF;
            }
            return result;
        }
    }
}
