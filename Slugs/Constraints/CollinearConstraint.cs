using SkiaSharp;
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
	    public CollinearConstraint(IElement startElement, IElement endElement) : base(startElement, endElement)
	    {
		    if (!(startElement is Trait || endElement is Trait))
		    {
                throw new ArgumentException("Collinear Constraint needs at least one segment.");
		    }
	    }

	    public override void OnStartChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
            Adjust(StartElement, EndElement, adjustedElements);
        }
	    public override void OnEndChanged(Dictionary<int, SKPoint> adjustedElements)
        {
	        Adjust(EndElement, StartElement, adjustedElements);
        }

	    private void Adjust(IElement moved, IElement affected, Dictionary<int, SKPoint> adjustedElements)
	    {
		    if (moved is Trait movedTrait)
		    {
			    if (affected is IPoint point)
			    {
				    point.Position = movedTrait.ProjectPointOnto(point.Position, false);
				    point.Pad.UpdateConstraints(point, adjustedElements);
			    }
			    else if (affected is Trait affectedTrait)
			    {
				    affectedTrait.StartPoint.Position = movedTrait.ProjectPointOnto(affectedTrait.StartPosition, false);
				    affectedTrait.EndPoint.Position = movedTrait.ProjectPointOnto(affectedTrait.EndPosition, false);
				    affectedTrait.Pad.UpdateConstraints(affectedTrait, adjustedElements);
			    }
		    }
		    else if (moved is IPoint movedPoint)
		    {
			    if (affected is Trait affectedTrait)
			    {
				    var target = affectedTrait.ProjectPointOnto(movedPoint.Position, false);
				    var offset = movedPoint.Position - target;

                    affectedTrait.StartPoint.Position = affectedTrait.StartPoint.Position + offset;
				    affectedTrait.EndPoint.Position = affectedTrait.EndPoint.Position + offset;
				    affectedTrait.Pad.UpdateConstraints(affectedTrait, adjustedElements);
			    }
		    }
	    }
    }
}
