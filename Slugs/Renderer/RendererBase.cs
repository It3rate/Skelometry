using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Entities;
using Slugs.Input;
using Slugs.Primitives;
using Slugs.UI;

namespace Slugs.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class RendererBase
    {
        public RenderStatus Status { get; set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public event EventHandler DrawingComplete;

        public UIData Data { get; set; }

        public SKCanvas Canvas;
        public SlugPens Pens { get; set; }
        public SKBitmap Bitmap { get; set; }
        public bool ShowBitmap { get; set; }

        public RendererBase()
        {
            GeneratePens();
        }

        protected bool hasControl = false;
        public Control AddAsControl(Control parent, bool useGL = false)
        {
            Control result;
            if (useGL)
            {
                result = new SKGLControl();
                ((SKGLControl)result).PaintSurface += DrawOnGLSurface;
            }
            else
            {
                result = new SKControl();
                ((SKControl)result).PaintSurface += DrawOnPaintSurface;
            }
            result.Width = parent.Width;
            result.Height = parent.Height;
            Width = result.Width;
            Height = result.Height;
            parent.Controls.Add(result);
            hasControl = true;


            return result;
        }

        private void DrawOnGLSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            if (Status != null)
            {
                DrawOnCanvas(e.Surface.Canvas);
            }
        }
        private void DrawOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (true || Status != null)
            {
                DrawOnCanvas(e.Surface.Canvas);
            }
        }
        public void DrawOnBitmapSurface()
        {
            if (Bitmap != null)
            {
                using (SKCanvas canvas = new SKCanvas(Bitmap))
                {
                    DrawOnCanvas(canvas);
                }
            }
        }

        public void DrawOnCanvas(SKCanvas canvas)
        {
            Canvas = canvas;
            BeginDraw();
            Draw();
            EndDraw();
            //WorkingPad = null;
        }
        public virtual void BeginDraw()
        {
            Canvas.Save();
            Canvas.SetMatrix(Data.Matrix);
            if (hasControl == false)
            {
                Canvas.Clear(SKColors.White);
            }
            else
            {
                Canvas.Clear(SKColors.Beige);
            }

        }
        public virtual void Draw()
        {
            if (Data.HasHighlightPoint)
            {
                DrawRoundBox(Data.HighlightPoint.Position, Pens.HoverPen);
            }

            if (Data.HasHighlightLine)
            {
                DrawDirectedLine(Data.HighlightLine.Segment, Pens.HoverPen);
            }
            foreach (var pad in Data.Pads)
            {
                pad.Refresh();
                foreach (var trait in pad.Traits)
                {
                    DrawDirectedLine(trait.Segment, trait.IsLocked ? Pens.LockedPen : Pens.DarkPen);
                    foreach (var focal in trait.Focals)
                    {
                        var focalPen = focal.IsUnit ? Pens.UnitPen : Pens.FocalPen;
                        DrawDirectedLine(focal.Segment, focalPen);
                        var offset = focal.Direction > 0 ? -5f : 5f;
                        if (Data.DisplayMode.HasFlag(DisplayMode.ShowLengths))
                        {
                            DrawText(focal.Segment.OffsetAlongLine(0.5f, offset), focal.TRatio.ToString("0.###"), Pens.LineTextPen);
                        }
                        if (Data.DisplayMode.HasFlag(DisplayMode.ShowSlugValues))
                        {
                            DrawText(focal.Segment.OffsetAlongLine(0f, offset), focal.StartT.ToString("0.###"), Pens.SlugTextPen);
                            DrawText(focal.Segment.OffsetAlongLine(1f, offset), focal.EndT.ToString("0.###"), Pens.SlugTextPen);
                        }
                        foreach (var bond in focal.SingleBonds)
                        {
                            DrawDirectedLine(bond.Segment, Pens.BondPen);
                            if (Data.DisplayMode.HasFlag(DisplayMode.ShowSlugValues))
                            {
                                var val = bond.TRatio;
                                var displayVal = val == float.MaxValue ? "max" : val.ToString("0.###");
                                DrawText(bond.Segment.OffsetAlongLine(0.6f, offset), displayVal, Pens.LineTextPen);
                            }
                        }
                    }
                }

                foreach (var doubleBond in pad.ElementsOfKind(ElementKind.DoubleBond))
                {
                    var db = (DoubleBond)doubleBond;
                    var pen = Data.Highlight.FirstElement.Key == db.Key ? Pens.BondSelectPen : Pens.BondFillPen;

                    SKPoint[] pts = new SKPoint[] { db.StartFocal.StartPosition, db.StartFocal.EndPosition, db.EndFocal.EndPosition, db.EndFocal.StartPosition };
                    DrawPath(pts, pen);
                }

                if (Data.WorkingPoints.Count > 0)
                {
                    DrawPath(Data.WorkingPoints.ToArray(), Pens.BondFillPen);
                }

                foreach (var selectedElement in Data.Selected.Elements)
                {
                    DrawElement(selectedElement, Pens.SelectedPen);
                }

            }
        }
        public virtual void EndDraw()
        {
            Canvas.Restore();
            if (ShowBitmap && Bitmap != null)
            {
                DrawBitmap(Bitmap);
            }
            Canvas = null;
            OnDrawingComplete();
        }
        protected void OnDrawingComplete()
        {
            DrawingComplete?.Invoke(this, EventArgs.Empty);
        }

        public void DrawElement(IElement element, SKPaint paint, float radius = 4f)
        {
            if (element is SegmentBase seg)
            {
                Canvas.DrawLine(seg.Segment.StartPoint, seg.Segment.EndPoint, paint);
            }
            else if (element is IPoint point)
            {
                Canvas.DrawCircle(point.Position.X, point.Position.Y, radius, paint);
            }
            else if (element is IAreaElement area)
            {
                Canvas.DrawPath(area.Path, paint);
            }
        }

        public abstract void DrawRoundBox(SKPoint point, SKPaint paint, float radius = 8f);
        public abstract void DrawPolyline(SKPoint[] polyline, SKPaint paint);
        public abstract void DrawPath(SKPoint[] polyline, SKPaint paint);
        public abstract void DrawDirectedLine(SKSegment seg, SKPaint paint);
        public abstract void DrawText(SKPoint center, string text, SKPaint paint);
        public abstract void DrawBitmap(SKBitmap bitmap);

        public abstract void GeneratePens();

        protected SKRect GetTextBackgroundSize(float x, float y, String text, SKPaint paint)
        {
            var fm = paint.FontMetrics;
            float halfTextLength = paint.MeasureText(text) / 2 + 4;
            return new SKRect((int)(x - halfTextLength), (int)(y + fm.Top + 3), (int)(x + halfTextLength), (int)(y + fm.Bottom - 1));
        }
        public SKBitmap GenerateBitmap(int width, int height)
        {
            Bitmap = new SKBitmap(width, height);
            return Bitmap;
        }
    }
}
