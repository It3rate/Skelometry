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
        FocusPad = 1,
        ViewPad = 2,
        WorkingPad = 4,

        SnapToPoint = 8,
        SnapToPath = 16,
        SnapToPathExtend = 32,
        ConstrainAngle = 64,

        ShowTicks = 128,
        ShowValues = 256,
        ShowDebugInfo = 512,
    }
}
