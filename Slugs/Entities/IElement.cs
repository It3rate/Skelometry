using System;
using System.Collections.Generic;
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
        IPoint[] Points { get; }
        SKPoint[] SKPoints{ get; }
    }

    public abstract class ElementBase : IElement
    {
	    protected static int KeyCounter = 1;
	    public const int EmptyKeyValue = -99;
	    public virtual bool IsEmpty => Key == EmptyKeyValue;
	    public static bool IsEmptyKey(int key) => key == EmptyKeyValue;

        public PadKind PadKind { get; set; }
        public Pad Pad => Agent.Current.PadAt(PadKind);
		public int Key { get; set; }

		public abstract IElement EmptyElement { get; }
		public abstract ElementKind ElementKind { get; }
		public abstract IPoint[] Points { get; }
		public SKPoint[] SKPoints
		{
			get
			{
				var pts = Points;
				var result = new List<SKPoint>(pts.Length);
				foreach (var point in pts)
				{
					result.Add(point.SKPoint);
				}
				return result.ToArray();
			}
		}
        //public SKPoint[] SKPoints => null;

        protected ElementBase(bool isEmpty) // used to privately create an empty element
		{
			Key = EmptyKeyValue;
		}
		protected ElementBase(PadKind padKind)
		{
			Key = KeyCounter++;
			PadKind = padKind;
			//if (padKind != PadKind.None) // used in empty definitions.
			{
				Pad.AddElement(this);
            }
        }
    }
    [Flags]
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

        PointKind = RefPoint | Terminal | VPoint,
        SegmentKind = Trait | Focal | Bond,
    }
	public static class SelectionKindExtensions
	{
		public static bool HasSnap(this ElementKind kind) => (kind != ElementKind.None && kind != ElementKind.Terminal);
		public static bool IsPoint(this ElementKind kind) => ElementKind.PointKind.HasFlag(kind);
		public static bool IsTerminal(this ElementKind kind) => kind == ElementKind.Terminal;
    }
}
