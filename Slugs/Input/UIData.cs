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
        public bool IsDraggingElement => Begin.HasElement;
        public bool IsDraggingPoint => Begin.HasPoint;


	    public bool HasHighlightPoint => !Highlight.Point.IsEmpty;
	    public IPoint HighlightPoint => Highlight.Point;
	    public bool HasHighlightLine => Highlight.ElementKind == ElementKind.Trait;
        public Trait HighlightLine => HasHighlightLine ? (Trait)Highlight.FirstElement : Trait.Empty;

	    //public Position GetHighlightPoint() => Current.SnapPosition; //HighlightPoints.Count > 0 ? HighlightPoints[0].Position : Position.Empty;
	    //public SKSegment GetHighlightLine() => HighlightLine.Trait;

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

        public IElement GetHighlight(SKPoint p, SelectionSet targetSet, List<int> ignoreKeys)
        {
	        IElement result = targetSet.Pad.GetSnapPoint(p, ignoreKeys);
	        if (!result.IsEmpty)
	        {
		        targetSet.SetPoint(p, (IPoint) result);
	        }
	        else
	        {
		        result = targetSet.Pad.GetSnapTrait(p, ignoreKeys);
		        if (!result.IsEmpty)
		        {
			        targetSet.SetElements(result);
		        }
		        else
		        {
			        //targetSet.SetPoint(p);
                    targetSet.Clear();
                }
            }
	        return result;
        }
        // todo: Creating traits will depend on selected entity etc.

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
