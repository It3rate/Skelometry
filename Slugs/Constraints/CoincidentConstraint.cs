using SkiaSharp;
using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CoincidentConstraint : TwoElementConstraintBase
    {
	    public IPoint StartPoint => (IPoint)StartElement;
	    public IPoint EndPoint => (IPoint)EndElement;

        public CoincidentConstraint(IPoint startElement, IPoint endElement) : base(startElement, endElement) { }

	    public override void OnStartChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    EndPoint.Position = StartPoint.Position;
            EndPoint.Pad.UpdateConstraints(EndPoint, adjustedElements);
	    }
	    public override void OnEndChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    StartPoint.Position = EndPoint.Position;
		    StartPoint.Pad.UpdateConstraints(StartPoint, adjustedElements);
        }
    }
}
