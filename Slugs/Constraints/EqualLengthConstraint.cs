using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EqualLengthConstraint : TwoElementConstraintBase
    {
	    public SegmentBase StartSegment => (SegmentBase)StartElement;
	    public SegmentBase EndSegment => (SegmentBase)StartElement;

	    public EqualLengthConstraint(SegmentBase startElement, SegmentBase endElement) : base(startElement, endElement) { }


	    public override void OnStartChanged()
	    {
		    if (EndElement is IPoint point)
		    {
		    }
		    else if (EndElement is SegmentBase segment)
		    {
		    }
	    }
	    public override void OnEndChanged()
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
