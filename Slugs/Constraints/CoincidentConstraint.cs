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

	    public override void OnStartChanged()
	    {
		    EndPoint.Position = StartPoint.Position;
	    }
	    public override void OnEndChanged()
	    {
		    StartPoint.Position = EndPoint.Position;
        }
    }
}
