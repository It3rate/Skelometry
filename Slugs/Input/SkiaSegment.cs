using System.Drawing;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public readonly struct SkiaSegment
    {
	    public SKPoint Start => _ref.Points[_startIndex];
	    public SKPoint End => _ref.Points[_endIndex];

        private readonly SkiaPolyline _ref;
        private readonly int _startIndex;
        private readonly int _endIndex;

        private static readonly SkiaPolyline _values = new SkiaPolyline(SKPoint.Empty, SkPointExtension.MinPoint, SkPointExtension.MaxPoint);
        public static SkiaSegment Max = new SkiaSegment(_values, 2, 2);
	    public static SkiaSegment Min = new SkiaSegment(_values, 1, 1);
	    public static SkiaSegment Empty = new SkiaSegment(_values, 0, 0);

	    public SkiaSegment(SkiaPolyline reference, int startIndex, int endIndex)
	    {
		    _ref = reference;
		    _startIndex = startIndex >= 0 && startIndex < _ref.Points.Count ? startIndex : 0;
		    _endIndex = endIndex >= 0 && endIndex < _ref.Points.Count ? endIndex : _ref.Points.Count - 1;
	    }
        public SkiaSegment(SKPoint start, SKPoint end)
        {
            _ref = new SkiaPolyline(start, end);
            _startIndex = 0;
            _endIndex = 1;
        }
        public SkiaSegment(PointF start, PointF end) : this(start.ToSKPoint(), end.ToSKPoint()) { }

        public static SkiaSegment operator +(SkiaSegment a, float value) => new SkiaSegment(a.Start.Add(value), a.End.Add(value));
	    public static SkiaSegment operator -(SkiaSegment a, float value) => new SkiaSegment(a.Start.Subtract(value), a.End.Subtract(value));
	    public static SkiaSegment operator *(SkiaSegment a, float value) => new SkiaSegment(a.Start.Multiply(value), a.End.Multiply(value));
	    public static SkiaSegment operator /(SkiaSegment a, float value) => value == 0 ? Max : new SkiaSegment(a.Start.Divide(value), a.End.Divide(value));

	    public float Length() => (float)Math.Sqrt((End.X - Start.X) * (End.X - Start.X) + (End.Y - Start.Y) * (End.Y - Start.Y));
	    public float SquaredLength() => (End.X - Start.X) * (End.X - Start.X) + (End.Y - Start.Y) * (End.Y - Start.Y);
        public SKPoint PointAlongLine(float t) => new SKPoint((End.X - Start.X) * t + Start.X, (End.Y - Start.Y) * t + Start.Y);
        public SKPoint SKPointFromStart(float dist) => PointAlongLine(dist / Length());
        public SKPoint SKPointFromEnd(float dist) => PointAlongLine(1 - dist / Length());

        public SKPoint OrthogonalPoint(SKPoint pt, float offset)
        {
	        var angle = (End - Start).Angle();
	        return pt.PointAtRadiansAndDistance(angle + (float)Math.PI / 2f, offset);
        }
        public SKPoint ProjectPointOnto(SKPoint p)
        {

            var e1 = End - Start;
	        var e2 = p - Start;
	        var dp = e1.DotProduct(e2);

            var len2 = e1.SquaredLength();
	        var x = Start.X + (dp * e1.X) / len2;
            var y = Start.Y + (dp * e1.Y) / len2;
            x = (x < Start.X && x < End.X) ? (float)Math.Min(Start.X, End.X) : (x > Start.X && x > End.X) ? (float)Math.Max(Start.X, End.X) : x;
	        y = (y < Start.Y && y < End.Y) ? (float)Math.Min(Start.Y, End.Y) : (y > Start.Y && y > End.Y) ? (float)Math.Max(Start.Y, End.Y) : y;
	        return new SKPoint(x, y);
        }

        public SKPoint[] EndArrow(float dist = 8f)
        {
            var result = new SKPoint[3];
            var p0 = SKPointFromEnd(dist);
            result[0] = OrthogonalPoint(p0, -dist / 2f);
            result[1] = End;
            result[2] = OrthogonalPoint(p0, dist / 2f);
            
            return result;
        }

    }
}
