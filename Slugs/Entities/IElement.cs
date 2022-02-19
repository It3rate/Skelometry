using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Constraints;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public interface IElement
    {
	    PadKind PadKind { get; }
	    Pad Pad { get; }
        ElementKind ElementKind { get; }
        int Key { get; }
        IElement EmptyElement { get; }
        bool IsLocked { get; set; }
        bool IsEmpty { get; }
        bool HasArea { get; }
        SKPath Path { get; }
        List<IConstraint> Constraints { get; }

        //IPoint[] TerminalPoints { get; }
        List<IPoint> Points { get; }
        List<SKPoint> SKPoints { get; }
        float DistanceToPoint(SKPoint point);

        void MoveTo(SKPoint position);
        void SetLock(bool lockStatus);
    }

	public interface IAreaElement : IElement
	{
		bool ContainsPosition(SKPoint point);
	}
	public interface ISlugElement : IElement
	{
        Slug Ratio { get; set; }
		//SKSegment Segment { get; }
		//float Length { get; }
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
		public abstract SKPath Path { get; }
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

		public virtual bool HasArea => false;

		public List<IConstraint> Constraints { get; } = new List<IConstraint>();
        private bool _isLocked;
        public bool IsLocked
		{
			get => _isLocked;
			set
			{
				_isLocked = value;
				if (!ElementKind.IsPoint())
				{
					foreach (var point in Points)
					{
						point.IsLocked = value;
					}
				}
			} 
		}

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

		public abstract float DistanceToPoint(SKPoint point);

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
		FocalPoint = 0x4,
		BondPoint = 0x8,
        Trait = 0x10,
		Focal = 0x20,
		SingleBond = 0x40,
		DoubleBond = 0x80,
        Entity = 0x100,
		Unit = 0x200,
		Group = 0x400,

		Grid = 0x800,
		SelectionRect = 0x1000,
        Ruler = 0x2000,
        Value = 0x4000,

        Any = 0xFFFF,

        PointKind = RefPoint | Terminal | FocalPoint | BondPoint,
        SegmentKind = Trait | Focal | SingleBond | Unit,
        TraitPart = Trait | Terminal | RefPoint,
        FocalPart = Focal | Unit | FocalPoint,
        BondPart = SingleBond | BondPoint,
        UIPart = Grid | SelectionRect | Ruler | Value,
        TraitPointSource = Terminal | RefPoint | Entity,
        FocalPointSource = FocalPoint | Terminal | RefPoint,
        BondPointSource = Focal | BondPoint | FocalPoint,
    }
	public static class SelectionKindExtensions
	{
		public static bool IsNone(this ElementKind kind) => kind == ElementKind.None;
		public static bool IsPoint(this ElementKind kind) => (int) (kind & ElementKind.PointKind) > 0;// ElementKind.PointKind.HasFlag(kind);
		public static bool IsTerminal(this ElementKind kind) => kind == ElementKind.Terminal;
		public static bool IsCompatible(this ElementKind self, ElementKind other) => (int)(self & other) != 0;

		public static ElementKind AttachableElements(this ElementKind self)
		{
			var result = ElementKind.None;
			switch (self)
			{
				case ElementKind.Terminal:
					result = ElementKind.Any; // or any?
					break;
				case ElementKind.RefPoint:
					result = ElementKind.PointKind;
					break;
				case ElementKind.FocalPoint:
					result = ElementKind.FocalPointSource;
					break;
				case ElementKind.BondPoint:
					result = ElementKind.BondPointSource;
					break;
			}
			return result;
        }

		public static bool CanCreateWith(this ElementKind self, ElementKind other)
		{
			var result = false;
			switch (self)
			{
				case ElementKind.Terminal:
					result = (other == ElementKind.None);
					break;
				case ElementKind.RefPoint:
					result = other.IsCompatible(ElementKind.PointKind); // maybe only terminal and ref points?
					break;
                case ElementKind.Trait:
					result = other.IsCompatible(ElementKind.TraitPointSource);
					break;
				case ElementKind.Focal:
					result = other.IsCompatible(ElementKind.FocalPointSource);
					break;
                case ElementKind.SingleBond:
					result = other.IsCompatible(ElementKind.BondPointSource);
					break;
            }
			return result;
		}
    }
}
