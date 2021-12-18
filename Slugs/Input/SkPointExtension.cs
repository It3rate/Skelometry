using SkiaSharp;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


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
    }
}
