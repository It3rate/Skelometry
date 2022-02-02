using System;
using System.Collections.Generic;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Primitives
{
	public abstract class SegmentBase : ElementBase
    {
	    public int StartKey { get; protected set; }
	    public int EndKey { get; protected set; }
	    public IPoint StartPoint => Pad.PointAt(StartKey); // todo: make this base class for segment elements.
	    public IPoint EndPoint => Pad.PointAt(EndKey);
	    public abstract SKPoint StartPosition { get; }//protected set; }
	    public abstract SKPoint EndPosition { get; } //protected set; }

	    //public SKPoint StartPosition
        //{
        // get => StartPoint.Position;
        // set => StartPoint.Position = value;
        //}
        //public SKPoint EndPosition
        //{
        // get => EndPoint.Position;
        // set => EndPoint.Position = value;
        //}

        public abstract SKSegment Segment { get; }//=> new SKSegment(StartPosition, EndPosition);

        public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { StartPoint, EndPoint };

        protected SegmentBase(bool isEmpty) : base(isEmpty) { }
        //protected SegmentBase(PadKind padKind) : base(padKind) { }
        protected SegmentBase(PadKind padKind, int startKey, int endKey) : base(padKind)
        {
	        StartKey = startKey;
	        EndKey = endKey;
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

        //public static SegmentBase operator +(SegmentBase a, float value)
        //{
        // a.StartPosition = a.StartPosition.Add(value);
        // a.EndPosition = a.EndPosition.Add(value);
        // return a;
        //}
        //public static SegmentBase operator -(SegmentBase a, float value)
        //{
        // a.StartPosition = a.StartPosition.Subtract(value);
        // a.EndPosition = a.EndPosition.Subtract(value);
        // return a;
        //}
        //public static SegmentBase operator *(SegmentBase a, float value)
        //{
        // a.StartPosition = a.StartPosition.Multiply(value);
        // a.EndPosition = a.EndPosition.Multiply(value);
        // return a;
        //}
        //public static SegmentBase operator /(SegmentBase a, float value)
        //{
        // a.StartPosition = a.StartPosition.Divide(value);
        // a.EndPosition = a.EndPosition.Divide(value);
        // return a;
        //}
        public static bool operator ==(SegmentBase left, SegmentBase right) =>
            left.Key == right.Key && left.StartKey == right.StartKey && left.EndKey == right.EndKey;

        public static bool operator !=(SegmentBase left, SegmentBase right) =>
            left.Key != right.Key || left.StartKey != right.StartKey || left.EndKey != right.EndKey;

        public override bool Equals(object obj) => obj is SegmentBase value && this == value;

        public bool Equals(SegmentBase value) =>
            Key == value.Key && StartKey == value.StartKey && EndKey == value.EndKey;

        public override int GetHashCode() => Key * 17 + StartKey * 29 + EndKey * 31;

    }
}
