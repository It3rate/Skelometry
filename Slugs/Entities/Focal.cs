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
        private Focal() : base(true) {Slug = Slug.Empty;}

        public int TraitKey { get; }
        public Trait Trait => Pad.TraitAt(TraitKey);
        public Slug Slug { get; }
	    public float Focus { get; set; }
        public float T => Slug.IsZeroLength ? 0 : (float)(Slug.Length() / Focus + Slug.Start);

        public override SKPoint StartPosition => Trait.PointAlongLine(Slug.Start);
        public override SKPoint EndPosition => Trait.PointAlongLine(Slug.End);
        public override SKSegment Segment => new SKSegment(StartPosition, EndPosition);

        public override List<IPoint> Points => new List<IPoint>();

        // focals (vpoint segments) are really just a trait ref and a slug. A bond is just a focal with a second trait ref.
        // trait instances in an entity are traits with a unit, that are contained in one or more entities.

        // maybe a bond is just two focals connected (on the same or different traits),
        //    with the option of intermediate repetition, scaling and directionality (causation)
        public Focal(PadKind padKind, PointOnTrait start, PointOnTrait end, float focus) : base(padKind, start.Key, end.Key)
        {
	        Focus = focus;
	        Slug = new Slug(start.GetT(), end.GetT());
        }

        public Focal(PadKind padKind, Slug slug, float focus) : base(padKind, -1, -1)
        {
		    Slug = slug;
		    Focus = focus;
            //StartKey = new PointOnTrait(padKind,); // does this need to be TraitPoints and FocalPoints?
        }

	    public static bool operator ==(Focal left, Focal right) => left.Key == right.Key && left.Slug == right.Slug && left.Focus == right.Focus;
        public static bool operator !=(Focal left, Focal right) => left.Key != right.Key || left.Slug != right.Slug || left.Focus != right.Focus;
	    public override bool Equals(object obj) => obj is Focal value && this == value;
	    public override int GetHashCode() => Key.GetHashCode() + 17*Slug.GetHashCode() + 23*Focus.GetHashCode();
    }
}
