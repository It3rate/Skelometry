namespace Vis.Model.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PadAttributes
    {
        public static readonly PadAttributes Empty = new PadAttributes();

	    public int Index { get; }
	    public UIType DisplayType { get; } = UIType.None;
	    public DisplayStyle DisplayStyle { get; } = DisplayStyle.None;
	    public DisplayState DisplayState { get; } = DisplayState.None;
	    public CorrelationState CorrelationState { get; } = CorrelationState.None;

	    public PadAttributes(int index = -1)
	    {
		    Index = index;
	    }
    }

    public class PadAttributes<T> : PadAttributes //where T : IPrimitive
    {
	    public T Element { get; }

	    public PadAttributes(T element, int index = -1) : base(index)
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
