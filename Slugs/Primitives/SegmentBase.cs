﻿using System;
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
        public abstract SKPoint StartPosition { get; }//protected set; }
	    public abstract SKPoint EndPosition { get; } //protected set; }

	    public SKSegment Segment => new SKSegment(StartPosition, EndPosition);

        protected SegmentBase(bool isEmpty) : base(isEmpty) { }
        //protected SegmentBase(PadKind padKind) : base(padKind) { }
        protected SegmentBase(PadKind padKind) : base(padKind)
        {
        }

        public float Length() => Segment.Length;
	    public float SquaredLength() => Segment.LengthSquared;
	    public SKPoint PointAlongLine(float t) => Segment.PointAlongLine(t);
	    public SKPoint SKPointFromStart(float dist) => Segment.PointAlongLine(dist);
	    public SKPoint SKPointFromEnd(float dist) => Segment.SKPointFromEnd(dist);
	    public SKPoint OrthogonalPoint(SKPoint pt, float offset) => Segment.OrthogonalPoint(pt, offset);
	    public SKPoint ProjectPointOnto(SKPoint p) => Segment.ProjectPointOnto(p);
        public (float, SKPoint) TFromPoint(SKPoint point, bool clamp = true) => Segment.TFromPoint(point, clamp);
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

    }
}