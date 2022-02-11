using SkiaSharp;
using Slugs.Agents;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

	// Eventually selection sets are the basis of commands
    public class SelectionSet
    {
	    public PadKind PadKind { get; private set; }
	    public Pad Pad => Agent.Current.PadFor(PadKind);
        public SelectionSetKind SelectionSetKind { get; }

	    public SKPoint Position { get; set; }
        private IPoint _point = TerminalPoint.Empty;
        public IPoint Point
        {
	        get => _point;
	        set { _point = value; Position = _point.Position; }
        }
        public int PointKey => _point.Key;

        private readonly Group _elements; // maybe just make this public
        public IEnumerable<IElement> Elements => _elements.Elements;
        public IEnumerable<int> ElementKeys => _elements.ElementKeys;
        public int[] ElementKeysCopy => _elements.ElementKeysCopy;
        public IElement FirstElement => _elements.FirstElement;
        public IElement LastElement => _elements.LastElement;
        public ElementKind ElementKind => _elements.Kind;
        public void ClearElements() => _elements.Clear();

        public bool HasSelection => !Point.IsEmpty || _elements.Count > 0;
        public bool HasPoint => !Point.IsEmpty;
        public bool HasElement => _elements.Count > 0;


        public SelectionSet(PadKind padKind, SelectionSetKind selectionSetKind)
        {
	        PadKind = padKind;
	        SelectionSetKind = selectionSetKind;
            _elements = new Group(padKind);
	        Clear();
        }

        public void SetWith(SelectionSet selSet)
        {
	        SetPoint(selSet.Position, selSet.Point);
            SetElements(selSet.ElementKeys);
        }
        public void SetPoint(SKPoint position, IPoint point = null)
        {
	        Position = position;
	        Point = point ?? TerminalPoint.Empty;
        }
        public void SetPoint(IPoint point)
        {
	        Point = point;
	        Position = Point.Position;
        }
        public void SetPoint(int pointKey)
        {
	        Point = Pad.PointAt(pointKey);
	        Position = Point.Position;
        }
        public void SetElements(params IElement[] selections)
        {
	        _elements.Clear();
            _elements.AddRange(selections);
        }
        public void SetElements(IEnumerable<IElement> selection)
        {
	        _elements.Clear();
	        _elements.AddRange(selection);
        }
        public void SetElements(IEnumerable<int> selectionKeys)
        {
	        _elements.Clear();
	        _elements.AddRange(selectionKeys);
        }
        public void UpdatePositions(SKPoint newPosition)
        {
	        var dif = newPosition - Position;
	        if (!Point.IsLocked)
	        {
		        Point.Position = Position + dif;
	        }

	        foreach (var element in Elements)
	        {
		        if (!element.IsLocked)
		        {
			        var points = element.Points;
                    var initialPoints = _elements.InitialPositionFor(element.Key);
                    for (int i = 0; i < points.Count; i++)
                    {
	                    points[i].Position = initialPoints[i] + dif;
                    }
                }
	        }
        }

	    public void Clear()
	    {
			Position = SKPoint.Empty;
            Point = TerminalPoint.Empty;
            _elements.Clear();
        }

	    public List<IPoint> AllPoints()
	    {
		    var result = _elements.Points;
		    if (!Point.IsEmpty)
		    {
			    result.Add(Point);
		    }
		    return result;
	    }
    }

    public enum SelectionSetKind
    {
	    Begin,
	    Current,
	    Highlight,
	    Selection,
	    Working,
	    Clipboard,
    }
}
