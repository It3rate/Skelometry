using System.Collections;
using SkiaSharp;
using Slugs.Pads;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class InfoSet : IEnumerable<SKPoint>
    {
	    public static readonly InfoSet Empty = new InfoSet(new []{SKPoint.Empty, SKPoint.Empty});

        private readonly List<SKPoint> Points = new List<SKPoint>();

        public int PadIndex { get; set; } = -1;
        public int InfoSetIndex { get; set; } = -1;
        public int Count => Points.Count;
	    public bool IsEmpty => this == Empty;
        public SKPoint[] ToArray() => Points.ToArray();

        public InfoSet(IEnumerable<SKPoint> points)
        {
	        Points.AddRange(points);
        }
        public InfoSet(params SKPoint[] points)
        {
	        Points.AddRange(points);
        }

        public PointRef PointRefAt(int index) => new PointRef(PadIndex, InfoSetIndex, index);
        public SKPoint this[int index]
	    {
            get =>  index >=0 && index < Points.Count ? Points[index] : SKPoint.Empty;
		    set { if(index >= 0 && index < Points.Count) Points[index] = value; }
	    }

        public SegmentRef SegmentAt(int startIndex, int endIndex = -1)
	    {
		    SegmentRef result;
		    endIndex = (endIndex == -1) ? startIndex + 1 : endIndex;
		    if (startIndex < 0 || startIndex > Points.Count - 1 || endIndex < 0 || endIndex > Points.Count - 1)
		    {
			    result = SegmentRef.Empty;
		    }
		    else
		    {
			    result = new SegmentRef(PointRefAt(startIndex), PointRefAt(endIndex));

		    }
		    return result;
        }
        public SKSegment Line => new SKSegment(Points[0], Points[1]);
        public SKSegment LineAt(int start) => new SKSegment(Points[start], Points[start + 1]);
        public SKSegment LineSegment(int start, int end) => new SKSegment(Points[start], Points[end]);

        public float Length(int startIndex) => SegmentAt(startIndex).Length();
	    public SKPoint PointAlongLine(int startIndex, int endIndex, float t) => SegmentAt(startIndex, endIndex).PointAlongLine(t);
	    public SKPoint SKPointFromStart(int startIndex, float dist) => SegmentAt(startIndex).SKPointFromStart(dist);
	    public SKPoint SKPointFromEnd(int startIndex, float dist) => SegmentAt(startIndex).SKPointFromEnd(dist);
	    public SKPoint OrthogonalPoint(int startIndex, SKPoint pt, float offset) => SegmentAt(startIndex).OrthogonalPoint(pt, offset);

	    public int GetSnapPoint(SKPoint input, float maxDist = 6.0f)
	    {
		    var result = -1;
		    var dist = maxDist * maxDist;
		    int index = 0;
		    foreach (var skPoint in Points)
		    {
			    if (input.SquaredDistanceTo(skPoint) < dist)
			    {
				    result = index;
				    break;
			    }
			    index++;
		    }
		    return result;
	    }

	    public SKPoint[] EndArrow(int startIndex, float dist = 8f) => SegmentAt(startIndex).EndArrow(dist);

	    public IEnumerator<SKPoint> GetEnumerator()
	    {
		    foreach (var pt in Points)
		    {
			    yield return (pt);
		    }
	    }
	    IEnumerator IEnumerable.GetEnumerator()
	    {
		    foreach (var pt in Points)
		    {
			    yield return (pt);
		    }
	    }
    }
}
