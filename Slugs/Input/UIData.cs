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
	    public SelectionSet Highlight { get; }
	    public SelectionSet Selection { get; }

        public IElement DragElement { get; set; }
        public bool IsDraggingElement => DragElement != null;
        public bool IsDraggingPoint => Origin.Kind == ElementKind.Point && IsDraggingElement;
        public readonly List<SKPoint> DragSegment = new List<SKPoint>();


        public bool IsDown { get; private set; }
	    public bool IsDragging { get; private set; }
	    //public bool IsDraggingPoint => IsDragging && Origin.Extent == SelectionExtent.Point;

        public SKPoint DownPoint => Origin.OriginPoint;
	    public IPoint StartHighlight => Origin.Snap;

	    public SKPoint CurrentPoint => Current.OriginPoint;
	    public SKPoint SnapPoint => Current.OriginSnap;
	    //public DragRef DragRef = new DragRef();

	    public bool HasHighlightPoint => Current.Kind == ElementKind.Point;
	    public IPoint HighlightPoint => HasHighlightPoint ? (IPoint)Current.Snap : (IPoint)Point.Empty;
	    public bool HasHighlightLine => Current.Kind == ElementKind.Trait;
        public Trait HighlightLine => HasHighlightLine ? Current.Snap.GetTrait() : Trait.Empty;

	    public SKPoint GetHighlightPoint() => Current.OriginSnap; //HighlightPoints.Count > 0 ? HighlightPoints[0].SKPoint : SKPoint.Empty;
	    public SKSegment GetHighlightLine() => HighlightLine.Segment;

        public UIData()
	    {
		    Origin = new SelectionSet(PadKind.Input);
		    Current = new SelectionSet(PadKind.Input);
		    Selection = new SelectionSet(PadKind.Input);
            Highlight = new SelectionSet(PadKind.Input);
        }

	    private IElement UpdateHighlight(PadKind padKind, SKPoint p, params SelectionSet[] sels)
	    {
		    IElement result = null;
		    var pad = PadFrom(padKind);
		    var snap = pad.GetSnapPoints(p, Origin.Points);
		    if (snap.Count > 0)
		    {
			    result = snap[0];
			    foreach (var sel in sels)
			    {
					sel.Update(p, snap[0], ElementKind.Point);
			    }
		    }
		    else
		    {
			    var trait = pad.GetSnapLine(p);
			    if (!trait.IsEmpty)
			    {
				    result = trait;
                    var (t, _) = trait.TFromPoint(p);
                    foreach (var sel in sels)
                    {
	                    sel.Update(p, trait.EntityKey, trait.Key, -1, t, ElementKind.Trait);
                    }
			    }
		    }
		    return result;
	    }

        public void Start(SKPoint actual)
        {
	        DragElement = UpdateHighlight(PadKind.Input, actual, Origin);
            IsDown = true;
	    }
	    public void Move(SKPoint actual)
	    {
		    Current.Update(actual);
		    UpdateHighlight(PadKind.Input, actual, Current);
		    UpdateHighlight(PadKind.Input, SKPoint.Empty, Highlight);
            IsDragging = IsDown ? true : false;
		    if (IsDraggingElement)
		    {
			    var orgPts = Origin.Points;
			    var curPts = Current.Points;
			    if (orgPts.Length > 0 && orgPts.Length == curPts.Length)
			    {
					var diff = (Current.OriginPoint - Origin.OriginPoint);
					for (int i = 0; i < orgPts.Length; i++)
					{
						curPts[i].SKPoint = Origin.OriginPoint + diff;
					}
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
		    Origin.Kind = ElementKind.None;
		    Current.Kind = ElementKind.None;
        }
	    public void CancelDrag(SKPoint actual, IPoint snap, ElementKind kind)
	    {
		    IsDown = false;
		    IsDragging = false;
	    }
        public void SetHighlight(SKPoint actual, IPoint snap, ElementKind kind)
	    {
	    }

    }

}
