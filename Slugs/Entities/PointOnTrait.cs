using System;
using System.Collections.Generic;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
	public class PointOnTrait : PointBase
	{
		public override ElementKind ElementKind => ElementKind.PointOnTrait;
		public override IElement EmptyElement => Empty;
	    public static readonly PointOnTrait Empty = new PointOnTrait();
        private PointOnTrait():base(true) { }

        public int TraitKey { get; set; }
        public float T { get; set; }

        public PointOnTrait(PadKind padKind, int traitKey, float t) : base(padKind)
        {
		    TraitKey = traitKey;
		    T = t;
        }

        public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { this };
        public override SKPoint Position
        {
	        get
	        {
                var trait = Pad.TraitAt(TraitKey);
                return trait.IsEmpty ? SKPoint.Empty : trait.PointAlongLine(T);
	        }
	        set
	        {
		        var trait = Pad.TraitAt(TraitKey);
		        T = trait.TFromPoint(value, false).Item1;
	        }
        }

        public Entity GetEntity() => Pad.EntityAt(GetTrait().EntityKey);
        public Trait GetTrait() => Pad.TraitAt(TraitKey);
        public float GetT() => T;
        public IPoint GetStartPoint() => Pad.TraitAt(TraitKey).StartPoint;
        public IPoint GetEndPoint() => Pad.TraitAt(TraitKey).EndPoint;

        public bool ReplaceWith(IPoint to)
        {
	        var result = false;
	        if (Key != -1)
	        {
		        Pad.SetPointAt(Key, to);
		        result = true;
	        }
	        return result;
        }

        public void Update(int traitKey, float t)
        {
	        TraitKey = traitKey;
	        T = t;
        }

        public static bool operator ==(PointOnTrait left, PointOnTrait right) =>
	        left.Key == right.Key && left.TraitKey == right.TraitKey;
        public static bool operator !=(PointOnTrait left, PointOnTrait right) =>
	        left.Key != right.Key || left.TraitKey != right.TraitKey;

        public override bool Equals(object obj) => obj is PointOnTrait value && this == value;
        public override int GetHashCode() => Key * 23 + TraitKey * 31;
    }
}
