namespace Slugs.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ElementRecord
    {
	    public int Index { get; }

	    //public IElement Selected { get; }

	    public ElementType ElementType { get; set; } = ElementType.None;
	    public ElementStyle ElementStyle { get; set; } = ElementStyle.None;
	    public ElementState ElementState { get; set; } = ElementState.None;
	    //public ElementLinkage ElementLinkage { get; set; } = ElementLinkage.HasUnit;

	    //public ElementRecord(IElement element, PadKind padKind, int index = -1)
	    //{
		   // Selected = element;
		   // PadKind = padKind;
		   // Index = index;
	    //}
    }

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
	    None = 0,
	    Highlighting = 1,
	    ShowTicks = 2,
	    ShowRuler = 4,
	    ShowHotspots = 8,
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
