using Vis.Model.Agent;

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
	    public PadKind PadKind { get; set; } = PadKind.None;
	    public UIType DisplayType { get; set; } = UIType.None;
        public DisplayStyle DisplayStyle { get; set; } = DisplayStyle.None;
	    public DisplayState DisplayState { get; set; } = DisplayState.None;
	    public CorrelationState CorrelationState { get; set; } = CorrelationState.HasUnit;

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
        MeasureTick,
    }

    [Flags]
    public enum DisplayStyle
    {
	    None,
        Highlighting,
	    ShowTicks,
        ShowRuler,
	    ShowHotspots,
	    WillDelete,
    }

    public enum DisplayState
    {
        None,
        Hidden,
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
