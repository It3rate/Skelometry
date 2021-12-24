using SkiaSharp;
using Slugs.Agent;
using Slugs.Pads;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct SKSegment
    {
        public SKPoint StartPoint { get; private set; }
        public SKPoint EndPoint { get; private set; }

        private static readonly InfoSet _values = new InfoSet(new[] { SKPoint.Empty, SkPointExtension.MinPoint, SkPointExtension.MaxPoint });
        public static SKSegment Max = new SKSegment(new SKPoint(float.MaxValue, float.MaxValue));
        public static SKSegment Min = new SKSegment(new SKPoint(float.MinValue, float.MinValue));
        public static SKSegment Empty = new SKSegment(SKPoint.Empty);

        public SKSegment(SKPoint start)
        {
	        StartPoint = start;
	        EndPoint = start;
        }
        public SKSegment(SKPoint start, SKPoint end)
        {
	        StartPoint = start;
	        EndPoint = end;
        }

        public SKPoint[] Points => new[] {StartPoint, EndPoint};
        public SKSegment Clone() => new SKSegment(StartPoint, EndPoint);

        public static SKSegment operator +(SKSegment a, float value)
        {
            a.StartPoint = a.StartPoint.Add(value);
            a.EndPoint = a.EndPoint.Add(value);
            return a.Clone();
        }
        public static SKSegment operator -(SKSegment a, float value)
        {
            a.StartPoint = a.StartPoint.Subtract(value);
            a.EndPoint = a.EndPoint.Subtract(value);
            return a.Clone();
        }
        public static SKSegment operator *(SKSegment a, float value)
        {
            a.StartPoint = a.StartPoint.Multiply(value);
            a.EndPoint = a.EndPoint.Multiply(value);
            return a.Clone();
        }
        public static SKSegment operator /(SKSegment a, float value)
        {
            a.StartPoint = a.StartPoint.Divide(value);
            a.EndPoint = a.EndPoint.Divide(value);
            return a.Clone();
        }

        public float Length() => (float)Math.Sqrt((EndPoint.X - StartPoint.X) * (EndPoint.X - StartPoint.X) + (EndPoint.Y - StartPoint.Y) * (EndPoint.Y - StartPoint.Y));
        public float SquaredLength() => (EndPoint.X - StartPoint.X) * (EndPoint.X - StartPoint.X) + (EndPoint.Y - StartPoint.Y) * (EndPoint.Y - StartPoint.Y);
        public SKPoint PointAlongLine(float t) => new SKPoint((EndPoint.X - StartPoint.X) * t + StartPoint.X, (EndPoint.Y - StartPoint.Y) * t + StartPoint.Y);
        public SKPoint SKPointFromStart(float dist) => PointAlongLine(dist / Length());
        public SKPoint SKPointFromEnd(float dist) => PointAlongLine(1 - dist / Length());

        public SKPoint OrthogonalPoint(SKPoint pt, float offset)
        {
            var angle = (EndPoint - StartPoint).Angle();
            return pt.PointAtRadiansAndDistance(angle + (float)Math.PI / 2f, offset);
        }
        public SKPoint ProjectPointOnto(SKPoint p)
        {

            var e1 = EndPoint - StartPoint;
            var e2 = p - StartPoint;
            var dp = e1.DotProduct(e2);

            var len2 = e1.SquaredLength();
            var x = StartPoint.X + (dp * e1.X) / len2;
            var y = StartPoint.Y + (dp * e1.Y) / len2;
            x = (x < StartPoint.X && x < EndPoint.X) ? (float)Math.Min(StartPoint.X, EndPoint.X) : (x > StartPoint.X && x > EndPoint.X) ? (float)Math.Max(StartPoint.X, EndPoint.X) : x;
            y = (y < StartPoint.Y && y < EndPoint.Y) ? (float)Math.Min(StartPoint.Y, EndPoint.Y) : (y > StartPoint.Y && y > EndPoint.Y) ? (float)Math.Max(StartPoint.Y, EndPoint.Y) : y;
            return new SKPoint(x, y);
        }

        public SKPoint[] EndArrow(float dist = 8f)
        {
            var result = new SKPoint[3];
            var p0 = SKPointFromEnd(dist);
            result[0] = OrthogonalPoint(p0, -dist / 2f);
            result[1] = EndPoint;
            result[2] = OrthogonalPoint(p0, dist / 2f);

            return result;
        }
    }
}
