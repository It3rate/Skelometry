using System;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;
using Slugs.Slugs;

namespace Slugs.Entities
{
	public class VPoint : IPoint
    {
        public static readonly VPoint Empty = new VPoint(-1,-1,-1, -1, SKPoint.Empty);
        public bool IsEmpty => EntityKey == -1 && CachedPoint == SKPoint.Empty;

        private static int _counter = int.MaxValue / 2;

        public int Key { get; }
        public int PadIndex { get; set; }
        public int EntityKey { get; set; } // if motor index < 0, use cached point.
        public int TraitKey { get; set; }
        public int FocalKey { get; set; }
        public PointKind Kind { get; set; }
        private SKPoint CachedPoint { get; set; }

        public VPoint(int padIndex, int entityIndex, int traitKey, int tFocalIndex, SKPoint cachedPoint)
        {
	        Key = _counter++;
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
		        return Entity.GetPointAt(T);
          //      if (Kind != PointKind.Terminal)
		        //{
			       // CachedPoint = Entity.GetPointAt(T);
			       // //Kind = (Kind == PointKind.Dirty) ? PointKind.Cached : Kind;
		        //}
		        //return CachedPoint;
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

        public EntityPad Pad => Agents.Agent.Current.PadAt(PadIndex);
	    public PadData Data => Agents.Agent.Current.PadAt(PadIndex).Data;
        public Slug T => Data.FocalFromIndex(FocalKey);
        public Entity Entity => Data.EntityFromIndex(EntityKey);

        public static bool operator ==(VPoint left, IPoint right) => left.Key == right.Key;
        public static bool operator !=(VPoint left, IPoint right) => left.Key != right.Key;
        public override bool Equals(object obj) => obj is VPoint value && this == value;
        public bool Equals(IPoint value) => this == value;
        public override int GetHashCode() => Key.GetHashCode();// 17 * 23 + PadIndex.GetHashCode() * 29 + TraitKey.GetHashCode() * 31 + EntityKey.GetHashCode() * 37 + FocalKey.GetHashCode();
    }
}
