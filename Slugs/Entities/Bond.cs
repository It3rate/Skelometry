
using System;
using System.Collections.Generic;
using SkiaSharp;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public class Bond : SegmentBase
    {
	    public override ElementKind ElementKind => ElementKind.Bond;
        public override IElement EmptyElement => Empty;
        public static readonly Bond Empty = new Bond();
	    private Bond() : base(true) {}// Empty ctor

        public BondPoint StartPoint => (BondPoint)Pad.PointAt(StartKey);
        public BondPoint EndPoint => (BondPoint)Pad.PointAt(EndKey);
        public override SKPoint StartPosition => StartPoint.Position;
        public override SKPoint EndPosition => EndPoint.Position;

        public override List<IPoint> Points => new List<IPoint>() {StartPoint, EndPoint};

	    public Bond(BondPoint startPoint, BondPoint endPoint) : base(startPoint.PadKind)
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

        public static bool operator ==(Bond left, Bond right) => 
		    left.Key == right.Key && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
	    public static bool operator !=(Bond left, Bond right) => 
		    left.Key != right.Key|| left.StartKey != right.StartKey || left.EndKey != right.EndKey;
	    public override bool Equals(object obj) => obj is Bond value && this == value;
	    public override int GetHashCode() => 13 * Key + 17 * Key + 27 * StartKey + 29 * EndKey;
    }
}
