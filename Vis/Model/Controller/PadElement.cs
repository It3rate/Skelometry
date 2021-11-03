namespace Vis.Model.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PadElement<T> //where T : IPrimitive
    {
        public int Index { get; } // may make a double for concurrent editing

	    public T Element { get; }
	    public UIType DisplayType { get; }
        public DisplayStyle DisplayStyle { get; }
        public DisplayState DisplayState { get; }
	    public CorrelationState CorrelationState { get; }

	    public PadElement(T element)
	    {
		    Element = element;
	    }
    }

    public enum UIType
    {
        None,
        Node,
        Joint,
        Edge,
        HighlightSpot,
        HighlightPath,
        Measure,
    }

    [Flags]
    public enum DisplayStyle
    {
	    None,
	    ShowTicks,
	    ShowRuler,
	    ShowHotspots,
	    WillDelete,
    }

    public enum DisplayState
    {
        None,
        Hidden,
        Hovering,
        Selected,
        Dragging,
        ConnectedToHovered,
        ConnectedToSelected,
    }

    public enum CorrelationState
    {
        None,
        IsUnit,
        HasUnit,
        SharedJoint,
    }
}
