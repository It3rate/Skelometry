using System.Windows.Forms;
using Slugs.Renderer;

namespace Slugs.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SlugAgent : IAgent
    {
	    private SlugRenderer _renderer;

        public RenderStatus Status { get; }

        public SlugAgent(SlugRenderer renderer)
        {
	        _renderer = renderer;
        }

        public void Clear()
	    {
	    }

	    public void Draw()
	    {
		    _renderer.Draw();
	    }

	    public bool MouseDown(MouseEventArgs e)
	    {
		    return true;
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
		    return true;
        }

	    public bool MouseUp(MouseEventArgs e)
	    {
		    return true;
        }

	    public bool KeyDown(KeyEventArgs e)
	    {
		    return true;
        }

	    public bool KeyUp(KeyEventArgs e)
	    {
		    return true;
        }
    }
}
