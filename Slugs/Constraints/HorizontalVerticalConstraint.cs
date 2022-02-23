using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HorizontalVerticalConstraint : ConstraintBase
    {
	    public bool IsHorizontal { get; }
	    public bool IsVertical => !IsHorizontal;

        public SegmentBase StartSegment => (SegmentBase)StartElement;

	    public HorizontalVerticalConstraint(SegmentBase startElement, bool isHorizontal): base(startElement)
	    {
		    IsHorizontal = isHorizontal;
	    }

	    public override void OnElementChanged(IElement changedElement)
	    {
	    }
    }
}
