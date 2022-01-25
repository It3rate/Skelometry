﻿using System;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
	public class VPoint : PointBase
	{
		public override ElementKind ElementKind => ElementKind.RefPoint;
		public override IElement EmptyElement => Empty;
	    public static readonly VPoint Empty = new VPoint();
        private VPoint():base(true) { }

        public int EntityKey { get; set; } // if motor index < 0, use cached point.
        public int TraitKey { get; set; }
        public int FocalKey { get; set; }

        public VPoint(PadKind padKind, int entityKey, int traitKey, int focalKey) : base(padKind)
        {
		    EntityKey = entityKey;
		    TraitKey = traitKey;
            FocalKey = focalKey;
        }

        public override IPoint[] Points => IsEmpty ? new IPoint[] { } : new IPoint[] { this };
        public override SKPoint SKPoint
        {
	        get
	        {
                var trait = Pad.TraitAt(TraitKey);
                var focal = Pad.FocalAt(FocalKey);
                return trait.IsEmpty ? SKPoint.Empty : trait.PointAlongLine(focal.T);
	        }
	        set => throw new InvalidOperationException("Can't set location of virtual point."); // not impossible to adjust terminal points?
        }

        public Entity GetEntity() => Pad.EntityAt(EntityKey);
        public Trait GetTrait() => Pad.TraitAt(TraitKey);
        public Focal GetFocal() => Pad.FocalAt(FocalKey);
        public float GetT() => Pad.FocalAt(FocalKey).T;
        public IPoint GetStartPoint() => Pad.TraitAt(TraitKey).StartRef;
        public IPoint GetEndPoint() => Pad.TraitAt(TraitKey).EndRef;


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

        public void Update(PadKind padKind, int entityKey, int traitKey, int focalKey)
        {
	        PadKind = padKind;
	        EntityKey = entityKey;
	        TraitKey = traitKey;
	        FocalKey = focalKey;
        }

        public void CopyValuesFrom(IPoint from)
        {
            PadKind = from.PadKind;
	        if (from is VPoint vp)
	        {
		        //SelectionKind = vp.SelectionKind;
		        EntityKey = vp.EntityKey;
		        TraitKey = vp.TraitKey;
		        FocalKey = vp.FocalKey;
	        }
	        else
	        {
		        //SelectionKind = PointKind.Reference;
		        Key = from.Key;
	        }
        }
        //public static bool operator ==(VPoint left, IPoint right) => left.Key == right.Key;
        //public static bool operator !=(VPoint left, IPoint right) => left.Key != right.Key;
        //public override bool Equals(object obj) => obj is VPoint value && this == value;
        //public bool Equals(IPoint value) => this == value;
        //public override int GetHashCode() => Key.GetHashCode();// 17 * 23 + PadKind.GetHashCode() * 29 + TraitKey.GetHashCode() * 31 + EntityKey.GetHashCode() * 37 + FocalKey.GetHashCode();
    }
}
