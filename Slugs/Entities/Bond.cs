
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

        public PointOnFocal StartPoint => (PointOnFocal)Pad.PointAt(StartKey);
        public PointOnFocal EndPoint => (PointOnFocal)Pad.PointAt(EndKey);
        public override SKPoint StartPosition => StartPoint.Position;
        public override SKPoint EndPosition => EndPoint.Position;

        public override List<IPoint> Points => new List<IPoint>() {StartPoint, EndPoint};

	    public Bond(PointOnFocal startPoint, PointOnFocal endPoint) : base(startPoint.PadKind)
	    {
		    SetStartKey(startPoint.Key);
		    SetEndKey(endPoint.Key);
        }

	    protected override void SetStartKey(int key)
	    {
		    if (Pad.ElementAt(key) is PointOnFocal)
		    {
			    base.SetStartKey(key);
		    }
		    else
		    {
			    throw new ArgumentException("Focal points must be PointOnTrait.");
		    }
	    }
	    protected override void SetEndKey(int key)
	    {
		    if (Pad.ElementAt(key) is PointOnFocal)
		    {
			    base.SetEndKey(key);
		    }
		    else
		    {
			    throw new ArgumentException("Focal points must be PointOnTrait.");
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
