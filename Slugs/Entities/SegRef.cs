using System;
using SkiaSharp;
using Slugs.Extensions;
using Slugs.Slugs;

namespace Slugs.Entities
{
	public class SegRef : IEquatable<SegRef>
    {
	    public static SegRef Empty = new SegRef(VPoint.Empty);
	    public bool IsEmpty => Start.IsEmpty && End.IsEmpty;

	    public IPoint Start { get; private set; }
	    public IPoint End { get; private set; }
	    private PointKind Kind { get; set; } = PointKind.Dirty;

        private SKSegment _cachedSeg;
        public SKSegment Segment
        {
	        get
	        {
		        return new SKSegment(Start.SKPoint, End.SKPoint);

          //      if (Kind.NeedsUpdate())
		        //{
			       // _cachedSeg = new SKSegment(Start.SKPoint, End.SKPoint);
			       // Kind = PointKind.Terminal; //(Kind == PointKind.Dirty) ? PointKind.Cached : Kind;
		        //}
		        //return _cachedSeg;
	        }
        }
        public SKPoint StartPoint
        {
	        get => Start.SKPoint;
	        set => Start.SKPoint = value;
        }
        public SKPoint EndPoint
        {
	        get => End.SKPoint;
	        set => End.SKPoint = value;
        }

        public SegRef(IPoint start)
	    {
		    Start = start;
		    End = start;
	    }
	    public SegRef(IPoint start, IPoint end)
	    {
		    Start = start;
		    End = end;
        }

	    public static SegRef operator +(SegRef a, float value)
	    {
		    a.StartPoint = a.StartPoint.Add(value);
		    a.EndPoint = a.EndPoint.Add(value);
		    return a.Clone();
	    }
	    public static SegRef operator -(SegRef a, float value)
	    {
		    a.StartPoint = a.StartPoint.Subtract(value);
		    a.EndPoint = a.EndPoint.Subtract(value);
		    return a.Clone();
	    }
	    public static SegRef operator *(SegRef a, float value)
	    {
		    a.StartPoint = a.StartPoint.Multiply(value);
		    a.EndPoint = a.EndPoint.Multiply(value);
		    return a.Clone();
	    }
	    public static SegRef operator /(SegRef a, float value)
	    {
		    a.StartPoint = a.StartPoint.Divide(value);
		    a.EndPoint = a.EndPoint.Divide(value);
		    return a.Clone();
	    }

        public SegRef Clone() => new SegRef(Start, End);
	    public float Length() => Segment.Length;
	    public float SquaredLength() => Segment.LengthSquared;
	    public SKPoint PointAlongLine(float t) => Segment.PointAlongLine(t);
	    public SKPoint SKPointFromStart(float dist) => Segment.PointAlongLine(dist);
	    public SKPoint SKPointFromEnd(float dist) => Segment.SKPointFromEnd(dist);
	    public SKPoint OrthogonalPoint(SKPoint pt, float offset) => Segment.OrthogonalPoint(pt, offset);
	    public SKPoint ProjectPointOnto(SKPoint p) => Segment.ProjectPointOnto(p);
	    public float TFromPoint(SKPoint point) => Segment.TFromPoint(point);
	    public SKPoint[] EndArrow(float dist = 8f) => Segment.EndArrow(dist);

        //public IPoint GetVirtualPointFor(SKPoint point)
        //{
        // var t = TFromPoint(point);
        // return new VirtualPoint(this, t);
        //}

        public static bool operator ==(SegRef left, SegRef right) =>
	        left.Start == right.Start && left.End == right.End;

        public static bool operator !=(SegRef left, SegRef right) =>
	        left.Start != right.Start || left.End != right.End;

        public override bool Equals(object obj) => obj is SegRef value && this == value;

        public bool Equals(SegRef value) =>
	        Start.Equals(value.Start) && End.Equals(value.End);

        public override int GetHashCode() =>
	        17 * 23 + Start.GetHashCode() * 29 + End.GetHashCode() * 31;

    }
}
