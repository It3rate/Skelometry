using System.Windows.Forms;
using SkiaSharp;
using Slugs.Renderer;
using Slugs.Slugs;

namespace Slugs.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EntityAgent : IAgent
    {
	    public RenderStatus RenderStatus { get; }
	    public SKPoint this[IPointRef pointRef]
	    {
		    get => throw new NotImplementedException();
		    set => throw new NotImplementedException();
	    }

	    public void UpdatePointRef(IPointRef @from, IPointRef to)
	    {
		    throw new NotImplementedException();
	    }

	    public void Clear()
	    {
		    throw new NotImplementedException();
	    }

	    public void Draw()
	    {
		    throw new NotImplementedException();
	    }

	    public bool MouseDown(MouseEventArgs e)
	    {
		    throw new NotImplementedException();
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
		    throw new NotImplementedException();
	    }

	    public bool MouseUp(MouseEventArgs e)
	    {
		    throw new NotImplementedException();
	    }

	    public bool KeyDown(KeyEventArgs e)
	    {
		    throw new NotImplementedException();
	    }

	    public bool KeyUp(KeyEventArgs e)
	    {
		    throw new NotImplementedException();
	    }
    }
}
