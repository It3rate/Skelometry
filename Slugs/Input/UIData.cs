using System;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Primitives;
using IPoint = Slugs.Entities.IPoint;

namespace Slugs.Input
{
	public class UIData
	{
		public IEnumerable<Pad> Pads => _agent.Pads.Values;

        //public SelectionSet Origin { get; }
        public SelectionSet DownSet { get; }
        public SelectionSet Current { get; } // Drag, Highlight, Selection, Clipboard
        public SelectionSet Highlight { get; }
	    public SelectionSet Selection { get; }

        private IElement DragElement { get; set; } =  TerminalPoint.Empty;
        public bool IsDraggingElement => !DragElement.IsEmpty;
        public bool IsDraggingPoint => Current.SelectionKind == ElementKind.RefPoint && IsDraggingElement;
        //public readonly List<SKPoint> DragSegment = new List<SKPoint>();


        public bool IsDown { get; private set; }
	    public bool IsDragging { get; private set; }
	    //public bool IsDraggingPoint => IsDragging && Origin.Extent == SelectionExtent.RefPoint;

        public SKPoint DownPoint => Current.OriginPosition;
	    public IPoint DownHighlight => DownSet.OriginIPoint;
	    public SKPoint SnapPoint => Current.SnapOriginPosition;

	    public bool HasHighlightPoint => !Highlight.OriginIPoint.IsEmpty;
	    public IPoint HighlightPoint => Highlight.OriginIPoint;
	    public bool HasHighlightLine => Highlight.SelectionKind == ElementKind.Trait;
        public Trait HighlightLine => HasHighlightLine ? (Trait)Highlight.Selection : Trait.Empty;

	    //public SKPoint GetHighlightPoint() => Current.SnapOriginPosition; //HighlightPoints.Count > 0 ? HighlightPoints[0].SKPoint : SKPoint.Empty;
	    //public SKSegment GetHighlightLine() => HighlightLine.Segment;

	    private readonly Agent _agent;

        public UIData(Agent agent)
        {
	        _agent = agent;
		    DownSet = new SelectionSet(PadKind.Input);
		    Current = new SelectionSet(PadKind.Input);
		    Selection = new SelectionSet(PadKind.Input);
            Highlight = new SelectionSet(PadKind.Input);
        }

        private IElement GetHighlight(SKPoint p, SelectionSet targetSet, SelectionSet ignoreSet)
        {
	        var points = ignoreSet?.Selection.Points;
            IElement result = targetSet.Pad.GetSnapPoint(p, points);
	        if (!result.IsEmpty)
	        {
				Highlight.Set(p, (IPoint) result);
	        }
	        else
	        {
		        result = targetSet.Pad.GetSnapTrait(p);
		        if (!result.IsEmpty)
		        {
			        Highlight.Set(p, null, result);
		        }
		        else
		        {
			        Highlight.Set(p, null, null);
                }
            }
	        return result;
        }
        private void UpdateDragElement(SKPoint p, SelectionSet sel)
	    {
		    if (!DragElement.IsEmpty)
		    {
			    switch (DragElement.ElementKind)
			    {
				    case ElementKind.Terminal:
				    case ElementKind.RefPoint:
				    case ElementKind.VPoint:
                        sel.Update(p);//, (IPoint)DragElement, ElementKind.RefPoint);
                        break;
                    case ElementKind.Trait:
	                    var trait = (Trait)DragElement;
	                    var (t, _) = trait.TFromPoint(p);
	                    sel.Update(p);//, trait.EntityKey, trait.Key, -1, t, ElementKind.Trait);
                        break;
			    }
		    }
	    }
        // todo: Creating traits will depend on selected entity etc.
        private Trait CreateTrait(SKPoint p, Pad pad)
        {
	        var entity = pad.CreateEntity();
	        return pad.AddTrait(entity.Key, new SKSegment(p, p), 5);
        }

        public void Start(SKPoint actual)
        {
	        DragElement = GetHighlight(actual, DownSet, null);
	        GetHighlight(actual, Highlight, null);
            if (DragElement.IsEmpty)
            {
	            var trait = CreateTrait(actual, Current.Pad);
	            DragElement = trait.EndRef;
            }
            Current.Set(actual, Highlight.OriginIPoint, DragElement);
            //UpdateDragElement(actual, Current);
            IsDown = true;
        }
	    public void Move(SKPoint actual)
	    {
		    Current.Update(actual);
            GetHighlight(actual, Highlight, Current);
		    UpdateDragElement(actual, Current);

     //       IsDragging = IsDown ? true : false;
		   // if (IsDraggingElement)
		   // {
			  //  var orgPts = Origin.Points;
			  //  var curPts = Current.Points;
			  //  if (orgPts.Length > 0 && orgPts.Length == curPts.Length)
			  //  {
					//var diff = (Current.OriginPosition - Origin.OriginPosition);
					//for (int i = 0; i < orgPts.Length; i++)
					//{
					//	curPts[i].SKPoint = Origin.OriginPosition + diff;
					//}
			  //  }
		   // }
	    }
	    public void End(SKPoint actual)
	    {
		    GetHighlight(actual, Highlight, null);
            UpdateDragElement(actual, Current);
            Current.Clear();
            DownSet.Clear();
            //UpdateHighlight(actual, Current);
            IsDown = false;
		    IsDragging = false;
		    DragElement = TerminalPoint.Empty;
	    }

	    public void Reset()
	    {
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
