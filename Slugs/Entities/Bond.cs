
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

        public int StartPointKey { get; }
        public int EndPointKey { get; }

        public PointOnFocal StartPoint => (PointOnFocal)Pad.PointAt(StartPointKey);
        public PointOnFocal EndPoint => (PointOnFocal)Pad.PointAt(EndPointKey);
        public override SKPoint StartPosition => StartPoint.Position;
        public override SKPoint EndPosition => EndPoint.Position;

        public override List<IPoint> Points => new List<IPoint>() {StartPoint, EndPoint};

	    public Bond(PointOnFocal startPoint, PointOnFocal endPoint) : base(startPoint.PadKind)
	    {
		    StartPointKey = startPoint.Key;
		    EndPointKey = endPoint.Key;
	    }

	    public static bool operator ==(Bond left, Bond right) => 
		    left.Key == right.Key && left.StartPointKey == right.StartPointKey && left.EndPointKey == right.EndPointKey;
	    public static bool operator !=(Bond left, Bond right) => 
		    left.Key != right.Key|| left.StartPointKey != right.StartPointKey || left.EndPointKey != right.EndPointKey;
	    public override bool Equals(object obj) => obj is Bond value && this == value;
	    public override int GetHashCode() => 13 * Key + 17 * Key + 27 * StartPointKey + 29 * EndPointKey;
    }
}
