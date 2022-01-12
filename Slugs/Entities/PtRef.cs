using System;
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
        NeedsUpdate = Dirty | Dynamic,
    }

    public class PtRef : IPointRef, IEquatable<PtRef>
    {
        public static readonly PtRef Empty = new PtRef(-1,-1,-1,SKPoint.Empty);
        public bool IsEmpty => EntityKey == -1 && _cachedPoint == SKPoint.Empty;

        public int PadIndex { get; }
	    public int EntityKey { get; } // if motor index < 0, use cached point.
        public int TraitKey { get; }
        public int FocalKey { get; }
        private PointKind Kind { get; set; }
        private SKPoint _cachedPoint { get; set; }

        public PtRef(int padIndex, int entityIndex, int tFocalIndex, SKPoint cachedPoint)
	    {
		    PadIndex = padIndex;
		    EntityKey = entityIndex;
		    FocalKey = tFocalIndex;
		    Kind = entityIndex < 0 ? PointKind.Terminal : PointKind.Dirty;
		    _cachedPoint = cachedPoint;
        }
        public SKPoint SKPoint
        {
	        get
	        {
		        if (Kind == PointKind.NeedsUpdate)
		        {
			        _cachedPoint = Entity.GetPointAt(T);
			        Kind = (Kind == PointKind.Dirty) ? PointKind.Cached : Kind;
		        }
		        return _cachedPoint;
	        }
	        set
	        {
		        if (Kind == PointKind.Terminal)
		        {
			        _cachedPoint = value;
		        }
		        else
		        {
			        Entity.SetPointAt(T, value);
		        }
            }
        }

        //public bool ReplaceWith(PtRef ptRef)
        //{
        //    Entity.
        //}

        public SlugPad Pad => SlugAgent.ActiveAgent.PadAt(PadIndex);
	    public PadData Data => SlugAgent.ActiveAgent.PadAt(PadIndex).Data;
        public Slug T => Data.FocalFromIndex(FocalKey);
        public Entity Entity => Data.EntityFromIndex(EntityKey);

        public static bool operator ==(PtRef left, PtRef right) => left.PadIndex == right.PadIndex && left.EntityKey == right.EntityKey && left.FocalKey == right.FocalKey;
        public static bool operator !=(PtRef left, PtRef right) => left.PadIndex != right.PadIndex || left.EntityKey != right.EntityKey || left.FocalKey != right.FocalKey;
        public override bool Equals(object obj) => obj is PtRef value && this == value;
        public bool Equals(PtRef value) => this == value;
        public override int GetHashCode() => 17 * 23 + PadIndex.GetHashCode() * 29 + EntityKey.GetHashCode() * 37 + FocalKey.GetHashCode();
    }
}
