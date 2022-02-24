using SkiaSharp;
using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PerpendicularConstraint : TwoElementConstraintBase
    {
	    public SegmentBase StartSegment => (SegmentBase)StartElement;
	    public SegmentBase EndSegment => (SegmentBase)EndElement;

        public PerpendicularConstraint(SegmentBase startElement, SegmentBase endElement) : base(startElement, endElement) { }


	    public override void OnStartChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
	    }
	    public override void OnEndChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
	    }
    }
}