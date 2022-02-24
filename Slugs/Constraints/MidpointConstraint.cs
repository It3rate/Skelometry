using SkiaSharp;
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

	    public override void OnStartChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    if (EndElement is IPoint point)
		    {
			    point.Position = StartSegment.MidPoint;
			    point.Pad.UpdateConstraints(point, adjustedElements);
            }
		    else if (EndElement is IMidpointSettable ms)
		    {
			    ms.SetMidpoint(StartSegment.MidPoint);
			    EndElement.Pad.UpdateConstraints(EndElement, adjustedElements);
            }
	    }
	    public override void OnEndChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    if (StartElement is IMidpointSettable ms)
		    {
                ms.SetMidpoint(EndElement.Center);
                StartElement.Pad.UpdateConstraints(StartElement, adjustedElements);
            }
	    }
    }
}
