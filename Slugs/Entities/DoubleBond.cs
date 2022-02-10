using System.Net;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Primitives;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DoubleBond : ElementBase
    {
	    public int StartKey { get; private set; }
	    public int EndKey { get; private set; }


        public override ElementKind ElementKind => ElementKind.DoubleBond;
	    public override IElement EmptyElement => Empty;
	    public static readonly DoubleBond Empty = new DoubleBond();
	    private DoubleBond() : base(true) { }// Empty ctor

	    public Focal StartFocal => Pad.FocalAt(StartKey);
	    public Focal EndFocal => Pad.FocalAt(EndKey);
	    public SKSegment StartSegment => StartFocal.Segment;
	    public SKSegment EndPosition => EndFocal.Segment;

	    public override List<IPoint> Points => new List<IPoint>() { StartFocal.StartPoint, StartFocal.EndPoint, EndFocal.StartPoint, EndFocal.EndPoint };
	    public override float DistanceToPoint(SKPoint point)
	    {
		    var startDist = StartFocal.DistanceToPoint(point);
		    var endDist = EndFocal.DistanceToPoint(point);
		    return startDist < endDist ? startDist : endDist;
	    }

	    public DoubleBond(Focal startFocal, Focal endFocal) : base(startFocal.PadKind)
	    {
		    SetStartKey(startFocal.Key);
		    SetEndKey(endFocal.Key);
	    }

	    protected void SetStartKey(int key)
	    {
		    if (!Pad.FocalAt(key).IsEmpty)
		    {
			    StartKey = key;
		    }
		    else
		    {
			    throw new ArgumentException("DoubleBond focals must be type Focal.");
		    }
	    }
	    protected void SetEndKey(int key)
	    {
		    if (!Pad.FocalAt(key).IsEmpty)
		    {
			    EndKey = key;
            }
		    else
		    {
			    throw new ArgumentException("DoubleBond focals must be type Focal.");
		    }
	    }

	    public static bool operator ==(DoubleBond left, DoubleBond right) =>
		    left.Key == right.Key && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
	    public static bool operator !=(DoubleBond left, DoubleBond right) =>
		    left.Key != right.Key || left.StartKey != right.StartKey || left.EndKey != right.EndKey;
	    public override bool Equals(object obj) => obj is DoubleBond value && this == value;
	    public override int GetHashCode() => 13 * Key + 17 * Key + 27 * StartKey + 29 * EndKey;
    }
}
