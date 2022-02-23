using System;
using System.Collections.Generic;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public interface IPoint : IElement
    {
        //PointKind SelectionKind { get; }
        SKPoint Position { get; set; }
        //bool ReplaceWith(IPoint pt);
        //void CopyValuesFrom(IPoint from);
        bool CanMergeWith(IPoint point);
         IPoint TargetPoint { get; }
        int MergeInto(IPoint point);
    }

	public abstract class PointBase : ElementBase, IPoint
	{
		public abstract SKPoint Position { get; set; }

		protected PointBase(bool isEmpty) : base(isEmpty) { }
		protected PointBase(PadKind padKind) : base(padKind) { }

        public override SKPath Path {
			get
			{
                var path = new SKPath();
                path.AddCircle(Position.X, Position.Y, 1);
                return path;
			}
		}
        public override float DistanceToPoint(SKPoint point)
		{
			return point.DistanceTo(Position);
		}

		public abstract bool CanMergeWith(IPoint point);
		public virtual IPoint TargetPoint => this;

		public virtual int MergeInto(IPoint point)
		{
            Pad.SetPointAt(Key, point);
            return point.Key;
		}

        //public static bool operator ==(PointBase left, IPoint right) => left.Key == right.Key;
        //public static bool operator !=(PointBase left, IPoint right) => left.Key != right.Key;
        //public override bool Equals(object obj) => obj is PointBase value && this == value;
        //public override int GetHashCode() => Key.GetHashCode();
    }

    //[Flags]
    //public enum PointKind
    //{
	   // Terminal,
	   // Reference,
    //    Virtual,
    //}
    //public static class PointKindExtensions
    //{
	   // public static bool IsCalculated(this PointKind kind) => (kind == PointKind.Virtual); // fill in as needed
	   // //public static bool NeedsUpdate(this PointKind grade) => (grade & (PointKind.Dirty | PointKind.Dynamic)) != 0;
    //}
}
