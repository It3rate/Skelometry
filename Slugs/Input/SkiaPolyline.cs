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
	    public readonly List<SKPoint> Points = new List<SKPoint>();

	    public SkiaPolyline(IEnumerable<SKPoint> points)
	    {
		    Points.AddRange(points);
	    }
	    public SkiaPolyline(params SKPoint[] points)
	    {
		    Points.AddRange(points);
	    }

        public double Length(int startIndex)
	    {
		    double result = 0;
		    if (startIndex < Points.Count)
		    {
			    var start = Points[startIndex];
			    var end = Points[startIndex + 1];
			    result = Math.Sqrt((end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y));
		    }
		    return result;
	    }

	    public SKPoint PointAlongLine(int startIndex, float t)
        {
	        var result = SKPoint.Empty;
	        if (startIndex < Points.Count)
	        {
		        var start = Points[startIndex];
		        var end = Points[startIndex + 1];
		        result = new SKPoint((end.X - start.X) * t + start.X, (end.Y - start.Y) * t + start.Y);
	        }
	        return result;
        } 

    }
}
