using System.Windows.Forms.VisualStyles;
using SkiaSharp;
using Slugs.Pads;

namespace Slugs.Entities
{
	public interface IElement
    {
	    PadKind PadKind { get; set; }
        ElementKind ElementKind { get; }
        int Key { get; }
        IElement EmptyElement { get; }
        bool IsEmpty { get; }
        //IPoint[] TerminalPoints { get; }
        //IPoint[] Points { get; }
        //SKPoint[] SKPoints { get; } // maybe just use enumerators
    }

    public abstract class ElementBase : IElement
    {
	    protected static int KeyCounter = 1;
	    protected const int EmptyKeyValue = -99;

        public PadKind PadKind { get; set; }
		public int Key { get; set; }
		public virtual ElementKind ElementKind => ElementKind.None;
		public virtual IElement EmptyElement => null;
		public virtual bool IsEmpty => Key == EmptyKeyValue;
		//public SKPoint[] SKPoints => null;

		protected ElementBase(bool isEmpty) // used to privately create an empty element
		{
			Key = EmptyKeyValue;
		}
		protected ElementBase(PadKind padKind)
		{
			Key = KeyCounter++;
			PadKind = padKind;
		}
    }
	public enum ElementKind
	{
		None,
		Terminal,
		Point,
		VPoint,
        Trait,
		Focal,
		Bond,
		Entity,
		Unit,
		Group,
		SelectionGroup,
        PadProjection,
        Grid,
	}
	public static class SelectionKindExtensions
	{
		public static bool HasSnap(this ElementKind kind) => (kind != ElementKind.None && kind != ElementKind.Terminal);
		public static bool IsPoint(this ElementKind kind) => (kind == ElementKind.Terminal || kind == ElementKind.Point || kind == ElementKind.VPoint);
    }
}
