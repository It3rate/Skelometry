using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LockConstraint : IConstraint
    {
	    public IElement StartElement { get; private set; }
	    public bool HasElement(int key) => StartElement.Key == key;

	    public ConstraintTarget ConstraintTarget => ConstraintTarget.Perpendicular;

	    public LockConstraint(SegmentBase start, SegmentBase end)
	    {
		    StartElement = start;
	    }

	    public void OnAddConstraint() { StartElement.IsLocked = true; }
	    public void OnRemoveConstraint() { StartElement.IsLocked = false; }

        public void OnElementChanged(IElement changedElement) { }
    }
}
