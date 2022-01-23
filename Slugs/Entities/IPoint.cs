using System;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
	public interface IPoint : IElement, IEquatable<IPoint>
    {
        //PointKind Kind { get; }
        SKPoint SKPoint { get; set; }
        //bool ReplaceWith(IPoint pt);
	    //void CopyValuesFrom(IPoint from);
    }

	public abstract class PointBase : ElementBase, IPoint
	{
		public abstract SKPoint SKPoint { get; set; }
		public PointBase(PadKind padKind) : base(padKind)
		{
		}

		public abstract bool Equals(IPoint other);

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
