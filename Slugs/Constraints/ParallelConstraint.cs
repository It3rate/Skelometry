﻿using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ParallelConstraint : TwoElementConstraintBase
    {
	    public SegmentBase StartSegment => (SegmentBase)StartElement;
	    public SegmentBase EndSegment => (SegmentBase)EndElement;

	    public ParallelConstraint(SegmentBase startElement, SegmentBase endElement) : base(startElement, endElement) { }

	    public override void OnStartChanged()
	    {
	    }
	    public override void OnEndChanged()
	    {
	    }
    }
}
