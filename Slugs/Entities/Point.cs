using SkiaSharp;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Point : IPoint
    {
        public static Point Empty = new Point();
	    private Point() { Key = -1; SKPoint = SKPoint.Empty; }
        public bool IsEmpty => Key == -1;

	    private static int _counter = 1;

	    public int Key { get; }
	    public int PadIndex { get; set; }
        public SKPoint SKPoint { get; set; }
	    public PointKind Kind { get; set; }

	    public Point(int padIndex, SKPoint point)
	    {
		    Key = _counter++;
		    PadIndex = padIndex;
		    SKPoint = point;
	    }
	    public bool ReplaceWith(IPoint pt)
	    {
		    SKPoint = pt.SKPoint;
		    return true;
        }
	    public static bool operator ==(Point left, IPoint right) => left.Key == right.Key;
	    public static bool operator !=(Point left, IPoint right) => left.Key != right.Key;
	    public override bool Equals(object obj) => obj is Point value && this == value;
	    public bool Equals(IPoint value) => this == value;
	    public override int GetHashCode() => Key.GetHashCode();
    }
}
