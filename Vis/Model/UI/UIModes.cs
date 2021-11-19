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
        None = 0,
        Line = 1,
        Polygon = 2,
        Circle = 3,
        Oval = 4,
        Curve = 5,
        Select = 6,
        MultiSelect = 7,
        SelectUnit = 8,
        Connect = 9,
        Disconnect = 10,
    }
    [Flags]
    public enum UIState
    {
	    None = 0,
        FocusPad = 1 << 0,
        ViewPad = 1 << 1,
        WorkingPad = 1 << 2,

        SnapToPoint = 1 << 3,
        SnapToPath = 1 << 4,
        SnapToPathExtend = 1 << 5,
        ConstrainAngle = 1 << 6,
    }

    [Flags]
    public enum UIDisplay
    {
        None = 0,
	    ShowFocusPad = 1 << 0,
	    ShowViewPad = 1 << 1,

	    ShowTicks = 1 << 2,
	    ShowValues = 1 << 3,
	    ShowGrid = 1 << 4,
	    ShowRulers = 1 << 5,

	    ShowDebugInfo = 1 << 6,

    }
}
