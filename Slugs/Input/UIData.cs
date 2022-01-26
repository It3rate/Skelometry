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
        public SelectionSet Drag { get; }
        public SelectionSet Current { get; } // Drag, Highlight, Selection, Clipboard
        public SelectionSet Highlight { get; }
        public SelectionSet Selection { get; }
        public SelectionSet Clipboard { get; }

        public bool IsDown { get; private set; }

        //private IElement DragElement => Drag.Selection;
        public bool IsDraggingElement => !Drag.Selection.IsEmpty;
        public bool IsDraggingPoint => !Drag.SnapPoint.IsEmpty;


	    public bool HasHighlightPoint => !Highlight.SnapPoint.IsEmpty;
	    public IPoint HighlightPoint => Highlight.SnapPoint;
	    public bool HasHighlightLine => Highlight.Selection.ElementKind == ElementKind.Trait;
        public Trait HighlightLine => HasHighlightLine ? (Trait)Highlight.Selection : Trait.Empty;

	    //public SKPoint GetHighlightPoint() => Current.SnapPosition; //HighlightPoints.Count > 0 ? HighlightPoints[0].SKPoint : SKPoint.Empty;
	    //public SKSegment GetHighlightLine() => HighlightLine.Segment;

	    private readonly Agent _agent;

        public UIData(Agent agent)
        {
	        _agent = agent;
		    Drag = new SelectionSet(PadKind.Input);
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
		        targetSet.Set(p, (IPoint) result);
	        }
	        else
	        {
		        result = targetSet.Pad.GetSnapTrait(p);
		        if (!result.IsEmpty)
		        {
			        targetSet.Set(p, null, result);
		        }
		        else
		        {
			        targetSet.Set(p, null, null);
                }
            }
	        return result;
        }
        // todo: Creating traits will depend on selected entity etc.
        private Trait CreateTrait(SKPoint p, Pad pad)
        {
	        var entity = pad.CreateEntity();
	        return pad.AddTrait(entity.Key, new SKSegment(p, p), 5);
        }

        public void Start(SKPoint actual)
        {
	        GetHighlight(actual, Drag, null);
	        GetHighlight(actual, Highlight, null);
            if (Drag.Selection.IsEmpty)
            {
	            var trait = CreateTrait(actual, _agent.WorkingPad);
	            Drag.Selection = trait.EndRef;
            }
            Current.Set(actual, Highlight.SnapPoint, Drag.Selection);
            IsDown = true;
        }
	    public void Move(SKPoint actual)
	    {
		    Current.Update(actual);
            GetHighlight(actual, Highlight, Current);
     //       IsDragging = IsDown ? true : false;
		   // if (IsDraggingElement)
		   // {
			  //  var orgPts = Origin.Points;
			  //  var curPts = Current.Points;
			  //  if (orgPts.Length > 0 && orgPts.Length == curPts.Length)
			  //  {
					//var diff = (Current.MousePosition - Origin.MousePosition);
					//for (int i = 0; i < orgPts.Length; i++)
					//{
					//	curPts[i].SKPoint = Origin.MousePosition + diff;
					//}
			  //  }
		   // }
	    }
	    public void End(SKPoint actual)
	    {
		    GetHighlight(actual, Highlight, null);
            Current.Clear();
            Drag.Clear();
            //UpdateHighlight(actual, Current);
            IsDown = false;
	    }

	    public void Reset()
	    {
        }
	    public void CancelDrag(SKPoint actual, IPoint snap, ElementKind kind)
	    {
		    IsDown = false;
	    }
        public void SetHighlight(SKPoint actual, IPoint snap, ElementKind kind)
	    {
	    }

    }

}
