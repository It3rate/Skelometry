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
using Vis.Model.Render;

namespace Vis.Model.Controller
{
    public class SkiaRenderer : RendererBase
    {
	    public SkiaPens Pens { get; set; }
	    public SKBitmap Bitmap { get; set; }
	    public bool ShowBitmap { get; set; }

        public SkiaRenderer()
	    {
	    }

        private bool hasControl = false;
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
	        hasControl = true;

            return result;
        }

        public SKBitmap GenerateBitmap(int width, int height)
        {
	        Bitmap = new SKBitmap(width, height);
	        return Bitmap;
        }

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
	        if (Status != null)
	        {
		        DrawOnCanvas(e.Surface.Canvas);
            }
        }

        private void SkiaRenderer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
	        if (Status != null)
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
            if (hasControl == false)
            {
	            _canvas.Clear(SKColors.White);
            }
            else
            {
	            _canvas.Clear(SKColors.Beige);
            }
				
        }
        public override void EndDraw()
        {
            _canvas.Restore();
	        if (ShowBitmap && Bitmap != null)
	        {
                DrawBitmap(Bitmap);
	        }
            _canvas = null;
            OnDrawingComplete();
        }

        public override void Flush()
        {
            _canvas.Flush();
        }

        public void DrawBitmap(SKBitmap bitmap)
        {
            _canvas.DrawBitmap(bitmap, new SKRect(0,0, Width, Height));
        }

        public override void DrawSpot(VisPoint pos, ElementRecord attributes = null, float scale = 1f)
        {
	        var pen = Pens.GetPenForUIType(ElementType.HighlightSpot);
	        var r = pen.StrokeWidth * scale;
	        _canvas.DrawCircle(pos.X, pos.Y, r, pen);
        }
        public override void DrawTick(VisPoint pos, ElementRecord attributes = null, float scale = 1f)
        {
	        var pen = Pens.GetPenForUIType(ElementType.MeasureTick);
	        var r = pen.StrokeWidth * scale;
	        _canvas.DrawCircle(pos.X, pos.Y, r, pen);
        }
        public override void DrawCircle(VisCircle circ, ElementRecord attributes = null)
        {
	        var pens = Pens.GetPensForElement(attributes);
            foreach(var pen in pens)
            {
	            if (pen != null)
	            {
					_canvas.DrawCircle(circ.Center.X, circ.Center.Y, circ.Radius, pen);
	            }
            }
        }

        public override void DrawOval(VisRectangle rect, ElementRecord attributes = null)
        {
	        var pens = Pens.GetPensForElement(attributes);
	        foreach (var pen in pens)
	        {
		        if (pen != null)
		        {
			        _canvas.DrawOval(rect.Center.X, rect.Center.Y, rect.HalfSize.X, rect.HalfSize.Y, pen);
                }
	        }
        }

        public override void DrawRect(VisRectangle rect, ElementRecord attributes = null)
        {
	        var pens = Pens.GetPensForElement(attributes);
	        foreach (var pen in pens)
	        {
		        if (pen != null)
		        {
			        _canvas.DrawRect(rect.SKRect(), pen);
                }
	        }
        }

        public override void DrawLine(VisLine line, ElementRecord attributes = null)
        {
	        var pens = Pens.GetPensForElement(attributes);
	        foreach (var pen in pens)
	        {
		        if (pen != null)
		        {
			        _canvas.DrawLine(line.X, line.Y, line.EndPoint.X, line.EndPoint.Y, pen);
                }
	        }
        }

        public override void DrawLine(VisPoint p0, VisPoint p1, ElementRecord attributes = null)
        {
	        var pens = Pens.GetPensForElement(attributes);
	        foreach (var pen in pens)
	        {
		        if (pen != null)
		        {
			        _canvas.DrawLine(p0.X, p0.Y, p1.X, p1.Y, pen);
                }
	        }
        }

        public override void DrawLines(VisPoint[] points, ElementRecord attributes = null)
        {
	        var pens = Pens.GetPensForElement(attributes);
	        foreach (var pen in pens)
	        {
		        if (pen != null)
		        {
			        _canvas.DrawPoints(SKPointMode.Polygon, points.SKPoints(), pen);
                }
	        }
        }
        public override void DrawPolyline(VisPolyline polyline, ElementRecord attributes = null)
        {
	        var pens = Pens.GetPensForElement(attributes);
	        foreach (var pen in pens)
	        {
		        if (pen != null)
		        {
			        _canvas.DrawPoints(SKPointMode.Polygon, polyline.Points.SKPoints(), pen);
			        //_canvas.DrawLine(polyline.Points[0].X, polyline.Points[0].Y, polyline.Points[1].X, polyline.Points[1].Y, Pens.SelectedPen);
                }
	        }
        }

        public override void GeneratePens()
        {
	        Pens = new SkiaPens(UnitPixels * 4);
        }
    }
    public static class SkiaExtensions
    {
	    public static SKPoint[] SKPoints(this IEnumerable<VisPoint> points)
	    {
		    var result = new SKPoint[points.Count()];
		    int index = 0;
		    foreach (var visPoint in points)
		    {
			    result[index++] = visPoint.SKPoint();
            }
		    return result;
	    }
	    public static SKPoint SKPoint(this VisPoint point) => new SKPoint(point.X, point.Y);
	    public static SKRect SKRect(this VisRectangle rect) => new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }
}