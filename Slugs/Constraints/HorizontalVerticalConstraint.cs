  using SkiaSharp;
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

        public Trait StartTrait => (Trait)StartElement;

	    public HorizontalVerticalConstraint(Trait startElement, bool isHorizontal): base(startElement)
	    {
		    IsHorizontal = isHorizontal;
	    }

	    public override void OnElementChanged(IElement changedElement, Dictionary<int, SKPoint> adjustedElements)
	    {
		    var cp = (changedElement is Trait t ? (t.StartPoint.IsLocked ? t.EndPoint : t.StartPoint) : changedElement);
		    if (cp is IPoint changedPoint) // only need to adjust on point changes
		    {
			    var otherPoint = StartTrait.OtherPoint(changedPoint);
			    if (IsHorizontal)
			    {
				    otherPoint.Position = new SKPoint(otherPoint.Position.X, changedPoint.Position.Y);
			    }
			    else
			    {
				    otherPoint.Position = new SKPoint(changedPoint.Position.X, otherPoint.Position.Y);
			    }
			    changedPoint.Pad.UpdateConstraints(changedPoint, adjustedElements);
		    }
	    }
    }
}
