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
        //SKPoint[] SKPoints { get; }
    }

    public abstract class ElementBase : IElement
    {
	    protected static int KeyCounter = 1;
	    protected const int EmptyKeyValue = -99;

        public PadKind PadKind { get; set; }
		public int Key { get; set; }
		public ElementKind ElementKind => ElementKind.None;
		public IElement EmptyElement => null;
		public bool IsEmpty => Key == EmptyKeyValue;

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
