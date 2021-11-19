namespace Vis.Model.Render
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum ElementState
    {
	    None,
	    Hidden,
	    Selected,
	    Dragging,
	    ConnectedToHovered,
	    ConnectedToSelected,
    }

    [Flags]
    public enum ElementStyle
    {
	    None,
	    Highlighting,
	    ShowTicks,
	    ShowRuler,
	    ShowHotspots,
    }


    public enum ElementLinkage
    {
	    None,
	    IsUnit,
	    HasUnit,
	    SharedJoint,
    }
    public enum ElementType
    {
	    None,
	    Node,
	    Joint,
	    Edge,
	    HighlightSpot,
	    HighlightPath,
	    MeasureTick,
    }
}
