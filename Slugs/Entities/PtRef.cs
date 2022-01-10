using SkiaSharp;
using Slugs.Agent;
using Slugs.Pads;
using Slugs.Slugs;

namespace Slugs.Motors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
        public bool IsEmpty => EntityIndex == -1 && _cachedPoint == SKPoint.Empty;

        public int PadIndex { get; }
	    public int EntityIndex { get; } // if motor index < 0, use cached point.
        public int FocalIndex { get; }
        private PointKind Kind { get; set; }
        private SKPoint _cachedPoint { get; set; }

        public PtRef(int padIndex, int entityIndex, int tFocalIndex, SKPoint cachedPoint)
	    {
		    PadIndex = padIndex;
		    EntityIndex = entityIndex;
		    FocalIndex = tFocalIndex;
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

        public SlugPad Pad => SlugAgent.Pads[PadIndex];
	    public PadData Data => SlugAgent.Pads[PadIndex].Data;
        public Slug T => Data.TFromIndex(FocalIndex);
        public Entity Entity => Data.MotorFromIndex(EntityIndex);

        public static bool operator ==(PtRef left, PtRef right) => left.PadIndex == right.PadIndex && left.EntityIndex == right.EntityIndex && left.FocalIndex == right.FocalIndex;
        public static bool operator !=(PtRef left, PtRef right) => left.PadIndex != right.PadIndex || left.EntityIndex != right.EntityIndex || left.FocalIndex != right.FocalIndex;
        public override bool Equals(object obj) => obj is PtRef value && this == value;
        public bool Equals(PtRef value) => this == value;
        public override int GetHashCode() => 17 * 23 + PadIndex.GetHashCode() * 29 + EntityIndex.GetHashCode() * 37 + FocalIndex.GetHashCode();
    }
}
