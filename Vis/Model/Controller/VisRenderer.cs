using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Agent;
using Vis.Model.Primitives;

namespace Vis.Model.Controller
{
    public class VisRenderer
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public VisRenderer(int width = 250, int height = 250)
        {
            Width = width;
            Height = height;
            GenPens(height * 4);
        }
        public void Draw(Graphics g, IAgent agent, int penIndex = 0)
        {
            //g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(-1f, 0), new PointF(1f, 0));
            //g.DrawLine(Pens[(int)PenTypes.LightGray], new PointF(0, -1f), new PointF(0, 1f));

            foreach (var prim in agent.WorkingPad.Paths)
            {
                DrawPrimitive(g, prim, penIndex);
            }
            foreach (var prim in agent.FocusPad.Paths)
            {
                DrawPrimitive(g, prim, penIndex);
            }

            foreach (var path in agent.ViewPad.Paths)
            {
                DrawPath(g, path, 1);
            }
        }
        public void DrawPrimitive(Graphics g, IPrimitive path, int penIndex = 0)
        {
            if (path is VisLine line)
            {
                DrawLine(g, line.StartPoint, line.EndPoint, penIndex);
            }
            else if (path is VisCircle circ)
            {
                DrawSpot(g, circ.Center, penIndex);
                DrawCircle(g, circ, penIndex);
            }
            else if (path is VisRectangle rect)
            {
                DrawRect(g, rect, penIndex);
            }
            else if (path is VisPoint p)
            {
                DrawSpot(g, p, 6);// penIndex);
            }
        }
        public void DrawShape(Graphics g, VisStroke shape)
        {
            //foreach (var stroke in shape.Strokes)
            //{
            //    DrawStroke(g, stroke, (int)shape.StructuralType);
            //}
        }

        public void DrawPath(Graphics g, VisStroke stroke, int penIndex = 0)
        {
            foreach (var segment in stroke.Segments)
            {
                if (segment is VisLine line)
                {
                    DrawLine(g, line, penIndex);
                }
                else if (segment is VisArc arc)
                {
                    DrawPolyline(g, arc.GetPolylinePoints(), penIndex);
                }
            }
            g.Flush();

            //foreach (var point in stroke.Anchors)
            //{
            //    DrawCircle(g, point, 0);
            //}

            //if (stroke.Edges.Count == 0)
            //{
            //    DrawLine(g, stroke.Start, stroke.EndPoint, penIndex);
            //}
            //else
            //{
            //    foreach (var edge in stroke.Edges)
            //    {
            //        DrawCircle(g, edge.Anchor0, 2, 0.5);
            //        DrawCircle(g, edge.Anchor1, 3, 0.5);
            //    }
            //    //DrawCurve(g, stroke.Start, stroke.Edges[0], stroke.EndPoint, penIndex);
            //    DrawCurve(g, stroke, penIndex);
            //}
        }
        public void DrawSpot(Graphics g, VisPoint pos, int penIndex = 0, float scale = 1f)
        {
            var r = Pens[penIndex].Width * scale;
            g.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }

        public void DrawCircle(Graphics g, VisCircle circ, int penIndex = 0)
        {
            var pos = circ.Center;
            var r = circ.Radius;
            g.DrawEllipse(Pens[penIndex], pos.X - r, pos.Y - r, r * 2f, r * 2f);
        }
        public void DrawRect(Graphics g, VisRectangle rect, int penIndex = 0)
        {
            g.DrawRectangle(Pens[penIndex], rect.TopLeft.X, rect.TopLeft.Y, rect.Size.X, rect.Size.Y);
        }

        public void DrawLine(Graphics g, VisLine line, int penIndex = 0)
        {
            g.DrawLine(Pens[penIndex], line.StartPoint.X, line.StartPoint.Y, line.EndPoint.X, line.EndPoint.Y);
        }
        public void DrawLine(Graphics g, VisPoint p0, VisPoint p1, int penIndex = 0)
        {
            g.DrawLine(Pens[penIndex], p0.X, p0.Y, p1.X, p1.Y);
        }
        public void DrawPolyline(Graphics g, VisPoint[] points, int penIndex = 0)
        {
            g.DrawLines(Pens[penIndex], ToPointF(points));
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
        public List<Pen> Pens = new List<Pen>();
        private enum PenTypes
        {
            LightGray,
            Black,
            DarkRed,
            Orange,
            DarkGreen,
            DarkBlue,
            DarkViolet,
        }
        private void GenPens(float scale)
        {
            Pens.Clear();
            Pens.Add(GetPen(Color.LightGray, 8f / scale));
            Pens.Add(GetPen(Color.Black, 8f / scale));
            Pens.Add(GetPen(Color.DarkRed, 8f / scale));
            Pens.Add(GetPen(Color.Orange, 8f / scale));
            Pens.Add(GetPen(Color.DarkGreen, 8f / scale));
            Pens.Add(GetPen(Color.DarkBlue, 8f / scale));
            Pens.Add(GetPen(Color.DarkViolet, 16f / scale));
        }
        private Pen GetPen(Color color, float width)
        {
            var pen = new Pen(color, width);
            pen.LineJoin = LineJoin.Round;
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
    }
}
