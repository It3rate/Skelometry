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

        private static int _counter = int.MaxValue / 2;

        public int Key { get; }
        public int PadIndex { get; set; }
        public int EntityKey { get; set; } // if motor index < 0, use cached point.
        public int TraitKey { get; set; }
        public int FocalKey { get; set; }

        public PointKind Kind => PointKind.Virtual;
        public bool IsEmpty => Key == -1;

        public VPoint(int padIndex, int entityIndex, int traitKey, int focalIndex)
        {
	        Key = _counter++;
		    PadIndex = padIndex;
		    EntityKey = entityIndex;
		    TraitKey = traitKey;
            FocalKey = focalIndex;
        } 
        public SKPoint SKPoint
        {
	        get => Agent.Current.SKPointFor(this);
	        set => throw new InvalidOperationException("Can't set location of virtual point."); // not impossible to adjust terminal points?
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

        public Pad Pad => Agents.Agent.Current.PadAt(PadIndex);
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
