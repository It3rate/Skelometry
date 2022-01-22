using System;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
	public interface IPoint : IElement, IEquatable<IPoint>
    {
        PointKind Kind { get; }
        SKPoint SKPoint { get; set; }
        //Pad GetPad();
	    //Entity GetEntity();
	    //Trait GetTrait();
	    //Focal GetFocal();
     //   float GetT();

        bool ReplaceWith(IPoint pt);
	    void CopyValuesFrom(IPoint from);
    }

    [Flags]
    public enum PointKind
    {
	    Terminal,
	    Reference,
        Virtual,
    }
    public static class PointKindExtensions
    {
	    public static bool IsCalculated(this PointKind kind) => (kind == PointKind.Virtual); // fill in as needed
	    //public static bool NeedsUpdate(this PointKind grade) => (grade & (PointKind.Dirty | PointKind.Dynamic)) != 0;
    }
}
