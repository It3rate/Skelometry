using System;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
	public class VPoint : IPoint
	{
		public ElementKind ElementKind => ElementKind.Point;

	    public static VPoint Empty = new VPoint();
	    private VPoint() { Key = -99; }
	    public bool IsEmpty => Key == -99;

        private static int _counter = int.MaxValue / 2;

        public PointKind Kind { get; private set; } = PointKind.Virtual; // all points could be vpoints if adding cached SkPoint, not doing this atm as terminal points are external.
        public int Key { get; private set; }
        public PadKind PadKind { get; set; }
        public int EntityKey { get; set; } // if motor index < 0, use cached point.
        public int TraitKey { get; set; }
        public int FocalKey { get; set; }

        public VPoint(PadKind padKind, int entityKey, int traitKey, int focalKey)
        {
	        Key = _counter++;
		    PadKind = padKind;
		    EntityKey = entityKey;
		    TraitKey = traitKey;
            FocalKey = focalKey;
        } 
        public SKPoint SKPoint
        {
	        get => Agent.Current.SKPointFor(this);
	        set => throw new InvalidOperationException("Can't set location of virtual point."); // not impossible to adjust terminal points?
        }

        public Pad GetPad() => Agent.Current.PadAt(PadKind);
        public Entity GetEntity() => Agent.Current.PadAt(PadKind).EntityAt(EntityKey);
        public Trait GetTrait() => Agent.Current.PadAt(PadKind).EntityAt(EntityKey).TraitAt(TraitKey);
        public Focal GetFocal() => Agent.Current.PadAt(PadKind).FocalAt(FocalKey);
        public float GetT() => Agent.Current.PadAt(PadKind).FocalAt(FocalKey).T;
        public IPoint GetStartPoint() => Agent.Current.PadAt(PadKind).EntityAt(EntityKey).TraitAt(TraitKey).StartRef;
        public IPoint GetEndPoint() => Agent.Current.PadAt(PadKind).EntityAt(EntityKey).TraitAt(TraitKey).EndRef;


        public bool ReplaceWith(IPoint to)
        {
	        var result = false;
	        if (Key != -1)
	        {
		        Agent.Current.SetPointAt(Key, to);
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
		        Kind = vp.Kind;
		        EntityKey = vp.EntityKey;
		        TraitKey = vp.TraitKey;
		        FocalKey = vp.FocalKey;
	        }
	        else
	        {
		        Kind = PointKind.Reference;
		        Key = from.Key;
	        }
        }
        public static bool operator ==(VPoint left, IPoint right) => left.Key == right.Key;
        public static bool operator !=(VPoint left, IPoint right) => left.Key != right.Key;
        public override bool Equals(object obj) => obj is VPoint value && this == value;
        public bool Equals(IPoint value) => this == value;
        public override int GetHashCode() => Key.GetHashCode();// 17 * 23 + PadKind.GetHashCode() * 29 + TraitKey.GetHashCode() * 31 + EntityKey.GetHashCode() * 37 + FocalKey.GetHashCode();
    }
}
