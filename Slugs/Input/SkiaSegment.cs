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

    public struct SkiaSegment
    {
	    public SKPoint start { get; }
	    public SKPoint end { get; }

	    public static SkiaSegment Max = new SkiaSegment(SkPointExtension.MaxPoint, SkPointExtension.MaxPoint);
	    public static SkiaSegment Min = new SkiaSegment(SkPointExtension.MinPoint, SkPointExtension.MinPoint);
	    public static SkiaSegment Empty = new SkiaSegment(SKPoint.Empty, SKPoint.Empty);

        public SkiaSegment(SKPoint _start, SKPoint _end)
	    {
		    start = _start;
		    end = _end;
	    }
	    public SkiaSegment(PointF start, PointF end) : this(start.ToSKPoint(), end.ToSKPoint()) { }

	    public static SkiaSegment operator +(SkiaSegment a, float value) => new SkiaSegment(a.start.Add(value), a.end.Add(value));
	    public static SkiaSegment operator -(SkiaSegment a, float value) => new SkiaSegment(a.start.Subtract(value), a.end.Subtract(value));
	    public static SkiaSegment operator *(SkiaSegment a, float value) => new SkiaSegment(a.start.Multiply(value), a.end.Multiply(value));
	    public static SkiaSegment operator /(SkiaSegment a, float value) => value == 0 ? Max : new SkiaSegment(a.start.Divide(value), a.end.Divide(value));

        public float Length() => (float)Math.Sqrt((end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y));
        public SKPoint PointAlongLine(float t) => new SKPoint((end.X - start.X) * t + start.X, (end.Y - start.Y) * t + start.Y);
        public SKPoint SKPointFromStart(float dist) => PointAlongLine(dist / Length());
        public SKPoint SKPointFromEnd(float dist) => PointAlongLine(1 - dist / Length());

        public SKPoint OrthogonalPoint(SKPoint pt, float offset)
        {
	        var angle = (end - start).Angle();
	        return pt.PointAtRadiansAndDistance(angle + (float)Math.PI / 2f, offset);
        }

        public SKPoint[] EndArrow(float dist = 8f)
        {
            var result = new SKPoint[3];
            var p0 = SKPointFromEnd(dist);
            result[0] = OrthogonalPoint(p0, -dist / 2f);
            result[1] = end;
            result[2] = OrthogonalPoint(p0, dist / 2f);
            
            return result;
        }

    }
}
