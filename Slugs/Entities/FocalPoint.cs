using System;
using System.Collections.Generic;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
	public class FocalPoint : PointBase
	{
		public override ElementKind ElementKind => ElementKind.FocalPoint;
		public override IElement EmptyElement => Empty;
	    public static readonly FocalPoint Empty = new FocalPoint();
        private FocalPoint():base(true) { }

        public int TraitKey { get; set; }
        public Trait Trait => Pad.TraitAt(TraitKey);
        public float T { get; set; }

        public FocalPoint(PadKind padKind, int traitKey, float t) : base(padKind)
        {
		    TraitKey = traitKey;
		    T = t;
        }

        public override bool CanMergeWith(IPoint point)
        {
	        var kind = point.TargetPoint.ElementKind;

            return kind == ElementKind.FocalPoint || kind == ElementKind.RefPoint || kind == ElementKind.Terminal;
        }
        public override int MergeInto(IPoint point)
        {
	        T = Trait.TFromPoint(point.Position).Item1;

            var result = Key;
	        //if (point.ElementKind == ElementKind.FocalPoint)
	        //{
		       // Pad.SetPointAt(Key, point);
		       // result = point.Key;
	        //}
	        //else
	        //{
		       // T = Trait.TFromPoint(point.Position).Item1;
	        //}
	        return result;
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

        public static bool operator ==(FocalPoint left, FocalPoint right) =>
	        left.Key == right.Key && left.TraitKey == right.TraitKey;
        public static bool operator !=(FocalPoint left, FocalPoint right) =>
	        left.Key != right.Key || left.TraitKey != right.TraitKey;

        public override bool Equals(object obj) => obj is FocalPoint value && this == value;
        public override int GetHashCode() => Key * 23 + TraitKey * 31;
    }
}
