using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EqualLengthConstraint : ITwoElementConstraint
    {
	    public IElement StartElement { get; private set; }
	    public IElement EndElement { get; private set; }
	    public bool HasElement(int key) => StartElement.Key == key || EndElement.Key == key;

	    public SegmentBase StartSegment => (SegmentBase)StartElement;
	    public SegmentBase EndSegment => (SegmentBase)StartElement;

        public ConstraintTarget ConstraintTarget => ConstraintTarget.Length;

	    public EqualLengthConstraint(SegmentBase start, IElement end)
	    {
		    StartElement = start;
		    EndElement = end;
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
		    if (EndElement is IPoint point)
		    {
		    }
		    else if (EndElement is SegmentBase segment)
		    {
		    }
	    }
	    public void OnEndChanged()
	    {
		    if (EndElement is IPoint point)
		    {
		    }
		    else if (EndElement is SegmentBase segment)
		    {
		    }
	    }
    }
}
