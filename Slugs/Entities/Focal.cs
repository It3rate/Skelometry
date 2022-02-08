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

        public int EntityKey { get; }

        public Entity Entity => Pad.EntityAt(EntityKey);
        public int TraitKey => StartPoint.TraitKey;
        public Trait Trait => Pad.TraitAt(TraitKey);
        public FocalPoint StartPoint => (FocalPoint)Pad.PointAt(StartKey);
        public FocalPoint EndPoint => (FocalPoint)Pad.PointAt(EndKey);
        public override SKPoint StartPosition => Trait.PointAlongLine(Slug.Start);
        public override SKPoint EndPosition => Trait.PointAlongLine(Slug.End);

        public Slug Slug => new Slug(StartPoint.T, EndPoint.T);

        public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { StartPoint, EndPoint };

        // trait instances in an entity are traits with a unit, that are contained in one or more entities.
        // maybe a bond is just two focals connected (on the same or different traits),
        //    with the option of intermediate repetition, scaling and directionality (causation)

        //public Focal(Trait trait, float startT, float endT) : base(trait.PadKind)
        //{
        //    var startPoint = new FocalPoint(PadKind, trait.Key, startT);
	       // StartKey = startPoint.Key;
	       // var endPoint = new FocalPoint(PadKind, trait.Key, endT);
        //    EndKey = endPoint.Key;
        //}
        public Focal(Entity entity, FocalPoint startPoint, FocalPoint endPoint) : base(entity.PadKind)
        {
	        if (startPoint.TraitKey != endPoint.TraitKey)
	        {
		        throw new ArgumentException("AddedFocal points must be on the same trait.");
	        }
	        SetStartKey(startPoint.Key);
	        SetEndKey(endPoint.Key);
        }

        protected override void SetStartKey(int key)
        {
	        if (Pad.ElementAt(key) is FocalPoint)
	        {
		        base.SetStartKey(key);
	        }
	        else
	        {
		        throw new ArgumentException("Focal points must be FocalPoint.");
	        }
        }
        protected override void SetEndKey(int key)
        {
	        if (Pad.ElementAt(key) is FocalPoint)
	        {
		        base.SetEndKey(key);
	        }
	        else
	        {
		        throw new ArgumentException("Focal points must be FocalPoint.");
	        }
        }

        private readonly HashSet<int> _bondStartKeys = new HashSet<int>();
        private readonly HashSet<int> _bondEndKeys = new HashSet<int>();
        public IEnumerable<Bond> Bonds
        {
	        get
	        {
		        foreach (var key in _bondStartKeys)
		        {
			        yield return Pad.BondAt(key);
		        }
	        }
        }

        public void AddBond(Bond bond) // todo: Add bonds from entity so able to verify focals belong to same entity.
        {
	        if (bond.StartPoint.FocalKey == Key)
	        {
		        _bondStartKeys.Add(bond.Key);
	        }
	        else if (bond.EndPoint.FocalKey == Key)
	        {
		        _bondEndKeys.Add(bond.Key);
	        }
	        else
	        {
		        throw new ArgumentException("Bond added to focal that doesn't belong to focal.");
	        }
        }
        public void RemoveBond(Bond bond)
        {
	        if (bond.StartPoint.FocalKey == Key)
	        {
		        _bondStartKeys.Remove(bond.Key);
	        }
	        else if (bond.EndPoint.FocalKey == Key)
	        {
		        _bondEndKeys.Remove(bond.Key);
	        }
	        else
	        {
		        throw new ArgumentException("Bond added to focal that doesn't belong to focal.");
	        }
        }

        public static bool operator ==(Focal left, Focal right) => left.Key == right.Key && left.EntityKey == right.EntityKey && left.TraitKey == right.TraitKey && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
        public static bool operator !=(Focal left, Focal right) => left.Key != right.Key || left.EntityKey != right.EntityKey || left.TraitKey != right.TraitKey || left.StartKey != right.StartKey || left.EndKey != right.EndKey;
	    public override bool Equals(object obj) => obj is Focal value && this == value;
	    public override int GetHashCode() => 13 * Key + 17 * EntityKey + 19 * TraitKey + 23 * StartKey + 29 * EndKey;
    }
}
