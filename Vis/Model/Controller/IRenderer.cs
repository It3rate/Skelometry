using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Agent;

namespace Vis.Model.Controller
{
    public interface IRenderer
    {
        IAgent Agent { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        Rectangle Bounds { get; }
        float UnitPixels { get; set; }
        int PenIndex { get; set; }

        void AttachPaintEvent();
        void Invalidate();
        void SetGraphicsContext(object context);
        void TranslateContext(float x, float y);
        void BeginDraw();
        void Draw();
        void EndDraw();
        event EventHandler DrawingComplete;
    }
}
