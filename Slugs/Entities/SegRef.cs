using System.Net;
using SkiaSharp;
using Slugs.Extensions;
using Slugs.Input;
using Slugs.Pads;

namespace Slugs.Motors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SegRef : IEquatable<SegRef>
    {
	    public static SegRef Empty = new SegRef(PtRef.Empty);
	    public bool IsEmpty => StartRef.IsEmpty && EndRef.IsEmpty;

	    public PtRef StartRef { get; private set; }
	    public PtRef EndRef { get; private set; }
	    private PointKind Kind { get; set; }

        private SKSegment _cachedSeg;
        public SKSegment Segment
        {
	        get
	        {
		        if (Kind == PointKind.NeedsUpdate)
		        {
			        _cachedSeg = new SKSegment(StartRef.SKPoint, EndRef.SKPoint);
			        Kind = (Kind == PointKind.Dirty) ? PointKind.Cached : Kind;
		        }
		        return _cachedSeg;
	        }
        }
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

        public SegRef(PtRef startRef)
	    {
		    StartRef = startRef;
		    EndRef = startRef;
	    }
	    public SegRef(PtRef startRef, PtRef endRef)
	    {
		    StartRef = startRef;
		    EndRef = endRef;
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

        public SegRef Clone() => new SegRef(StartRef, EndRef);
	    public float Length() => Segment.Length;
	    public float SquaredLength() => Segment.LengthSquared;
	    public SKPoint PointAlongLine(float t) => Segment.PointAlongLine(t);
	    public SKPoint SKPointFromStart(float dist) => Segment.PointAlongLine(dist);
	    public SKPoint SKPointFromEnd(float dist) => Segment.SKPointFromEnd(dist);
	    public SKPoint OrthogonalPoint(SKPoint pt, float offset) => Segment.OrthogonalPoint(pt, offset);
	    public SKPoint ProjectPointOnto(SKPoint p) => Segment.ProjectPointOnto(p);
	    public float TFromPoint(SKPoint point) => Segment.TFromPoint(point);
	    public SKPoint[] EndArrow(float dist = 8f) => Segment.EndArrow(dist);

        //public IPointRef GetVirtualPointFor(SKPoint point)
        //{
        // var t = TFromPoint(point);
        // return new VirtualPoint(this, t);
        //}

        public static bool operator ==(SegRef left, SegRef right) =>
	        left.StartRef == right.StartRef && left.EndRef == right.EndRef;

        public static bool operator !=(SegRef left, SegRef right) =>
	        left.StartRef != right.StartRef || left.EndRef != right.EndRef;

        public override bool Equals(object obj) => obj is SegRef value && this == value;

        public bool Equals(SegRef value) =>
	        StartRef.Equals(value.StartRef) && EndRef.Equals(value.EndRef);

        public override int GetHashCode() =>
	        17 * 23 + StartRef.GetHashCode() * 29 + EndRef.GetHashCode() * 31;

    }
}
