using System;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Agent;
using Slugs.Pads;
using Slugs.Slugs;

namespace Slugs.Entities
{
	[Flags]
    public enum PointKind
    {
        Terminal,
        Cached,
        Dirty,
        Dynamic,
        NeedsUpdate = Dirty & Dynamic,
    }
    public static class PointKindExtensions
    {
	    public static bool NeedsUpdate(this PointKind grade) => (grade & (PointKind.Dirty | PointKind.Dynamic)) != 0;
    }

    public class PtRef : IPointRef, IEquatable<PtRef>
    {
        public static readonly PtRef Empty = new PtRef(-1,-1,-1, -1, SKPoint.Empty);
        public bool IsEmpty => EntityKey == -1 && CachedPoint == SKPoint.Empty;

        public int PadIndex { get; set; }
        public int EntityKey { get; set; } // if motor index < 0, use cached point.
        public int TraitKey { get; set; }
        public int FocalKey { get; set; }
        private PointKind Kind { get; set; }
        private SKPoint CachedPoint { get; set; }

        public PtRef(int padIndex, int entityIndex, int traitKey, int tFocalIndex, SKPoint cachedPoint)
	    {
		    PadIndex = padIndex;
		    EntityKey = entityIndex;
		    TraitKey = traitKey;
            FocalKey = tFocalIndex;
		    Kind = entityIndex < 0 ? PointKind.Terminal : PointKind.Dirty;
		    CachedPoint = cachedPoint;
        } 
        public SKPoint SKPoint
        {
	        get
	        {
		        if (Kind == PointKind.NeedsUpdate)
		        {
			        CachedPoint = Entity.GetPointAt(T);
			        Kind = (Kind == PointKind.Dirty) ? PointKind.Cached : Kind;
		        }
		        return CachedPoint;
	        }
	        set
	        {
		        if (Kind == PointKind.Terminal)
		        {
			        CachedPoint = value;
		        }
		        else
		        {
			        Entity.SetPointAt(T, value);
		        }
            }
        }

        public bool ReplaceWith(IPointRef to)
        {
	        var result = false;
	        var key = Pad.KeyForPtRef(this);
	        if (key != -1)
	        {
		        Pad.SetPtRef(key, to);
		        result = true;
	        }
	        return result;
        }

        public EntityPad Pad => EntityAgent.ActiveAgent.PadAt(PadIndex);
	    public PadData Data => EntityAgent.ActiveAgent.PadAt(PadIndex).Data;
        public Slug T => Data.FocalFromIndex(FocalKey);
        public Entity Entity => Data.EntityFromIndex(EntityKey);

        public static bool operator ==(PtRef left, PtRef right) =>
	        left.PadIndex == right.PadIndex && left.EntityKey == right.EntityKey && left.TraitKey == right.TraitKey && left.FocalKey == right.FocalKey;
        public static bool operator !=(PtRef left, PtRef right) => 
	        left.PadIndex != right.PadIndex || left.EntityKey != right.EntityKey || left.TraitKey != right.TraitKey || left.FocalKey != right.FocalKey;
        public override bool Equals(object obj) => obj is PtRef value && this == value;
        public bool Equals(PtRef value) => this == value;
        public override int GetHashCode() => 17 * 23 + PadIndex.GetHashCode() * 29 + TraitKey.GetHashCode() * 31 + EntityKey.GetHashCode() * 37 + FocalKey.GetHashCode();
    }
}
