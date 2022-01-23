using System.Windows.Forms.VisualStyles;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;

namespace Slugs.Entities
{
	public interface IElement
    {
	    PadKind PadKind { get; set; }
	    Pad Pad { get; }
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
		public virtual bool IsEmpty => Key == EmptyKeyValue;

        public PadKind PadKind { get; set; }
        public Pad Pad => Agent.Current.PadAt(PadKind);
		public int Key { get; set; }

		public abstract IElement EmptyElement { get; }
		public abstract ElementKind ElementKind { get; }
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
		RefPoint,
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
		public static bool IsPoint(this ElementKind kind) => (kind == ElementKind.Terminal || kind == ElementKind.RefPoint || kind == ElementKind.VPoint);
    }
}
