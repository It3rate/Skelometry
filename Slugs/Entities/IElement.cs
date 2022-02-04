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
	    PadKind PadKind { get; }
	    Pad Pad { get; }
        ElementKind ElementKind { get; }
        int Key { get; }
        IElement EmptyElement { get; }
        bool IsLocked { get; }
        bool IsEmpty { get; }

        //IPoint[] TerminalPoints { get; }
        List<IPoint> Points { get; }
        List<SKPoint> SKPoints{ get; }

        void MoveTo(SKPoint position);
        void SetLock(bool lockStatus);
    }

    public abstract class ElementBase : IElement
    {
	    public const int EmptyKeyValue = -99;
	    public virtual bool IsEmpty => Key == EmptyKeyValue;
	    public static bool IsEmptyKey(int key) => key == EmptyKeyValue;

        public Pad Pad { get; }
	    public PadKind PadKind => Pad.PadKind;
		public int Key { get; }

        public abstract IElement EmptyElement { get; }
		public abstract ElementKind ElementKind { get; }
		public abstract List<IPoint> Points { get; }
		public List<SKPoint> SKPoints
		{
			get
			{
				var pts = Points;
				var result = new List<SKPoint>(pts.Count);
				foreach (var point in pts)
				{
					result.Add(point.Position);
				}
				return result;
			}
		}
		public bool IsLocked { get; protected set; }

        protected ElementBase(bool isEmpty) // used to privately create an empty element
        {
	        Pad = Agent.Current.PadFor(PadKind.None);
            Key = EmptyKeyValue;
		}
		protected ElementBase(PadKind padKind)
		{
			Pad = Agent.Current.PadFor(padKind);
			Key = Pad.KeyCounter++;
			Pad.AddElement(this);
        }
	    public virtual void MoveTo(SKPoint position)
	    {
		    foreach (var point in Points)
		    {
			    point.Position = position;
		    }
	    }

	    public void SetLock(bool lockStatus)
	    {
		    IsLocked = lockStatus;
	    }
    }

    [Flags]
	public enum ElementKind
	{
		None = 0,
		Terminal = 0x1,
		RefPoint = 0x2,
		PointOnTrait = 0x4,
		PointOnFocal = 0x8,
        Trait = 0x10,
		Focal = 0x20,
		Bond = 0x40,
		Entity = 0x80,
		Unit = 0x100,
		Group = 0x200,

		Grid = 0x400,
		SelectionRect = 0x800,
        Ruler = 0x1000,
        Value = 0x2000,

        Any = 0xFFFF,

        PointKind = RefPoint | Terminal | PointOnTrait | PointOnFocal,
        SegmentKind = Trait | Focal | Bond,
        TraitPart = Trait | Terminal | RefPoint,
        FocalPart = Focal | Unit | PointOnTrait,
        BondPart = Bond | PointOnFocal,
        UIPart = Grid | SelectionRect | Ruler | Value,
    }
	public static class SelectionKindExtensions
	{
		public static bool IsNone(this ElementKind kind) => kind == ElementKind.None;
		public static bool IsPoint(this ElementKind kind) => (int) (kind & ElementKind.PointKind) > 0;// ElementKind.PointKind.HasFlag(kind);
		public static bool IsTerminal(this ElementKind kind) => kind == ElementKind.Terminal;
		public static bool IsCompatible(this ElementKind self, ElementKind other) => (int)(self & other) != 0;
    }
}
