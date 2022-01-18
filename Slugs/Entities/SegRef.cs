using System;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Extensions;
using Slugs.Slugs;

namespace Slugs.Entities
{
	public class SegRef : IEquatable<SegRef>
    {
	    public static SegRef Empty = new SegRef(Point.Empty);
	    public bool IsEmpty => StartKey == -1 || EndKey == -1;

	    public int StartKey { get; private set; }
	    public int EndKey { get; private set; }
	    private PointKind Kind { get; set; } = PointKind.Terminal;

	    public IPoint StartRef => Agent.Current.PointAt(StartKey);
	    public IPoint EndRef => Agent.Current.PointAt(EndKey);

        public SKPoint StartPoint
        {
	        get => StartRef.SKPoint;
	        set => StartRef.SKPoint = value;
        }
        public SKPoint EndPoint
        {
	        get => EndRef.SKPoint;
	        set => EndRef.SKPoint = value;
        }
        public SKSegment Segment => new SKSegment(StartPoint, EndPoint);

        public SegRef(IPoint start)
	    {
		    StartKey = start.Key;
		    EndKey = start.Key;
        }
        public SegRef(IPoint start, IPoint end)
        {
	        StartKey = start.Key;
	        EndKey = end.Key;
        }
        public SegRef(int startKey, int endKey)
        {
	        StartKey = startKey;
	        EndKey = endKey;
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

        public SegRef Clone() => new SegRef(StartKey, EndKey);
	    public float Length() => Segment.Length;
	    public float SquaredLength() => Segment.LengthSquared;
	    public SKPoint PointAlongLine(float t) => Segment.PointAlongLine(t);
	    public SKPoint SKPointFromStart(float dist) => Segment.PointAlongLine(dist);
	    public SKPoint SKPointFromEnd(float dist) => Segment.SKPointFromEnd(dist);
	    public SKPoint OrthogonalPoint(SKPoint pt, float offset) => Segment.OrthogonalPoint(pt, offset);
	    public SKPoint ProjectPointOnto(SKPoint p) => Segment.ProjectPointOnto(p);
        public (float, SKPoint) TFromPoint(SKPoint point) => Segment.TFromPoint(point);
	    public SKPoint[] EndArrow(float dist = 8f) => Segment.EndArrow(dist);

        //public IPoint GetVirtualPointFor(SKPoint point)
        //{
        // var t = TFromPoint(point);
        // return new VirtualPoint(this, t);
        //}

        public static bool operator ==(SegRef left, SegRef right) =>
	        left.StartKey == right.StartKey && left.EndKey == right.EndKey && left.Kind == right.Kind;

        public static bool operator !=(SegRef left, SegRef right) =>
	        left.StartKey != right.StartKey || left.EndKey != right.EndKey || left.Kind != right.Kind;

        public override bool Equals(object obj) => obj is SegRef value && this == value;

        public bool Equals(SegRef value) =>
	        StartKey.Equals(value.StartKey) && EndKey.Equals(value.EndKey) && Kind.Equals(value.Kind);

        public override int GetHashCode() =>
	        StartKey.GetHashCode() * 29 + EndKey.GetHashCode() * 31 + (int)Kind * 37;

    }
}
