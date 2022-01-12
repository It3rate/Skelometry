//using System.Drawing;
//using SkiaSharp;
//using SkiaSharp.Views.Desktop;
//using Slugs.Agent;
//using Slugs.Extensions;
//using Slugs.Pads;
//using Slugs.Slugs;

//namespace Slugs.Input
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;

//    public readonly struct SegRef : IEquatable<SegRef>
//    {
//	    public static SegRef Empty = new SegRef(PointRef.Empty);
//	    public bool IsEmpty => StartRef.IsEmpty && EndRef.IsEmpty;

//        public IPointRef StartRef { get; }
//	    public IPointRef EndRef { get; }

//	    public SKPoint StartPoint
//	    {
//		    get => SlugAgent.ActiveAgent[StartRef];
//		    set => SlugAgent.ActiveAgent[StartRef] = value;
//	    }
//	    public SKPoint EndPoint
//	    {
//		    get => SlugAgent.ActiveAgent[EndRef];
//		    set => SlugAgent.ActiveAgent[EndRef] = value;
//	    }
//        public SKSegment SKSegment => new SKSegment(StartPoint, EndPoint);

//        public SegRef(IPointRef startRef)
//	    {
//		    StartRef = startRef;
//		    EndRef = startRef;
//	    }
//	    public SegRef(IPointRef startRef, IPointRef endRef)
//	    {
//		    StartRef = startRef;
//		    EndRef = endRef;
//	    }
//        public SegRef Clone() => new SegRef(StartRef, EndRef);

//        public static SegRef operator +(SegRef a, float value)
//        {
//	        a.StartPoint = a.StartPoint.Add(value);
//	        a.EndPoint = a.EndPoint.Add(value);
//            return a.Clone();
//        }
//        public static SegRef operator -(SegRef a, float value)
//        {
//	        a.StartPoint = a.StartPoint.Subtract(value);
//	        a.EndPoint = a.EndPoint.Subtract(value);
//	        return a.Clone();
//        }
//        public static SegRef operator *(SegRef a, float value)
//        {
//	        a.StartPoint = a.StartPoint.Multiply(value);
//	        a.EndPoint = a.EndPoint.Multiply(value);
//	        return a.Clone();
//        }
//        public static SegRef operator /(SegRef a, float value)
//        {
//	        a.StartPoint = a.StartPoint.Divide(value);
//	        a.EndPoint = a.EndPoint.Divide(value);
//	        return a.Clone();
//        }

//        public float Length() => SKSegment.Length;
//        public float SquaredLength() => SKSegment.LengthSquared;
//        public SKPoint PointAlongLine(float t) => SKSegment.PointAlongLine(t);
//        public SKPoint SKPointFromStart(float dist) => SKSegment.PointAlongLine(dist);
//        public SKPoint SKPointFromEnd(float dist) => SKSegment.SKPointFromEnd(dist);
//        public SKPoint OrthogonalPoint(SKPoint pt, float offset) => SKSegment.OrthogonalPoint(pt, offset);
//        public SKPoint ProjectPointOnto(SKPoint p) => SKSegment.ProjectPointOnto(p);
//        public float TFromPoint(SKPoint point) => SKSegment.TFromPoint(point);
//        public SKPoint[] EndArrow(float dist = 8f) => SKSegment.EndArrow(dist);

//        public IPointRef GetVirtualPointFor(SKPoint point)
//        {
//	        var t = TFromPoint(point);
//            return new VirtualPoint(this, t);
//        }

//        public static bool operator ==(SegRef left, SegRef right) =>
//	        left.StartRef == right.StartRef && left.EndRef == right.EndRef;

//        public static bool operator !=(SegRef left, SegRef right) =>
//	        left.StartRef != right.StartRef || left.EndRef != right.EndRef;

//        public override bool Equals(object obj) => obj is SegRef value && this == value;

//        public bool Equals(SegRef value) =>
//	        StartRef.Equals(value.StartRef) && EndRef.Equals(value.EndRef);

//        public override int GetHashCode() =>
//	        17 * 23 + StartRef.GetHashCode() * 29 + EndRef.GetHashCode() * 31;
//    }
//}
