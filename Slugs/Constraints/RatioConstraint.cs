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

    public class RatioConstraint : TwoElementConstraintBase
    {
	    public ConstraintTarget ConstraintTarget { get; }

        private Slug _ratio;
        public Slug Ratio { get => _ratio; set => _ratio = IsRatioLocked ? _ratio : value; }
        public bool IsRatioLocked { get; set; } 

        public RatioConstraint(IElement startElement, IElement endElement, ConstraintTarget constraintTarget, Slug ratio) : base(startElement, endElement)
        {
		    ConstraintTarget = constraintTarget;
            _ratio = ratio;
        }

        public override void OnStartChanged(Dictionary<int, SKPoint> adjustedElements)
        {
	        if (ConstraintTarget == ConstraintTarget.T && StartElement is ITValue start && EndElement is ITValue end)
	        {
		        end.T = start.T * (float)_ratio.DirectedLength();
		        EndElement.Pad.UpdateConstraints(EndElement, adjustedElements);
            }
        }
	    public override void OnEndChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    if (ConstraintTarget == ConstraintTarget.T && StartElement is ITValue start && EndElement is ITValue end)
		    {
			    var dl = (float) _ratio.DirectedLength();
			    start.T = (dl == 0) ? float.MaxValue : end.T / (float)_ratio.DirectedLength();
			    StartElement.Pad.UpdateConstraints(StartElement, adjustedElements);
            }
        }
    }
}
