using SkiaSharp;

namespace Slugs.Entities
{
	public interface IElement
    {
        ElementKind ElementKind { get; }
        int Key { get; }
        //int Parent { get; }
        //int[] Children { get; }
        //IPoint[] Points { get; }
        //SKPoint[] SKPoints { get; }
    }

	public enum ElementKind
	{
		None,
		Terminal,
		Point,
		Trait,
		Focal,
		Bond,
		Entity,
		Unit,
		Grid,
	}
	public static class SelectionKindExtensions
	{
		public static bool HasSnap(this ElementKind kind) => (kind != ElementKind.None && kind != ElementKind.Terminal);
	}
}
