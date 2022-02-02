using System;
using System.Collections.Generic;
using SkiaSharp;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public class Focal : SegmentBase
    {
	    public override ElementKind ElementKind => ElementKind.Focal;
	    public override IElement EmptyElement => Empty;
	    public static readonly Focal Empty = new Focal();
        private Focal() : base(true) {}

        public int TraitKey { get; }
        public Trait Trait => Pad.TraitAt(TraitKey);

        public PointOnTrait StartPoint => (PointOnTrait)Pad.PointAt(StartKey); // todo: make this base class for segment elements.
        public PointOnTrait EndPoint => (PointOnTrait)Pad.PointAt(EndKey);
        public override SKPoint StartPosition => Trait.PointAlongLine(Slug.Start);
        public override SKPoint EndPosition => Trait.PointAlongLine(Slug.End);

        public Slug Slug => new Slug(StartPoint.T, EndPoint.T);

        public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { StartPoint, EndPoint };

        // trait instances in an entity are traits with a unit, that are contained in one or more entities.
        // maybe a bond is just two focals connected (on the same or different traits),
        //    with the option of intermediate repetition, scaling and directionality (causation)

        public Focal(Trait trait, float startT, float endT) : base(trait.PadKind)
        {
	        TraitKey = trait.Key;
            var startPoint = new PointOnTrait(PadKind, TraitKey, startT);
	        StartKey = startPoint.Key;
	        var endPoint = new PointOnTrait(PadKind, TraitKey, endT);
            EndKey = endPoint.Key;
        }
        public Focal(Trait trait, PointOnTrait startPoint, PointOnTrait endPoint) : base(trait.PadKind)
        {
	        TraitKey = trait.Key;
	        StartKey = startPoint.Key;
	        EndKey = endPoint.Key;
        }
        public Focal(PadKind padKind, int traitKey, int startKey, int endKey) : base(padKind)
        {
	        TraitKey = traitKey;
	        StartKey = startKey;
	        EndKey = endKey;
        }

        public static bool operator ==(Focal left, Focal right) => left.Key == right.Key && left.TraitKey == right.TraitKey && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
        public static bool operator !=(Focal left, Focal right) => left.Key != right.Key || left.TraitKey != right.TraitKey || left.StartKey != right.StartKey || left.EndKey != right.EndKey;
	    public override bool Equals(object obj) => obj is Focal value && this == value;
	    public override int GetHashCode() => 13 * Key + 17 * TraitKey + 23 * StartKey + 29 * EndKey;
    }
}
