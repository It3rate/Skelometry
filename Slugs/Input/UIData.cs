using System;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Primitives;
using Slugs.UI;
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

	    public bool HasHighlightPoint => !Highlight.Point.IsEmpty;
	    public IPoint HighlightPoint => Highlight.Point;
	    public bool HasHighlightLine => Highlight.ElementKind.IsCompatible(ElementKind.SegmentKind);
        public SegmentBase HighlightLine => HasHighlightLine ? (SegmentBase)Highlight.FirstElement : Trait.Empty;

        public DisplayMode DisplayMode { get; set; }

        private readonly Agent _agent;

        public UIData(Agent agent)
        {
	        _agent = agent;
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Begin);
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Current);
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Selection);
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Highlight);
	        AddSelectionSet(PadKind.Input, SelectionSetKind.Clipboard);

	        DisplayMode |= DisplayMode.ShowAllValues;
        }

        public List<SKPoint> WorkingPoints = new List<SKPoint>();
        public void SetWorkingPoints(params SKPoint[] points)
        {
            WorkingPoints.Clear();
            WorkingPoints.AddRange(points);
        }

        public IElement GetHighlight(SKPoint p, SelectionSet targetSet, List<int> ignoreKeys, ElementKind kind)
        {
	        targetSet.Clear();
            IElement result = targetSet.Pad.GetSnapPoint(p, ignoreKeys, kind);
	        if (!result.IsEmpty)
	        {
		        targetSet.SetPoint(p, (IPoint) result);
	        }
	        else
	        {
		        result = targetSet.Pad.GetSnapElement(p, ignoreKeys, kind);
		        if (!result.IsEmpty)
		        {
			        targetSet.SetElements(result);
		        }
            }
	        return result;
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
