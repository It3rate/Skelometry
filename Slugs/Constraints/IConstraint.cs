using SkiaSharp;
using Slugs.Entities;

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

    public interface IConstraint
    {
	    int Key { get; }
        IElement StartElement { get; }
        bool HasElement(int key);
        List<int> AffectedKeys { get; }

	    void OnAddConstraint();
	    void OnRemoveConstraint();
        void OnElementChanged(IElement changedElement, Dictionary<int, SKPoint> _changes);
    }

    public interface ITwoElementConstraint : IConstraint
    {
	    IElement EndElement { get; }

	    IElement OtherElement(int originalKey);
	    void OnStartChanged(Dictionary<int, SKPoint> adjustedElements);
	    void OnEndChanged(Dictionary<int, SKPoint> adjustedElements);
    }

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
	    Collinear,
        Parallel,
        Perpendicular,
        Horizontal,
        Vertical,
        Tangent,
	    Angle,
    }

}
