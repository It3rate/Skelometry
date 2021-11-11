using System.Windows.Forms;
using Vis.Model.Agent;
using Vis.Model.Primitives;

namespace Vis.Model.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // Modes for UI

    // Keys
    //  Mode, 
    //  snap, limit direction/len, multiselect, disconnect
    //  choice of multiple overlaps in node
    //  add constraint
    //  extend vs rotate

    // Mouse move
    //  Hover over paths, nodes; highlight
    //  info popups on hover
    //  highlight unit

    // Mouse down,
    //  Select path, node
    //  Start creating path
    //  Drag selected path, node
    //  Snap to angle, unit length

    // Mouse Drag
    //  Creating path
    //  Move selected path, node, endpoint
    //  Join elements with constraint
    //  Delete

    // Mouse Up
    //  Finalize command/selection
    //  Add info to multi step command


    public enum UIMode
    {
        None,
        Line,
        Polygon,
        Circle,
        Oval,
        Curve,
        Select,
        MultiSelect,
        SelectUnit,
        Connect,
        Disconnect,
    }

    public class UIStatus
    {
	    public UIMode UIMode { get; set; }
        public Keys CurrentKey { get; set; }
	    public MouseButtons CurrentMouse { get; set; }
        public PadKind CurrentPadKind { get; set; }
        public VisPoint Position { get; } = new VisPoint(0, 0);
        public VisPoint PreviousPosition { get; } = new VisPoint(0, 0);
    }

    public class UIModeData
    {
	    public UIMode UIMode { get; }
	    public Keys ActivationKey { get; }
	    public bool IsActive { get; private set; }
	    public PadKind[] AllowedPadKinds { get; }

        public bool CanActivate(UIStatus status)
        {
	        return true;
        }
    }

}
