﻿using System;
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
        List<IPoint> Points { get; }
        List<SKPoint> SKPoints{ get; }

        void MoveTo(SKPoint position);
    }

    public abstract class ElementBase : IElement
    {
	    protected static int KeyCounter = 1;
	    public const int EmptyKeyValue = -99;
	    public virtual bool IsEmpty => Key == EmptyKeyValue;
	    public static bool IsEmptyKey(int key) => key == EmptyKeyValue;

        public PadKind PadKind { get; set; }
        public Pad Pad => Agent.Current.PadAt(PadKind);
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

        protected ElementBase(bool isEmpty) // used to privately create an empty element
        {
	        PadKind = PadKind.None;
            Key = EmptyKeyValue;
		}
		protected ElementBase(PadKind padKind)
		{
			Key = KeyCounter++;
			PadKind = padKind;
			Pad.AddElement(this);
        }
	    public virtual void MoveTo(SKPoint position)
	    {
		    foreach (var point in Points)
		    {
			    point.Position = position;
		    }
	    }
    }

    [Flags]
	public enum ElementKind
	{
		None,
		Terminal,
		RefPoint,
		PointOnTrait,
        Trait,
		Focal,
		Bond,
		Entity,
		Unit,
		Group,
		SelectionGroup,
        PadProjection,
        Grid,

        PointKind = RefPoint | Terminal | PointOnTrait,
        SegmentKind = Trait | Focal | Bond,
    }
	public static class SelectionKindExtensions
	{
		public static bool IsNone(this ElementKind kind) => kind == ElementKind.None;
		public static bool IsPoint(this ElementKind kind) => ElementKind.PointKind.HasFlag(kind);
        public static bool IsTerminal(this ElementKind kind) => kind == ElementKind.Terminal;
    }
}
