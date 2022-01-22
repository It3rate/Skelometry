using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Point : IPoint
    {
	    public ElementKind ElementKind => (Kind == PointKind.Terminal) ? ElementKind.Terminal : ElementKind.Point;

        public static Point Empty = new Point();
	    private Point() { Key = -99; SKPoint = SKPoint.Empty; }
        public bool IsEmpty => Key == -99;

	    private static int _counter = 1;

	    public int Key { get; private set; }
	    public PadKind PadKind { get; set; }
	    public PointKind Kind { get; private set; } // todo: move reference to VPoint or it's own class.
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

	    public Point(PadKind padKind, SKPoint point)
	    {
		    Key = _counter++;
		    Kind = PointKind.Terminal;
		    PadKind = padKind;
		    _point = point;
	    }


	    public Pad GetPad() => Agent.Current.PadAt(PadKind);
	    public Entity GetEntity() => Entity.Empty; 
	    public Trait GetTrait() => Trait.Empty; // todo: find valid trait/focal for terminal points.
	    public Focal GetFocal() => Focal.Empty;
	    public float GetT() => 0;

        public bool ReplaceWith(IPoint pt)
	    {
		    Key = pt.Key;
		    Kind = PointKind.Reference;
		    return true;
        }

	    public void CopyValuesFrom(IPoint from)
	    {
		    PadKind = from.PadKind;
		    if (from.Kind == PointKind.Terminal)
		    {
			    Kind = PointKind.Terminal;
			    _point = from.SKPoint;
		    }
            else
		    {
			    Kind = PointKind.Reference;
                Key = from.Key;
			    //_point = from.SKPoint;
		    }
	    }
        public static bool operator ==(Point left, IPoint right) => left.Key == right.Key;
	    public static bool operator !=(Point left, IPoint right) => left.Key != right.Key;
	    public override bool Equals(object obj) => obj is Point value && this == value;
	    public bool Equals(IPoint value) => this == value;
	    public override int GetHashCode() => Key.GetHashCode();
    }
}
