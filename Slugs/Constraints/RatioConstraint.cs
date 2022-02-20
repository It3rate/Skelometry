using Slugs.Entities;
using Slugs.Primitives;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // this probably includes directional locking (causation), limits, confidence, complexity etc
    // - lock points to a value (lock zeros on single bonds, focal start points, trait x positions etc)
    // - lock ratios on double bonds
    // - lock unit or focal lengths across traits
    // - lock trait angles (perpendicular, collinear) and display lengths/origins/ends to other traits. (traits are sort of view only, so meaning is less).
    // - perhaps merged points could just be equal constrained? (mechanically, virtually they already are this)
    public enum ConstraintTarget
    {
        None,
        Full,
        X,
        Y,
        T,
        Length,
        StartPoint,
        EndPoint,
        Parallel,
        Perpendicular,
        Tangent,
        Angle,
    }

    public class RatioConstraint : IConstraint
    {
	    public ISlugElement StartElement { get; private set; }
	    public ISlugElement EndElement { get; private set; }
        public Slug Ratio { get; set; }
        public ConstraintTarget ConstraintTarget{get; private set; }
        public bool IsLocked { get; set; } 

        public RatioConstraint(ISlugElement start, ISlugElement end)
	    {
		    StartElement = start;
		    EndElement = end;
        }

        public void OnStartChanged()
	    {
	    }
	    public void OnEndChanged()
	    {
	    }
    }
}
