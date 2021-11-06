using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Vis.Model.Agent;
using Vis.Model.Primitives;

namespace Vis.Model.Controller
{
    public class SkiaRenderer : RendererBase
    {
	    private SkiaPens Pens;
        public SkiaRenderer(Control parent, int width = -1, int height = -1) : base(parent, width, height)
        {
        }

        protected override Control CreateControl()
        {
            var result = new SKControl();
            result.PaintSurface += SkiaRenderer_PaintSurface;
            //var result = new SKGLControl();
            //result.PaintSurface += GL_PaintSurface;
            return result;
        }

        private void GL_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
	        if (Agent != null)
	        {
		        _canvas = e.Surface.Canvas;
		        BeginDraw();
		        Draw();
		        EndDraw();
	        }
        }

        private void SkiaRenderer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
	        if (Agent != null)
	        {
		        _canvas = e.Surface.Canvas;
		        BeginDraw();
		        Draw();
		        EndDraw();
	        }
        }


        private SKCanvas _canvas;
        public override void BeginDraw()
        {
	        _canvas.Save();
            _canvas.Scale(UnitPixels, UnitPixels);
	        _canvas.Clear(SKColors.WhiteSmoke); 
        }
        public override void EndDraw()
        {
            _canvas.Restore();
            _canvas = null;
            OnDrawingComplete();
        }

        public override void Flush()
        {
            _canvas.Flush();
        }

        public override void DrawSpot(VisPoint pos, PadAttributes attributes = null, float scale = 1f)
        {
	        var pen = Pens.GetPenForUIType(UIType.HighlightSpot);
	        var r = pen.StrokeWidth * scale;
            _canvas.DrawCircle(pos.X, pos.Y, r, pen);
        }
        public override void DrawCircle(VisCircle circ, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
	        _canvas.DrawCircle(circ.Center.X, circ.Center.Y, circ.Radius, pen);
        }

        public override void DrawOval(VisRectangle rect, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _canvas.DrawOval(rect.Center.X, rect.Center.Y, rect.HalfSize.X, rect.HalfSize.Y, pen);
        }

        public override void DrawRect(VisRectangle rect, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _canvas.DrawRect(rect.SKRect(), pen);
        }

        public override void DrawLine(VisLine line, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _canvas.DrawLine(line.X, line.Y, line.EndPoint.X, line.EndPoint.Y, pen);
        }

        public override void DrawLine(VisPoint p0, VisPoint p1, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _canvas.DrawLine(p0.X, p0.Y, p1.X, p1.Y, pen);
        }

        public override void DrawPolyline(VisPoint[] points, PadAttributes attributes = null)
        {
	        var pen = Pens.GetPenForElement(attributes);
            _canvas.DrawPoints(SKPointMode.Polygon, points.SKPoints(), pen);
        }

        public override void GeneratePens()
        {
	        Pens = new SkiaPens(UnitPixels * 4);
        }

        //public void Draw(Graphics IAgent agent, int penIndex = 0)
        //{
        //    //g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(-1f, 0), new PointF(1f, 0));
        //    //g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(0, -1f), new PointF(0, 1f));

        //    foreach (var prim in agent.FocusPad.Paths)
        //    {
        //        DrawPrimitive(prim, penIndex);
        //    }

        //}
    }
    public static class SkiaExtensions
    {
	    public static SKPoint[] SKPoints(this VisPoint[] points)
	    {
		    var result = new SKPoint[points.Length];
		    for (int i = 0; i < points.Length; i++)
		    {
			    result[i] = points[i].SKPoint();
		    }
		    return result;
	    }
	    public static SKPoint SKPoint(this VisPoint point) => new SKPoint(point.X, point.Y);
	    public static SKRect SKRect(this VisRectangle rect) => new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }
}