using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Agent;

namespace Vis.Model.Controller
{
    public interface IRenderer
    {
        int Width { get; set; }
        int Height { get; set; }

        void Invalidate();
        void SetGraphicsContext(object context);
        void TranslateContext(float x, float y);
        void BeginDraw(int unitPixels);
        void Draw(IAgent agent, int penIndex = 0);
        void EndDraw();
    }
}
