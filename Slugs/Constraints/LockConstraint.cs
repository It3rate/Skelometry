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
        public bool LockValue { get; }

        public LockConstraint(IElement startElement, bool lockValue) : base(startElement)
        {
	        LockValue = lockValue;
        }

        public override void OnElementChanged(IElement changedElement, Dictionary<int, SKPoint> adjustedElements)
        {
	        base.OnElementChanged(changedElement, adjustedElements);
	        StartElement.IsLocked = LockValue;
	        if (!LockValue)
	        {
		        StartElement.Pad.UpdateConstraints(StartElement, adjustedElements);
	        }
        }
    }
}
