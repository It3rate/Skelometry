
using System;
using System.Collections.Generic;
using SkiaSharp;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
    // should bonds always be required to attach to endpoints of focals? Or at least almost always?
    // Yes because to have a gap is to say that part of a focal isn't accounted for.
    // No because there can be multiple ranges, and maybe some ranges are illegal.
    // So every part of a focal needs singleBond mapping, other than illegal areas
    // These areas can also overlap, so yellow and red apples could be similar size ranges, but green tend to be larger.
    // This means ranges matter, so multiplication bonds should be based on slugs, and only addition on single line maps.
    // These slugs are really just focals, so maybe to the first point, bonds only attach to endpoints of focals,
    // just there can be more than one focal per trait entity. (Focals are just slugs on a trait assuming trait has a zero point, which can be defined with a unit focal)
    // Single line bonds only map the zero points for things that can add or compare? eg X and Y axis meeting at zero (or is this defined by the unit focals?)
    // or are they intrinsic to the trait, like min/max and reasons for limits (complexity, measurable, changed, terminated, likelihood)
    // Actually X and Y are focal segments where the two zeros share a point. They can not be connected on the other points (independent), or as a triangle (area).

	public class SingleBond : SegmentBase //, ISlugElement // will be T:T or 0:0 on each line, but maybe that ratio is set already by the double bond?
    {
	    public override ElementKind ElementKind => ElementKind.SingleBond;
        public override IElement EmptyElement => Empty;
        public static readonly SingleBond Empty = new SingleBond();
	    private SingleBond() : base(true) {}// Empty ctor

        public BondPoint StartPoint => (BondPoint)Pad.PointAt(StartKey);
        public BondPoint EndPoint => (BondPoint)Pad.PointAt(EndKey);
        public BondPoint OtherPoint(int notKey) => (notKey == StartKey) ? EndPoint : (notKey == EndKey) ? StartPoint : BondPoint.Empty;
        public override SKPoint StartPosition => StartPoint.Position;
        public override SKPoint EndPosition => EndPoint.Position;

        public override List<IPoint> Points => new List<IPoint>() {StartPoint, EndPoint};

	    public SingleBond(BondPoint startPoint, BondPoint endPoint) : base(startPoint.PadKind)
	    {
		    SetStartKey(startPoint.Key);
		    SetEndKey(endPoint.Key);
        }

	    protected override void SetStartKey(int key)
	    {
		    if (Pad.ElementAt(key) is BondPoint)
		    {
			    base.SetStartKey(key);
		    }
		    else
		    {
			    throw new ArgumentException("Focal points must be FocalPoint.");
		    }
	    }
	    protected override void SetEndKey(int key)
	    {
		    if (Pad.ElementAt(key) is BondPoint)
		    {
			    base.SetEndKey(key);
		    }
		    else
		    {
			    throw new ArgumentException("Focal points must be FocalPoint.");
		    }
	    }

        public static bool operator ==(SingleBond left, SingleBond right) => 
		    left.Key == right.Key && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
	    public static bool operator !=(SingleBond left, SingleBond right) => 
		    left.Key != right.Key|| left.StartKey != right.StartKey || left.EndKey != right.EndKey;
	    public override bool Equals(object obj) => obj is SingleBond value && this == value;
	    public override int GetHashCode() => 13 * Key + 17 * Key + 27 * StartKey + 29 * EndKey;
    }
}
