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
		    base.OnElementChanged(changedElement, adjustedElements);
		    IPoint changedPoint = changedElement is IPoint ? (IPoint)changedElement : null;
            // at first pass start and end may not be linear, and passed element may be trait, so straighten from center
            if (changedElement is Trait trait)
		    {
			    if (trait.StartPoint.IsLocked && !trait.EndPoint.IsLocked)
			    {
				    changedElement = trait.StartPoint; // if start is locked, can only adjust with end, so act like start has moved
			    }
			    else if (!trait.StartPoint.IsLocked && trait.EndPoint.IsLocked)
			    {
				    changedElement = trait.EndPoint;
			    }
			    else if (!trait.StartPoint.IsLocked && !trait.EndPoint.IsLocked)
			    {
				    var len = trait.Length;
				    var halfLen = len / 2f;
                    var sp = trait.StartPoint.Position;
                    var mp = trait.MidPosition;
				    if (IsHorizontal)
				    {
					    if (sp.X < mp.X)
					    {
						    trait.StartPoint.Position = new SKPoint(mp.X - halfLen, mp.Y);
						    trait.EndPoint.Position = new SKPoint(mp.X + halfLen, mp.Y);
                        }
					    else
					    {
						    trait.StartPoint.Position = new SKPoint(mp.X + halfLen, mp.Y);
						    trait.EndPoint.Position = new SKPoint(mp.X - halfLen, mp.Y);
                        }
				    }
				    else
				    {
					    if (sp.Y < mp.Y)
                        {
						    trait.StartPoint.Position = new SKPoint(mp.X, mp.Y - halfLen);
						    trait.EndPoint.Position = new SKPoint(mp.X, mp.Y + halfLen);
					    }
					    else
					    {
						    trait.StartPoint.Position = new SKPoint(mp.X, mp.Y + halfLen);
						    trait.EndPoint.Position = new SKPoint(mp.X, mp.Y - halfLen);
					    }
                    }
			    }
		    }

            // need to adjust the other point when dragging a point.
		    if (changedPoint != null)
		    {
			    var len = StartTrait.Length;
                var otherPoint = StartTrait.OtherPoint(changedPoint);
			    if (IsHorizontal)
			    {
				    var offset = changedPoint.Position.X < otherPoint.Position.X ? len : -len;
				    otherPoint.Position = new SKPoint(changedPoint.Position.X + offset, changedPoint.Position.Y);
			    }
			    else
			    {
				    var offset = changedPoint.Position.Y < otherPoint.Position.Y ? len : -len;
				    otherPoint.Position = new SKPoint(changedPoint.Position.X, changedPoint.Position.Y + offset);
			    }
			    changedPoint.Pad.UpdateConstraints(changedPoint, adjustedElements);
		    }
	    }
    }
}
