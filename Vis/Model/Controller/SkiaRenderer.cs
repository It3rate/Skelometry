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
        private SkiaPens Pens { get; set; }

        public SkiaRenderer(Control parent, int width = -1, int height = -1) : base(parent, width, height)
        {
        }

        protected override Control CreateControl()
        {
            var result = new SKControl();
            result.PaintSurface += SkiaRenderer_PaintSurface;
            return result;
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

        public override void DrawSpot(VisPoint pos, int penIndex = 0, float scale = 1f)
        {
	        var r = Pens[penIndex].StrokeWidth * scale;
            _canvas.DrawCircle(pos.X, pos.Y, r, Pens[penIndex]);
        }
        public override void DrawCircle(VisCircle circ, int penIndex = 0)
        {
            _canvas.DrawCircle(circ.Center.X, circ.Center.Y, circ.Radius, Pens[penIndex]);
        }

        public override void DrawRect(VisRectangle rect, int penIndex = 0)
        {
	        var skRect = new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
            _canvas.DrawRect(skRect, Pens[penIndex]);
        }

        public override void DrawLine(VisLine line, int penIndex = 0)
        {
            _canvas.DrawLine(line.X, line.Y, line.EndPoint.X, line.EndPoint.Y, Pens[penIndex]);
        }

        public override void DrawLine(VisPoint p0, VisPoint p1, int penIndex = 0)
        {
	        _canvas.DrawLine(p0.X, p0.Y, p1.X, p1.Y, Pens[penIndex]);
        }

        public override void DrawPolyline(VisPoint[] points, int penIndex = 0)
        {
            _canvas.DrawPoints(SKPointMode.Polygon, ToPoints(points), Pens[penIndex]);
        }

        public override void GeneratePens()
        {
	        Pens = new SkiaPens(UnitPixels * 4);
        }

        private SKPoint[] ToPoints(VisPoint[] points)
        {
	        var result = new SKPoint[points.Length];
	        for (int i = 0; i < points.Length; i++)
	        {
		        result[i] = new SKPoint(points[i].X, points[i].Y);
	        }
	        return result;
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
}