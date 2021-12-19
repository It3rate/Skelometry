using SkiaSharp;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SkiaPolyline
    {
	    public static readonly SkiaPolyline Empty = new SkiaPolyline(SKPoint.Empty, SKPoint.Empty);
        public readonly List<SKPoint> Points = new List<SKPoint>();

	    public SkiaPolyline(IEnumerable<SKPoint> points)
	    {
		    Points.AddRange(points);
	    }
	    public SkiaPolyline(params SKPoint[] points)
	    {
		    Points.AddRange(points);
	    }

	    public SkiaSegment SegmentAt(int index) => 
		    index < 0 || index > Points.Count - 1 ? SkiaSegment.Empty : new SkiaSegment(Points[index], Points[index + 1]);

	    public float Length(int startIndex) => SegmentAt(startIndex).Length();
	    public SKPoint PointAlongLine(int startIndex, float t) => SegmentAt(startIndex).PointAlongLine(t);
	    public SKPoint SKPointFromStart(int startIndex, float dist) => SegmentAt(startIndex).SKPointFromStart(dist);
	    public SKPoint SKPointFromEnd(int startIndex, float dist) => SegmentAt(startIndex).SKPointFromEnd(dist);
	    public SKPoint OrthogonalPoint(int startIndex, SKPoint pt, float offset) => SegmentAt(startIndex).OrthogonalPoint(pt, offset);

	    public SKPoint[] EndArrow(int startIndex, float dist = 8f) => SegmentAt(startIndex).EndArrow(dist);

    }
}
