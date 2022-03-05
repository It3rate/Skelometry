using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Entities;
using Slugs.Input;
using Slugs.Pads;
using Slugs.Primitives;
using Slugs.UI;

namespace Slugs.Renderer
{
	public class SlugRenderer : RendererBase
    {
        public SlugRenderer() : base()
        {
	    }

        public override void DrawRoundBox(SKPoint point, SKPaint paint, float radius = 8f)
        {
	        float round = radius / 3f;
	        var box = new SKRect(point.X - radius, point.Y - radius, point.X + radius, point.Y + radius);
	        Canvas.DrawRoundRect(box, round, round, paint);
        }
        public override void DrawPolyline(SKPoint[] polyline, SKPaint paint)
        {
	        Canvas.DrawPoints(SKPointMode.Polygon, polyline, paint);
        }
        public override void DrawPath(SKPoint[] polyline, SKPaint paint)
        {
	        var path = new SKPath
	        {
		        FillType = SKPathFillType.EvenOdd
	        };
            path.MoveTo(polyline[0]);
            path.AddPoly(polyline, true);
	        Canvas.DrawPath(path, paint);
        }
        public override void DrawDirectedLine(SKSegment seg, SKPaint paint)
        {
            DrawPolyline(seg.Points, paint);
	        Canvas.DrawCircle(seg.StartPoint, 2, paint);
            var triPts = seg.EndArrow(8);
            Canvas.DrawPoints(SKPointMode.Polygon, triPts, paint);
        }
        public override void DrawText(SKPoint center, string text, SKPaint paint)
        {
	        var rect = GetTextBackgroundSize(center.X, center.Y, text, paint);
            Canvas.DrawRoundRect(rect, 5,5, Pens.TextBackgroundPen);
            Canvas.DrawText(text, center.X, center.Y, paint);
        }
        public override void DrawBitmap(SKBitmap bitmap)
        {
            Canvas.DrawBitmap(bitmap, new SKRect(0,0, Width, Height));
        }

        public override void GeneratePens()
        {
	        Pens = new SlugPens(1);
        }
    }

}