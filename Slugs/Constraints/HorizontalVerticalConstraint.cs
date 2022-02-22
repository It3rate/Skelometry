using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HorizontalVerticalConstraint : ITwoElementConstraint
    {
	    public IElement StartElement { get; private set; }
	    public IElement EndElement { get; private set; }
	    public bool HasElement(int key) => StartElement.Key == key || EndElement.Key == key;

	    public bool IsHorizontal { get; }
	    public bool IsVertical => !IsHorizontal;

        public SegmentBase StartSegment => (SegmentBase)StartElement;
	    public SegmentBase EndSegment => (SegmentBase)EndElement;

	    public ConstraintTarget ConstraintTarget => IsHorizontal ? ConstraintTarget.Horizontal : ConstraintTarget.Vertical;

	    public HorizontalVerticalConstraint(SegmentBase start, SegmentBase end, bool isHorizontal)
	    {
		    StartElement = start;
		    EndElement = end;
		    IsHorizontal = isHorizontal;
	    }

	    public void OnAddConstraint() { }
	    public void OnRemoveConstraint() { }

	    public void OnElementChanged(IElement changedElement)
	    {
		    if (changedElement.Key == StartElement.Key)
		    {
			    OnStartChanged();
		    }
		    else if (changedElement.Key == EndElement.Key)
		    {
			    OnEndChanged();
		    }
	    }

	    public void OnStartChanged()
	    {
	    }
	    public void OnEndChanged()
	    {
	    }
    }
}
