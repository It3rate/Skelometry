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
    public class SkiaRenderer : IRenderer
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public SkiaRenderer(Control parent, int width = 250, int height = 250)
        {
            Width = width;
            Height = height;

            SKControl skControl = new SKControl();
            skControl.Width = parent.Width / 2;
            skControl.Height = parent.Height;
            skControl.PaintSurface += SkControl_PaintSurface;
            parent.Controls.Add(skControl);

            //GenPens(height * 4);
            //SKImageInfo imageInfo = new SKImageInfo(250, 250);
            //using (SKSurface surface = SKSurface.Create(imageInfo))
            //{
            //    SKCanvas canvas = surface.Canvas;
            //    canvas.Clear(SKColors.Red);
            //}
        }

        private void SkControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
        }
        public void TranslateContext(float x, float y)
        {
            //g.TranslateTransform(x, y);
        }

        public void BeginDraw(int unitPixels)
        {
        }
        public void EndDraw()
        {
        }
        public void Draw(IAgent agent, int penIndex = 0)
        {
        }

        public void SetGraphicsContext(object context)
        {
        }

        public void Invalidate()
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