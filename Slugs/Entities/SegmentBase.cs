using SkiaSharp;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public abstract class SegmentBase : ElementBase
    {
	    public int StartKey { get; protected set; }
	    public int EndKey { get; protected set; }
	    protected virtual void SetStartKey(int key) => StartKey = key;
	    protected virtual void SetEndKey(int key) => EndKey = key;

        public abstract SKPoint StartPosition { get; }//protected set; }
	    public abstract SKPoint EndPosition { get; } //protected set; }

	    public SKSegment Segment => new SKSegment(StartPosition, EndPosition);

        protected SegmentBase(bool isEmpty) : base(isEmpty) { }
        //protected SegmentBase(PadKind padKind) : base(padKind) { }
        protected SegmentBase(PadKind padKind) : base(padKind)
        {
        }
        public override SKPath Path
        {
	        get
	        {
		        var path = new SKPath();
		        path.MoveTo(StartPosition);
			    path.LineTo(EndPosition);
		        return path;
	        }
        }
        public float Length => Segment.Length;
	    public float SquaredLength => Segment.LengthSquared;
	    public SKPoint PointAlongLine(float t) => Segment.PointAlongLine(t);
	    public SKPoint SKPointFromStart(float dist) => Segment.PointAlongLine(dist);
	    public SKPoint SKPointFromEnd(float dist) => Segment.SKPointFromEnd(dist);
	    public SKPoint OrthogonalPoint(SKPoint pt, float offset) => Segment.OrthogonalPoint(pt, offset);
	    public SKPoint ProjectPointOnto(SKPoint p) => Segment.ProjectPointOnto(p);
        public (float, SKPoint) TFromPoint(SKPoint point, bool clamp = true) => Segment.TFromPoint(point, clamp);
	    public SKPoint[] EndArrow(float dist = 8f) => Segment.EndArrow(dist);

	    public override float DistanceToPoint(SKPoint point)
	    {
		    var closest = ProjectPointOnto(point);
		    return point.DistanceTo(closest);
	    }

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
