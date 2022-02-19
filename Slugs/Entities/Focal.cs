using System;
using System.Collections.Generic;
using System.Numerics;
using SkiaSharp;
using Slugs.Constraints;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
    // Eventually Focals will allow multiple segments to multiply/square/cube etc, as well as have probability and fuzzy endpoints.
    // These are slug collections with bonds between them and causation direction.
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
        public TraitKind TraitKind => Pad.TraitAt(TraitKey).TraitKind;

        public float Direction => StartPoint.T <= EndPoint.T ? 1f : -1f;
        public float ZeroT => FocalUnit.StartPoint.T;
        public FocalPoint StartPoint => (FocalPoint)Pad.PointAt(StartKey);
        public FocalPoint EndPoint => (FocalPoint)Pad.PointAt(EndKey);
        public FocalPoint OtherPoint(int notKey) => (notKey == StartKey) ? EndPoint : (notKey == EndKey) ? StartPoint : FocalPoint.Empty;

        public Slug LocalRatio
        {
	        get => new Slug(StartT, EndT);
	        set
	        {
		        StartPoint.T = (float)value.Imaginary;
		        EndPoint.T = (float)value.Real;
	        }
        }
        public Slug UnitSlug => Pad.UnitFor(TraitKind);
        public Slug AsUnitSlug => (Direction >= 0 ? Slug.Unit : Slug.Unot);

        public float StartT => StartPoint.T;
        public float EndT => EndPoint.T;
        public float LengthT => (float)(LocalSlug.DirectedLength() / FocalUnit.LocalSlug.DirectedLength());
        public override SKPoint StartPosition => Trait.PointAlongLine(StartPoint.T);
        public override SKPoint EndPosition => Trait.PointAlongLine(EndPoint.T);

        public bool IsUnit => Pad.UnitKeyFor(TraitKind) == Key;
        public float UnitLength => (float)Pad.UnitFor(TraitKind).DirectedLength();
        public Focal FocalUnit => Pad.FocalUnitFor(this);

        public Slug Slug
        {
	        get => IsUnit ? AsUnitSlug : LocalSlug - UnitSlug.Imaginary;
	        set
	        {
		        var focalUnit = FocalUnit;
		        if (!focalUnit.IsEmpty && focalUnit.Key != Key)
		        {
			        LocalSlug = focalUnit.Slug * value;
		        }
	        }
        }

        public Slug LocalSlug
        {
	        get => new Slug(StartPoint.T, EndPoint.T);
	        set
	        {
		        StartPoint.T = (float) value.Imaginary;
		        EndPoint.T = (float)value.Real;
	        }
        }

        public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { StartPoint, EndPoint };

        public Focal(Entity entity, FocalPoint startPoint, FocalPoint endPoint) : base(entity.PadKind)
        {
	        if (startPoint.TraitKey != endPoint.TraitKey)
	        {
		        throw new ArgumentException("AddedFocal points must be on the same trait.");
	        }
            EntityKey = entity.Key;
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
        public IEnumerable<SingleBond> StartBonds
        {
	        get
	        {
		        foreach (var key in _bondStartKeys)
		        {
			        yield return Pad.BondAt(key);
		        }
	        }
        }
        public IEnumerable<SingleBond> EndBonds
        {
	        get
	        {
		        foreach (var key in _bondEndKeys)
		        {
			        yield return Pad.BondAt(key);
		        }
	        }
        }
        public IEnumerable<SingleBond> AllBonds
        {
	        get
	        {
		        foreach (var key in _bondStartKeys)
		        {
			        yield return Pad.BondAt(key);
		        }
                foreach (var key in _bondEndKeys)
		        {
			        yield return Pad.BondAt(key);
		        }
	        }
        }

        public void AddBond(SingleBond singleBond) // todo: Add bonds from entity so able to verify focals belong to same entity.
        {
	        if (singleBond.StartPoint.FocalKey == Key)
	        {
		        _bondStartKeys.Add(singleBond.Key);
	        }
	        else if (singleBond.EndPoint.FocalKey == Key)
	        {
		        _bondEndKeys.Add(singleBond.Key);
	        }
	        else
	        {
		        throw new ArgumentException("SingleBond added to focal that doesn't belong to focal.");
	        }
        }
        public void RemoveBond(SingleBond singleBond)
        {
	        if (singleBond.StartPoint.FocalKey == Key)
	        {
		        _bondStartKeys.Remove(singleBond.Key);
	        }
	        else if (singleBond.EndPoint.FocalKey == Key)
	        {
		        _bondEndKeys.Remove(singleBond.Key);
	        }
	        else
	        {
		        throw new ArgumentException("SingleBond added to focal that doesn't belong to focal.");
	        }
        }

        public static bool operator ==(Focal left, Focal right) => left.Key == right.Key && left.EntityKey == right.EntityKey && left.TraitKey == right.TraitKey && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
        public static bool operator !=(Focal left, Focal right) => left.Key != right.Key || left.EntityKey != right.EntityKey || left.TraitKey != right.TraitKey || left.StartKey != right.StartKey || left.EndKey != right.EndKey;
	    public override bool Equals(object obj) => obj is Focal value && this == value;
	    public override int GetHashCode() => 13 * Key + 17 * EntityKey + 19 * TraitKey + 23 * StartKey + 29 * EndKey;
    }
}
