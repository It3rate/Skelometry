using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Vis.Model.Agent;

namespace Vis.Model.Controller
{
    public class SkiaRenderer : SKControl, IRenderer
    {
	    public event EventHandler DrawingComplete;
	    public IAgent Agent { get; set; }
        public float UnitPixels { get; set; }
	    public int PenIndex { get; set; }

        public SkiaRenderer()
        {
        }

        public SkiaRenderer(Control parent, int width = -1, int height = -1)
        {
            Width = width == -1 ? parent.Width : width;
            Height = height == -1 ? parent.Height : height;

            //_skControl = new SKControl();
            //_skControl.Width = Width;
            //_skControl.Height = Height;
            this.PaintSurface += SkiaRenderer_PaintSurface;
            parent.Controls.Add(this);
            //this.Invalidate();

            //GenPens(height * 4);
            //SKImageInfo imageInfo = new SKImageInfo(250, 250);
            //using (SKSurface surface = SKSurface.Create(imageInfo))
            //{
            //    SKCanvas canvas = surface.Canvas;
            //    canvas.Clear(SKColors.Red);
            //}
        }

        private void SkiaRenderer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
	        if (Agent != null)
	        {
				SetGraphicsContext(e.Surface);
		        BeginDraw();
		        Draw();
		        EndDraw();
	        }
        }


        public void AttachPaintEvent()
        {
        }

        public void TranslateContext(float x, float y)
        {
            //g.TranslateTransform(x, y);
        }

        private SKSurface _surface;
        public void SetGraphicsContext(object context) { _surface = (SKSurface)context; }
        public void BeginDraw()
        {
	        _surface.Canvas.Clear(SKColors.Blue);
        }
        public void EndDraw()
        {
	        DrawingComplete?.Invoke(this, EventArgs.Empty);
        }

        public void Draw()
        {
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