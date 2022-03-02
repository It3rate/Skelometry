using SkiaSharp;
using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PerpendicularConstraint : EqualConstraint
    {
        public PerpendicularConstraint(Trait startElement, Trait endElement, LengthLock lengthLock) : 
	        base(startElement, endElement, lengthLock, DirectionLock.Perpendicular) { }
    }
}