using SkiaSharp;
using Slugs.Agents;

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

	    public int Key { get; private set; }
	    public int PadIndex { get; set; }
	    private SKPoint _point;
        public SKPoint SKPoint
        {
	        get => Kind == PointKind.Terminal ? _point : Agent.Current.PointAt(Key).SKPoint;
	        set
	        {
		        if (Kind == PointKind.Terminal)
		        {
			        _point = value;
		        }
		        else
		        {
			        var tp = Agent.Current.TerminalPointAt(Key);
			        if (!tp.IsEmpty)
			        {
				        tp.SKPoint = value;
			        }
		        }
	        }
        }
        public PointKind Kind { get; set; }

	    public Point(int padIndex, SKPoint point)
	    {
		    Key = _counter++;
		    Kind = PointKind.Terminal;
		    PadIndex = padIndex;
		    _point = point;
	    }
	    public bool ReplaceWith(IPoint pt)
	    {
		    Key = pt.Key;
		    Kind = PointKind.Pointer;
		    return true;
        }
	    public static bool operator ==(Point left, IPoint right) => left.Key == right.Key;
	    public static bool operator !=(Point left, IPoint right) => left.Key != right.Key;
	    public override bool Equals(object obj) => obj is Point value && this == value;
	    public bool Equals(IPoint value) => this == value;
	    public override int GetHashCode() => Key.GetHashCode();
    }
}
