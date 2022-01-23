﻿using System;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Primitives
{
	public abstract class SegmentBase : ElementBase
    {
	    public int StartKey { get; private set; }
	    public int EndKey { get; private set; }

	    public IPoint StartRef => Pad.PointAt(StartKey); // todo: make this base class for segment elements.
	    public IPoint EndRef => Pad.PointAt(EndKey);

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

        protected SegmentBase(bool isEmpty):base(isEmpty){}
        public SegmentBase(PadKind padKind, IPoint start) : base(padKind)
	    {
		    StartKey = start.Key;
		    EndKey = start.Key;
        }
        public SegmentBase(PadKind padKind, IPoint start, IPoint end) : base(padKind)
        {
	        StartKey = start.Key;
	        EndKey = end.Key;
        }
        public SegmentBase(PadKind padKind, int startKey, int endKey) : base(padKind)
        {
	        StartKey = startKey;
	        EndKey = endKey;
        }

        public static SegmentBase operator +(SegmentBase a, float value)
	    {
		    a.StartPoint = a.StartPoint.Add(value);
		    a.EndPoint = a.EndPoint.Add(value);
		    return a;
	    }
	    public static SegmentBase operator -(SegmentBase a, float value)
	    {
		    a.StartPoint = a.StartPoint.Subtract(value);
		    a.EndPoint = a.EndPoint.Subtract(value);
		    return a;
	    }
	    public static SegmentBase operator *(SegmentBase a, float value)
	    {
		    a.StartPoint = a.StartPoint.Multiply(value);
		    a.EndPoint = a.EndPoint.Multiply(value);
		    return a;
	    }
	    public static SegmentBase operator /(SegmentBase a, float value)
	    {
		    a.StartPoint = a.StartPoint.Divide(value);
		    a.EndPoint = a.EndPoint.Divide(value);
		    return a;
	    }

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

        //public static bool operator ==(SegmentBase left, SegmentBase right) =>
        // left.StartKey == right.StartKey && left.EndKey == right.EndKey && left.Kind == right.Kind;

        //public static bool operator !=(SegmentBase left, SegmentBase right) =>
        // left.StartKey != right.StartKey || left.EndKey != right.EndKey || left.Kind != right.Kind;

        //public override bool Equals(object obj) => obj is SegmentBase value && this == value;

        //public bool Equals(SegmentBase value) =>
        // StartKey.Equals(value.StartKey) && EndKey.Equals(value.EndKey) && Kind.Equals(value.Kind);

        //public override int GetHashCode() =>
        // StartKey.GetHashCode() * 29 + EndKey.GetHashCode() * 31 + (int)Kind * 37;

    }
}
