using System;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
	public interface IPoint : IElement, IEquatable<IPoint>
    {
        //PointKind SelectionKind { get; }
        SKPoint SKPoint { get; set; }
        //bool ReplaceWith(IPoint pt);
	    //void CopyValuesFrom(IPoint from);
    }

	public abstract class PointBase : ElementBase, IPoint
	{
		public abstract SKPoint SKPoint { get; set; }

		protected PointBase(bool isEmpty) : base(isEmpty) { }
		protected PointBase(PadKind padKind) : base(padKind) { }

        public static bool operator ==(PointBase left, IPoint right) => left.Key == right.Key;
        public static bool operator !=(PointBase left, IPoint right) => left.Key != right.Key;
        public override bool Equals(object obj) => obj is PointBase value && this == value;
        public bool Equals(IPoint value) => this == value;
        public override int GetHashCode() => Key.GetHashCode();
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
