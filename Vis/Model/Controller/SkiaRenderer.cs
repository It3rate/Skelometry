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
    public class SkiaRenderer : SKControl, IRenderer
    {
	    public event EventHandler DrawingComplete;
	    public IAgent Agent { get; set; }
	    public int PenIndex { get; set; }
	    private float _unitPixels;
	    public float UnitPixels
	    {
		    get => _unitPixels;
		    set
		    {
			    _unitPixels = value;
			    Pens = new SkiaPens(value * 4);
		    }
	    }

	    private SkiaPens Pens { get; set; }

        public SkiaRenderer()
        {
        }

        public SkiaRenderer(Control parent, int width = -1, int height = -1)
        {
            Width = width == -1 ? parent.Width : width;
            Height = height == -1 ? parent.Height : height;

            this.PaintSurface += SkiaRenderer_PaintSurface;
            parent.Controls.Add(this);
            Pens = new SkiaPens(250 * 4);
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
        public void BeginDraw()
        {
	        _canvas.Save();
            _canvas.Scale(UnitPixels, UnitPixels);
	        _canvas.Clear(SKColors.WhiteSmoke); 
        }
        public void Draw()
        {
	        foreach (var prim in Agent.FocusPad.Paths)
	        {
		        DrawPrimitive(prim, PenIndex);
	        }

	        foreach (var path in Agent.ViewPad.Paths)
	        {
		        DrawPath(path, 1);
	        }

	        foreach (var prim in Agent.WorkingPad.Paths)
	        {
		        DrawPrimitive(prim, PenIndex);
	        }
        }
        public void EndDraw()
        {
            _canvas.Restore();
            _canvas = null;
            DrawingComplete?.Invoke(this, EventArgs.Empty);
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


        public void Flush()
        {
            _canvas.Flush();
        }

        public void DrawSpot(VisPoint pos, int penIndex = 0, float scale = 1f)
        {
	        var r = Pens[penIndex].StrokeWidth * scale;
            _canvas.DrawCircle(pos.X, pos.Y, r, Pens[penIndex]);
        }
        public void DrawCircle(VisCircle circ, int penIndex = 0)
        {
            _canvas.DrawCircle(circ.Center.X, circ.Center.Y, circ.Radius, Pens[penIndex]);
        }

        public void DrawRect(VisRectangle rect, int penIndex = 0)
        {
            _canvas.DrawRect(new SKRect(rect.X, rect.Y, rect.X + rect.Size.X, rect.Y + rect.Size.Y), Pens[penIndex]);
        }

        public void DrawLine(VisLine line, int penIndex = 0)
        {
            _canvas.DrawLine(line.X, line.Y, line.EndPoint.X, line.EndPoint.Y, Pens[penIndex]);
        }

        public void DrawLine(VisPoint p0, VisPoint p1, int penIndex = 0)
        {
	        _canvas.DrawLine(p0.X, p0.Y, p1.X, p1.Y, Pens[penIndex]);
        }

        public void DrawPolyline(VisPoint[] points, int penIndex = 0)
        {
            _canvas.DrawPoints(SKPointMode.Lines, ToPoints(points), Pens[penIndex]);
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