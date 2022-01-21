using System;
using System.Collections.Generic;
using SkiaSharp;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Primitives;
using IPoint = Slugs.Entities.IPoint;

namespace Slugs.Input
{
	public class UIData
    {
	    public readonly Dictionary<PadKind, Pad> Pads = new Dictionary<PadKind, Pad>();
	    public Pad PadFrom(PadKind key) => Pads[key];

	    public SelectionSet Origin { get; }
	    public SelectionSet Current { get; }
	    public SelectionSet Selection { get; }

        public IElement DragElement { get; set; }
        public bool IsDraggingElement => DragElement != null;
        public bool IsDraggingPoint => Origin.Kind == SelectionKind.Point && IsDraggingElement;

	    public bool IsDown { get; private set; }
	    public bool IsDragging { get; private set; }
	    //public bool IsDraggingPoint => IsDragging && Origin.Extent == SelectionExtent.Point;

        public SKPoint DownPoint => Origin.ActualPoint;
	    public IPoint StartHighlight => Origin.Snap;

	    public SKPoint CurrentPoint => Current.ActualPoint;
	    public SKPoint SnapPoint => Current.SnapPoint;
	    //public DragRef DragRef = new DragRef();

	    public bool HasHighlightPoint => Current.Kind == SelectionKind.Point;
	    public IPoint HighlightPoint => HasHighlightPoint ? (IPoint)Current.Snap : (IPoint)Point.Empty;
	    public bool HasHighlightLine => Current.Kind == SelectionKind.Trait;
        public Trait HighlightLine => HasHighlightLine ? Current.Snap.GetTrait() : Trait.Empty;

	    public SKPoint GetHighlightPoint() => Current.SnapPoint; //HighlightPoints.Count > 0 ? HighlightPoints[0].SKPoint : SKPoint.Empty;
	    public SKSegment GetHighlightLine() => HighlightLine.Segment;

        public UIData()
	    {
		    Origin = new SelectionSet(PadKind.Input);
		    Current = new SelectionSet(PadKind.Input);
		    Selection = new SelectionSet(PadKind.Input);
        }

	    private IElement UpdateHighlight(PadKind padKind, SKPoint p, SelectionSet sel)
	    {
		    IElement result = null;
		    sel.Update(p);
		    var pad = PadFrom(padKind);
		    var snap = pad.GetSnapPoints(p, Origin.Points);
		    sel.Kind = SelectionKind.None;
		    if (snap.Count > 0)
		    {
			    result = snap[0];
			    sel.Update(p, snap[0], SelectionKind.Point);
		    }
		    else
		    {
			    var trait = pad.GetSnapLine(p);
			    if (!trait.IsEmpty)
			    {
				    result = trait;
                    var (t, _) = trait.TFromPoint(p);
				    sel.Update(p, trait.EntityKey, trait.Key, -1, t, SelectionKind.Trait);
			    }
		    }
		    return result;
	    }

        public void Start(SKPoint actual)
        {
	        DragElement = UpdateHighlight(PadKind.Input, actual, Origin);
	        UpdateHighlight(PadKind.Input, actual, Current);
      //      Origin.Update(actual, snap);
		    //Current.Update(actual, snap);
            IsDown = true;
	    }
	    public void Move(SKPoint actual)
	    {
		    UpdateHighlight(PadKind.Input, actual, Current);
            //Current.Update(actual, snap);
		    IsDragging = IsDown ? true : false;
		    if (IsDraggingElement)
		    {
			    if (IsDraggingPoint)
			    {
				    var dif = Current.ActualPoint - Origin.ActualPoint;
                    ((Point)DragElement).SKPoint = Origin.ActualPoint + dif;
			    }
		    }
	    }
	    public void End(SKPoint actual)
	    {
		    UpdateHighlight(PadKind.Input, actual, Current);
            IsDown = false;
		    IsDragging = false;
		    DragElement = null;
	    }

	    public void Reset()
	    {
		    Origin.Kind = SelectionKind.None;
		    Current.Kind = SelectionKind.None;
        }
	    public void CancelDrag(SKPoint actual, IPoint snap, SelectionKind kind)
	    {
		    IsDown = false;
		    IsDragging = false;
	    }
        public void SetHighlight(SKPoint actual, IPoint snap, SelectionKind kind)
	    {
	    }

        public readonly List<SKPoint> DragSegment = new List<SKPoint>();

	    //public bool IsDown => DownPoint != SKPoint.Empty;
	    //public bool IsDragging => !DragRef.IsEmpty;
	    //public bool IsDraggingPoint => !DragRef.IsEmpty && !DragRef.IsLine;

    }

}
