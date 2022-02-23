using Slugs.Entities;
using Slugs.Primitives;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CollinearConstraint : TwoElementConstraintBase
    {
	    public SegmentBase SegmentElement => (SegmentBase)StartElement;

	    public CollinearConstraint(SegmentBase startElement, IElement endElement) : base(startElement, endElement) { }

	    public override void OnStartChanged()
	    {
		    if (EndElement is IPoint point)
		    {
			    point.Position = SegmentElement.ProjectPointOnto(point.Position);
            }
		    else if (EndElement is SegmentBase segment)
		    {
		    }
        }
	    public override void OnEndChanged()
	    {
		    if (EndElement is IPoint point)
		    {
			    point.Position = SegmentElement.ProjectPointOnto(point.Position);
		    }
		    else if (EndElement is SegmentBase segment)
		    {
		    }
        }
    }
}
