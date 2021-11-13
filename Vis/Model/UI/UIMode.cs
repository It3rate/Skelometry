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
    [Flags]
    public enum UIState
    {
	    None = 0,
        FocusPad,
        ViewPad,
        WorkingPad,

        SnapToPoint,
        SnapToPath,
        SnapToPathExtend,
        ConstrainAngle,

        ShowTicks,
        ShowValues,
        ShowDebugInfo,
    }
}
