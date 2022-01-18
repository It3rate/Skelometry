using System.Windows.Forms;
using Slugs.Entities;
using Slugs.Renderer;

namespace Slugs.Agents
{
    public interface IAgent
    {
        RenderStatus RenderStatus { get; }
        //SKPoint this[IPoint point] { get; set; }

        Pad PadAt(int index);

        //void UpdatePointRef(IPoint from, IPoint to);
        void Clear();
        void Draw();

        bool MouseDown(MouseEventArgs e);
        bool MouseMove(MouseEventArgs e);
        bool MouseUp(MouseEventArgs e);
        bool KeyDown(KeyEventArgs e);
        bool KeyUp(KeyEventArgs e);
    }
}
