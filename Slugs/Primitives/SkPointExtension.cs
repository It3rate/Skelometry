using System;
using SkiaSharp;

namespace Slugs.Primitives
{
	public static class SkPointExtension
    {
	    public static SKPoint MaxPoint = new SKPoint(float.MaxValue, float.MaxValue);
	    public static SKPoint MinPoint = new SKPoint(float.MinValue, float.MinValue);

	    public static float Angle(this SKPoint a) => (float)Math.Atan2(a.Y, a.X);
        public static float AngleDegrees(this SKPoint a) => a.Angle() * 180f / 2f;

	    public static SKPoint PointAtRadiansAndDistance(this SKPoint a, float angle, float distance) =>
		    new SKPoint(a.X + (float)Math.Cos(angle) * distance, a.Y + (float)Math.Sin(angle) * distance);
	    public static SKPoint PointAtDegreesAndDistance(this SKPoint a, float angle, float distance) =>
		    PointAtRadiansAndDistance(a, angle / 180f * 2f, distance);

        public static SKPoint Add(this SKPoint a, float value) => new SKPoint(a.X + value, a.Y + value);
	    public static SKPoint Subtract(this SKPoint a, float value) => new SKPoint(a.X - value, a.Y - value);
	    public static SKPoint Multiply(this SKPoint a, float value) => new SKPoint(a.X * value, a.Y * value);
	    public static SKPoint Divide(this SKPoint a, float value) => new SKPoint(value == 0 ? float.MaxValue : a.X / value, value == 0 ? float.MaxValue : a.Y / value);
        public static SKPoint TurnCCW(this SKPoint a) => new SKPoint(-a.Y, a.X);
	    public static SKPoint TurnCW(this SKPoint a) => new SKPoint(a.Y, -a.X);
	    public static SKPoint RotateOnOrigin(this SKPoint a, float degrees)
	    {
		    var angle = (float)(degrees / 180.0 * Math.PI);
		    var x = (float)(Math.Cos(angle) * a.X - Math.Sin(angle) * a.Y);
		    var y = (float)(Math.Sin(angle) * a.X + Math.Cos(angle) * a.Y);
		    return new SKPoint(x, y);
	    }
        public static SKPoint Normalize(this SKPoint a)
	    {
		    var len = a.Length;
		    return new SKPoint(a.X / len, a.Y / len);
	    }


	    public static float Length(this SKPoint self) => (float)Math.Sqrt(self.X * self.X + self.Y * self.Y);
	    public static float SquaredLength(this SKPoint self) => self.X * self.X + self.Y * self.Y;
        public static float DistanceTo(this SKPoint self, SKPoint b) => (self - b).Length;
	    public static float SquaredDistanceTo(this SKPoint self, SKPoint b) => (self - b).LengthSquared;
	    public static float DotProduct(this SKPoint self, SKPoint pt) => self.X * pt.X + self.Y * pt.Y;
	    public static float Atan2(this SKPoint self, SKPoint pt) => (float)Math.Atan2(pt.Y - self.Y, pt.X - self.X);
	    public static float DistanceToLine(this SKPoint self, SKSegment line, bool clamp = true) => line.DistanceTo(self, clamp);
	    public static float SignedDistanceTo(this SKPoint self, SKPoint pt)
	    {
		    var sDist = (pt.X - self.X) * (pt.X - self.X) + (pt.Y - self.Y) * (pt.Y - self.Y);
		    return (float)Math.Sqrt(sDist) * (sDist >= 0 ? 1f : -1f);
	    }

        public static (float, float, float) ABCLine(this SKPoint self, SKPoint pt)
	    {
		    var a = pt.Y - self.Y;
		    var b = self.X - pt.X;
		    var c = a * self.X + b * self.Y;
		    return (a, b, c);
	    }
    }
}
