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
        public Dictionary<SelectionSetKind, SelectionSet> SelectionSets = new Dictionary<SelectionSetKind, SelectionSet>();
        private void AddSelectionSet(PadKind padKind, SelectionSetKind kind) => SelectionSets.Add(kind, new SelectionSet(padKind, kind));
        public SelectionSet SelectionSetFor(SelectionSetKind kind) => SelectionSets[kind];
        public SelectionSet Begin => SelectionSets[SelectionSetKind.Begin];
        public SelectionSet Current => SelectionSets[SelectionSetKind.Current];
        public SelectionSet Highlight => SelectionSets[SelectionSetKind.Highlight];
        public SelectionSet Selected => SelectionSets[SelectionSetKind.Selection];
        public SelectionSet Clipboard => SelectionSets[SelectionSetKind.Clipboard];

        //private IElement DragElement => Begin.Selected;
        public bool IsDraggingElement => !Begin.Selection.IsEmpty;
        public bool IsDraggingPoint => !Begin.SnapPoint.IsEmpty;


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
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Begin);
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Current);
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Selection);
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Highlight);
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Clipboard);
        }

        private IElement GetHighlight(SKPoint p, SelectionSet targetSet, SelectionSet ignoreSet)
        {
	        var points = ignoreSet?.AllPoints().ToArray() ?? new IPoint[0];

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

        public void Start(SKPoint actual)
        {
	        GetHighlight(actual, Begin, null);
	        GetHighlight(actual, Highlight, null);
            Current.Set(actual, Highlight.SnapPoint, Begin.Selection);
        }
	    public void Move(SKPoint actual)
	    {
		    Current.Update(actual);
            Selected.Update(actual);
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
		    Current.Update(actual);
            GetHighlight(actual, Highlight, Current);
            Current.Clear();
            Begin.Clear();
	    }

	    public void Reset()
	    {
        }
	    public void CancelDrag(SKPoint actual, IPoint snap, ElementKind kind)
	    {
	    }
        public void SetHighlight(SKPoint actual, IPoint snap, ElementKind kind)
	    {
	    }

    }

}
