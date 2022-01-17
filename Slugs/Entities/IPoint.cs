using System;
using SkiaSharp;

namespace Slugs.Entities
{
	public interface IPoint : IEquatable<IPoint>
    {
	    int PadIndex { get; set; }
        int Key { get; }
        //int EntityKey { get; set; }
        //int TraitKey { get; set; }
        //int FocalKey { get; set; }
        PointKind Kind { get; set; }
        SKPoint SKPoint { get; set; }
	    bool IsEmpty { get; }

	    bool ReplaceWith(IPoint pt);
    }

    [Flags]
    public enum PointKind
    {
	    Terminal,
	    Pointer,
	    Dirty,
	    Dynamic,
	    NeedsUpdate = Dirty & Dynamic,
    }
    public static class PointKindExtensions
    {
	    public static bool NeedsUpdate(this PointKind grade) => (grade & (PointKind.Dirty | PointKind.Dynamic)) != 0;
    }
}
