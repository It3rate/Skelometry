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
        private readonly Group _elements;
        public IEnumerable<IElement> Elements => _elements.Elements;
        public IEnumerable<int> ElementKeys => _elements.ElementKeys;
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

        public void SetPoint(SKPoint position, IPoint snapPoint = null)
        {
	        Position = position;
	        Point = snapPoint ?? TerminalPoint.Empty;
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
        public void UpdatePositions(SKPoint newPosition)
        {
	        var dif = newPosition - Position;
	        var points = _elements.Points;

            for (int i = 0; i < points.Count; i++)
	        {
		        points[i].Position = _elements.SetPositions[i] + dif;
	        }
            Point.Position = Position + dif; // this may or may not be in Points - maybe convert to list and always add it if not empty.
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
	    Clipboard,
    }
}
