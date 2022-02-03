using System.Windows.Forms;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Renderer;

namespace Slugs.Agents
{
    public interface IAgent
    {
        RenderStatus RenderStatus { get; }
        //Position this[IPoint point] { get; set; }

        Pad PadFor(PadKind padKind);

        //void UpdatePointRef(IPoint from, IPoint to);
        void Clear();

        bool MouseDown(MouseEventArgs e);
        bool MouseMove(MouseEventArgs e);
        bool MouseUp(MouseEventArgs e);
        bool KeyDown(KeyEventArgs e);
        bool KeyUp(KeyEventArgs e);
    }
}
