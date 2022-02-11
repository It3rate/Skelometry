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

namespace Slugs.Renderer
{
	public class SlugRenderer
    {
        public RenderStatus Status { get; set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
	    public event EventHandler DrawingComplete;

	    public UIData Data { get; set; }

        public int PenIndex { get; set; }

        public SlugPens Pens { get; set; }
	    public SKBitmap Bitmap { get; set; }
	    public bool ShowBitmap { get; set; }

        public SlugRenderer()
        {
            GeneratePens();
	    }

        private bool hasControl = false;
        public Control AddAsControl(Control parent, bool useGL = false)
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

        public void DrawOnBitmap()
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
	        if (true || Status != null)
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
	        //WorkingPad = null;
        }

        public void Draw()
        {
            if (Data.HasHighlightPoint)
			{
				DrawRoundBox(Data.HighlightPoint.Position, Pens.HoverPen);
			}

			if (Data.HasHighlightLine)
			{
				DrawDirectedLine(Data.HighlightLine.Segment, Pens.HoverPen);
			}

	        foreach (var selectedElement in Data.Selected.Elements)
	        {
		        DrawElement(selectedElement, Pens.SelectedPen);
	        }

	        foreach (var pad in Data.Pads)
	        {
		        pad.Refresh();
		        var slug = Pad.ActiveSlug;
		        foreach (var entity in pad.Entities)
		        {
			        foreach (var trait in entity.Traits)
			        {
				        DrawDirectedLine(trait.Segment, trait.IsLocked ? Pens.LockedPen : Pens.DarkPen);
				        _canvas.Flush();
				        foreach (var focal in trait.Focals)
				        {
					        DrawDirectedLine(focal.Segment, Pens.FocalPen);
					        foreach (var bond in focal.Bonds)
					        {
						        DrawDirectedLine(bond.Segment, Pens.BondPen);
					        }
				        }
			        }
		        }

		        foreach (var doubleBond in pad.ElementsOfKind(ElementKind.DoubleBond))
		        {
			        var db = (DoubleBond) doubleBond;
			        var pen = Data.Highlight.FirstElement.Key == db.Key ? Pens.BondSelectPen : Pens.BondFillPen;

                    SKPoint[] pts = new SKPoint[] { db.StartFocal.StartPosition, db.StartFocal.EndPosition, db.EndFocal.EndPosition, db.EndFocal.StartPosition };
			        DrawPath(pts, pen);
		        }

		        if (Data.WorkingPoints.Count > 0)
		        {
			        DrawPath(Data.WorkingPoints.ToArray(), Pens.BondFillPen);
		        }
	        }
        }

        public void DrawElement(IElement element, SKPaint paint, float radius = 4f)
        {
	        if (element is SegmentBase seg)
	        {
                _canvas.DrawLine(seg.Segment.StartPoint, seg.Segment.EndPoint, paint);
	        }
            else if (element is IPoint point)
	        {
                _canvas.DrawCircle(point.Position.X, point.Position.Y, radius, paint);
	        }
        }

        public void DrawRoundBox(SKPoint point, SKPaint paint, float radius = 8f)
        {
	        float round = radius / 3f;
	        var box = new SKRect(point.X - radius, point.Y - radius, point.X + radius, point.Y + radius);
	        _canvas.DrawRoundRect(box, round, round, paint);
        }

        public void DrawPolyline(SKPoint[] polyline, SKPaint paint)
        {
	        _canvas.DrawPoints(SKPointMode.Polygon, polyline, paint);
        }
        public void DrawPath(SKPoint[] polyline, SKPaint paint)
        {
	        var path = new SKPath
	        {
		        FillType = SKPathFillType.EvenOdd
	        };
            path.MoveTo(polyline[0]);
            path.AddPoly(polyline, true);
	        _canvas.DrawPath(path, paint);
        }

        public void DrawDirectedLine(SKSegment seg, SKPaint paint)
        {
	            DrawPolyline(seg.Points, paint);
		        _canvas.DrawCircle(seg.StartPoint, 2, paint);
	            var triPts = seg.EndArrow(8);
	            _canvas.DrawPoints(SKPointMode.Polygon, triPts, paint);
        }

        private SKCanvas _canvas;
        public void BeginDraw()
        {
	        _canvas.Save();
            if (hasControl == false)
            {
	            _canvas.Clear(SKColors.White);
            }
            else
            {
	            _canvas.Clear(SKColors.Beige);
            }
				
        }
        public void EndDraw()
        {
            _canvas.Restore();
	        if (ShowBitmap && Bitmap != null)
	        {
                DrawBitmap(Bitmap);
	        }
            _canvas = null;
            OnDrawingComplete();
        }

        protected void OnDrawingComplete()
        {
	        DrawingComplete?.Invoke(this, EventArgs.Empty);
        }


        public void Flush()
        {
            _canvas.Flush();
        }

        public void DrawBitmap(SKBitmap bitmap)
        {
            _canvas.DrawBitmap(bitmap, new SKRect(0,0, Width, Height));
        }

     //   public override void DrawSpot(VisPoint pos, ElementRecord attributes = null, float scale = 1f)
     //   {
	    //    var pen = Pens.GetPenForUIType(ElementType.HighlightSpot);
	    //    var r = pen.StrokeWidth * scale;
	    //    _canvas.DrawCircle(pos.X, pos.Y, r, pen);
     //   }
     //   public override void DrawTick(VisPoint pos, ElementRecord attributes = null, float scale = 1f)
     //   {
	    //    var pen = Pens.GetPenForUIType(ElementType.MeasureTick);
	    //    var r = pen.StrokeWidth * scale;
	    //    _canvas.DrawCircle(pos.X, pos.Y, r, pen);
     //   }
     //   public override void DrawCircle(VisCircle circ, ElementRecord attributes = null)
     //   {
	    //    var pens = Pens.GetPensForElement(attributes);
     //       foreach(var pen in pens)
     //       {
	    //        if (pen != null)
	    //        {
					//_canvas.DrawCircle(circ.Center.X, circ.Center.Y, circ.Radius, pen);
	    //        }
     //       }
     //   }

     //   public override void DrawOval(VisRectangle rect, ElementRecord attributes = null)
     //   {
	    //    var pens = Pens.GetPensForElement(attributes);
	    //    foreach (var pen in pens)
	    //    {
		   //     if (pen != null)
		   //     {
			  //      _canvas.DrawOval(rect.Center.X, rect.Center.Y, rect.HalfSize.X, rect.HalfSize.Y, pen);
     //           }
	    //    }
     //   }

     //   public override void DrawRect(VisRectangle rect, ElementRecord attributes = null)
     //   {
	    //    var pens = Pens.GetPensForElement(attributes);
	    //    foreach (var pen in pens)
	    //    {
		   //     if (pen != null)
		   //     {
			  //      _canvas.DrawRect(rect.SKRect(), pen);
     //           }
	    //    }
     //   }

     //   public override void DrawLine(VisLine seg, ElementRecord attributes = null)
     //   {
	    //    var pens = Pens.GetPensForElement(attributes);
	    //    foreach (var pen in pens)
	    //    {
		   //     if (pen != null)
		   //     {
			  //      _canvas.DrawLine(seg.X, seg.Y, seg.EndKey.X, seg.EndKey.Y, pen);
     //           }
	    //    }
     //   }

     //   public override void DrawLine(VisPoint p0, VisPoint p1, ElementRecord attributes = null)
     //   {
	    //    var pens = Pens.GetPensForElement(attributes);
	    //    foreach (var pen in pens)
	    //    {
		   //     if (pen != null)
		   //     {
			  //      _canvas.DrawLine(p0.X, p0.Y, p1.X, p1.Y, pen);
     //           }
	    //    }
     //   }

     //   public override void DrawLines(VisPoint[] points, ElementRecord attributes = null)
     //   {
	    //    var pens = Pens.GetPensForElement(attributes);
	    //    foreach (var pen in pens)
	    //    {
		   //     if (pen != null)
		   //     {
			  //      _canvas.DrawPoints(SKPointMode.Polygon, points.SKPoints(), pen);
     //           }
	    //    }
     //   }
     //   public override void DrawPolyline(VisPolyline polyline, ElementRecord attributes = null)
     //   {
	    //    var pens = Pens.GetPensForElement(attributes);
	    //    foreach (var pen in pens)
	    //    {
		   //     if (pen != null)
		   //     {
			  //      _canvas.DrawPoints(SKPointMode.Polygon, polyline.Points.SKPoints(), pen);
			  //      //_canvas.DrawLine(polyline.Points[0].X, polyline.Points[0].Y, polyline.Points[1].X, polyline.Points[1].Y, Pens.SelectedPen);
     //           }
	    //    }
     //   }

        public void GeneratePens()
        {
	        Pens = new SlugPens(1);
        }
    }
    public static class SkiaExtensions
    {
	    //public static Position[] SKPoints(this IEnumerable<VisPoint> points)
	    //{
		   // var result = new Position[points.Count()];
		   // int index = 0;
		   // foreach (var visPoint in points)
		   // {
			  //  result[index++] = visPoint.Position();
     //       }
		   // return result;
	    //}
	    //public static Position Position(this VisPoint point) => new Position(point.X, point.Y);
	    //public static SKRect SKRect(this VisRectangle rect) => new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }
}