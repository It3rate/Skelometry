using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MidpointConstraint : TwoElementConstraintBase
    {
	    public SegmentBase StartSegment => (SegmentBase)StartElement;

	    public MidpointConstraint(SegmentBase startElement, IElement endElement) : base(startElement, endElement) { }

	    public override void OnStartChanged()
	    {
		    if (EndElement is IPoint point)
		    {
			    point.Position = StartSegment.MidPoint;
		    }
		    else if (EndElement is IMidpointSettable ms)
		    {
			    ms.SetMidpoint(StartSegment.MidPoint);
            }
	    }
	    public override void OnEndChanged()
	    {
		    if (StartElement is IMidpointSettable ms)
		    {
                ms.SetMidpoint(EndElement.Center);
		    }
	    }
    }
}
