using SkiaSharp;
using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LockConstraint : ConstraintBase
    {
	    public LockConstraint(IElement startElement) : base(startElement) { }

	    public override void OnAddConstraint() => StartElement.IsLocked = true; 
	    public override void OnRemoveConstraint() => StartElement.IsLocked = false;

        public override void OnElementChanged(IElement changedElement, Dictionary<int, SKPoint> adjustedElements)
        {
	        base.OnElementChanged(changedElement, adjustedElements);
        }
    }
}
