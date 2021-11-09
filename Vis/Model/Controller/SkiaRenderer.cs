using System;
using System.Collections.Generic;
using System.Drawing;
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
	    public SkiaRenderer()
	    {
	    }

        public override Control AddAsControl(Control parent, bool useGL = false)
        {
	        Control result;
	        if (useGL)
	        {
                result = new SKGLControl();
                ((SKGLControl)result).PaintSurface += GL_PaintSurface;
            }
	        else
	        {
	            result = new SKControl();
	            ((SKControl)result).PaintSurface += SkiaRenderer_PaintSurface;
	        }
	        result.Width = parent.Width;
	        result.Height = parent.Height;
	        Width = result.Width;
	        Height = result.Height;
	        parent.Controls.Add(result);
            result.BackColor = Color.Bisque;
	        return result;
        }

        public SKBitmap GenerateBitmap(int width, int height)
        {
	        Bitmap = new SKBitmap(width, height);
	        return Bitmap;
        }

        public SKBitmap Bitmap { get; private set; }


        public override void DrawOnBitmap()
        {
	        if (Bitmap != null)
	        {
		        using (SKCanvas canvas = new SKCanvas(Bitmap))
		        {
					DrawOnCanvas(canvas);
		        }
	        }
        }

        private void GL_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
	        if (Pads != null)
	        {
		        DrawOnCanvas(e.Surface.Canvas);
            }
        }

        private void SkiaRenderer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
	        if (Pads != null)
	        {
		        DrawOnCanvas(e.Surface.Canvas);
	        }
        }

        public void DrawOnCanvas(SKCanvas canvas)
        {
	        _canvas = canvas;
	        BeginDraw();
	        Draw();
	        EndDraw();
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
        public override void DrawTick(VisPoint pos, PadAttributes attributes = null, float scale = 1f)
        {
	        var pen = Pens.GetPenForUIType(UIType.MeasureTick);
	        var r = pen.StrokeWidth * scale;
	        _canvas.DrawCircle(pos.X, pos.Y, r, pen);
            //SKPath path = new SKPath();
            //path.AddCircle(pos.X, pos.Y, r);
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