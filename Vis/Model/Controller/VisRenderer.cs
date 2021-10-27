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

    public class VisRenderer : Panel, IRenderer
    {
        //public int Width { get; set; }
        //public int Height { get; set; }
        private VisPens Pens { get; }

        public VisRenderer()
        {
            Width = 250;
            Height = 250;
            Pens = new VisPens(250 * 4);
        }
        public VisRenderer(int width, int height) : base()
        {
            Width = width;
            Height = height;
            Pens = new VisPens(height * 4);
        }

        private Graphics _graphics;
        private GraphicsState _graphicsState;
        public void SetGraphicsContext(object context) { _graphics = (Graphics)context; }
        public void TranslateContext(float x, float y)
        {
            _graphics.TranslateTransform(x, y);
        }

        public void BeginDraw(int unitPixels)
        {
            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _graphicsState = _graphics.Save();
            _graphics.TranslateTransform(10, 10);
            _graphics.ScaleTransform(unitPixels, unitPixels);

        }
        public void EndDraw()
        {
            _graphics.Restore(_graphicsState);
            _graphicsState = null;
            _graphics = null;
        }
        public void Draw(IAgent agent, int penIndex = 0)
        {
            //g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(-1f, 0), new PointF(1f, 0));
            //g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(0, -1f), new PointF(0, 1f));

            foreach (var prim in agent.FocusPad.Paths)
            {
                DrawPrimitive(prim, penIndex);
            }

            foreach (var path in agent.ViewPad.Paths)
            {
                DrawPath(path, 1);
            }

            foreach (var prim in agent.WorkingPad.Paths)
            {
                DrawPrimitive(prim, penIndex);
            }
        }
        public void DrawPrimitive(IPrimitive path, int penIndex = 0)
        {
            if (path is VisLine line)
            {
                DrawLine(line.StartPoint, line.EndPoint, penIndex);
            }
            else if (path is VisCircle circ)
            {
                DrawSpot(circ.Center, penIndex);
                DrawCircle(circ, penIndex);
            }
            else if (path is VisRectangle rect)
            {
                DrawRect(rect, penIndex);
            }
            else if (path is RenderPoint rp)
            {
                DrawSpot(rp, rp.PenIndex, rp.Scale);// penIndex);
            }
            else if (path is VisPoint p)
            {
                DrawSpot(p, 6);// penIndex);
            }
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
            _graphics.Flush();

            //foreach (var point in stroke.Anchors)
            //{
            //    DrawCircle(point, 0);
            //}

            //if (stroke.Edges.Count == 0)
            //{
            //    DrawLine(stroke.Start, stroke.EndPoint, penIndex);
            //}
            //else
            //{
            //    foreach (var edge in stroke.Edges)
            //    {
            //        DrawCircle(edge.Anchor0, 2, 0.5);
            //        DrawCircle(edge.Anchor1, 3, 0.5);
            //    }
            //    //DrawCurve(stroke.Start, stroke.Edges[0], stroke.EndPoint, penIndex);
            //    DrawCurve(stroke, penIndex);
            //}
        }
        public void DrawSpot(VisPoint pos, int penIndex = 0, float scale = 1f)
        {
            var r = Pens[penIndex].Width * scale;
            _graphics.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }

        public void DrawCircle(VisCircle circ, int penIndex = 0)
        {
            var pos = circ.Center;
            var r = circ.Radius;
            _graphics.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }
        public void DrawRect(VisRectangle rect, int penIndex = 0)
        {
            _graphics.DrawRectangle(Pens[penIndex], rect.TopLeft.X, rect.TopLeft.Y, rect.Size.X, rect.Size.Y);
        }

        public void DrawLine(VisLine line, int penIndex = 0)
        {
            _graphics.DrawLine(Pens[penIndex], line.StartPoint.X, line.StartPoint.Y, line.EndPoint.X, line.EndPoint.Y);
        }
        public void DrawLine(VisPoint p0, VisPoint p1, int penIndex = 0)
        {
            _graphics.DrawLine(Pens[penIndex], p0.X, p0.Y, p1.X, p1.Y);
        }
        public void DrawPolyline(VisPoint[] points, int penIndex = 0)
        {
            _graphics.DrawLines(Pens[penIndex], ToPointF(points));
        }

        private PointF[] ToPointF(VisPoint[] points)
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
