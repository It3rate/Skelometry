using System.ComponentModel;
using SkiaSharp;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum TraitKind
    {
        None,
        Default,
        XAxis,
        YAxis,
    }

    public class Trait : SegmentBase, ISegmentElement, IMidpointSettable
    {
	    public override ElementKind ElementKind => ElementKind.Trait;
	    public override IElement EmptyElement => Empty;
	    public static readonly Trait Empty = new Trait();
        private Trait() : base(true) { TraitKind = TraitKind.None;}

        public IPoint StartPoint => Pad.PointAt(StartKey);
        public IPoint EndPoint => Pad.PointAt(EndKey);
        public IPoint OtherPoint(IPoint orgPoint) => StartPoint == orgPoint ? EndPoint : StartPoint;
        public TraitKind TraitKind { get; }

        public override SKPoint StartPosition => StartPoint.Position;
	    public override SKPoint EndPosition => EndPoint.Position;
	    public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { StartPoint, EndPoint };

        public Trait(TraitKind traitKind, IPoint start, IPoint end) : base(start.PadKind)
        {
		    TraitKind = traitKind;
		    StartKey = start.Key;
		    EndKey = end.Key;
        }
        public Trait(float x0, float y0, float x1, float y1, PadKind padKind = PadKind.Input, TraitKind traitKind = TraitKind.Default) : base(padKind)
        {
	        TraitKind = traitKind;
	        StartKey = new TerminalPoint(padKind, new SKPoint(x0, y0)).Key;
	        EndKey = new TerminalPoint(padKind, new SKPoint(x1, y1)).Key;
        }

        protected override void SetStartKey(int key)
        {
	        if (Pad.ElementAt(key) is IPoint)
	        {
		        base.SetStartKey(key);
	        }
	        else
	        {
		        throw new ArgumentException("Trait points must be IPoint.");
	        }
        }
        protected override void SetEndKey(int key)
        {
	        if (Pad.ElementAt(key) is IPoint)
	        {
		        base.SetEndKey(key);
	        }
	        else
	        {
		        throw new ArgumentException("Trait points must be IPoint.");
	        }
        }

        public void SetMidpoint(SKPoint midPoint)
        {
	        var midSeg = Segment;
	        midSeg.Midpoint = midPoint;
	        StartPoint.Position = midSeg.StartPoint;
	        EndPoint.Position = midSeg.EndPoint;
        }

        private readonly HashSet<int> _focalKeys = new HashSet<int>();
        public IEnumerable<Focal> Focals
        {
	        get
	        {
		        foreach (var key in _focalKeys)
		        {
			        yield return Pad.FocalAt(key);
		        }
	        }
        }
        public void AddFocal(Focal focal)
        {
	        _focalKeys.Add(focal.Key);
        }
        public void RemoveFocal(Focal focal)
        {
	        _focalKeys.Remove(focal.Key);
        }

        public bool IsPerpendicularTo(Trait trait)
        {
	        return Segment.IsPerpendicularTo(trait.Segment);
        }
        public void SetLengthByMidpoint(float length)
        {
	        SKSegment seg = Segment.GetMeasuredSegmentByMidpoint(length);
	        StartPoint.Position = seg.StartPoint;
	        EndPoint.Position = seg.EndPoint;
        }

        public static bool operator ==(Trait left, Trait right) => 
	        left.Key == right.Key && left.TraitKind == right.TraitKind && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
        public static bool operator !=(Trait left, Trait right) => 
	        left.Key != right.Key || left.TraitKind != right.TraitKind || left.StartKey != right.StartKey || left.EndKey != right.EndKey;
        public override bool Equals(object obj) => obj is Trait value && this == value;
        public override int GetHashCode() => Key * 17 + (int)TraitKind * 23 + StartKey * 29 + EndKey * 31;
    }
}
