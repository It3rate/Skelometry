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

        private readonly List<IElement> _elements = new List<IElement>();
        private readonly List<SKPoint> _elementPositions = new List<SKPoint>();
        public ElementKind ElementKind => _elements.Count == 0 ? ElementKind.None : _elements.Count > 1 ? ElementKind.Multiple : _elements[0].ElementKind;
        public IElement FirstElement => _elements.Count > 0 ? _elements[0] : TerminalPoint.Empty;
        public void AddElement(IElement element)
        {
	        _elements.Add(element);
	        _elementPositions.AddRange(element.SKPoints);
        }
        public void AddElements(IEnumerable<IElement> elements = null)
        {
	        if (elements != null)
	        {
		        foreach (var element in elements)
		        {
			        _elements.Add(element);
			        _elementPositions.AddRange(element.SKPoints);
		        }
	        }
        }
        public void ClearElements()
        {
	        _elements.Clear();
	        _elementPositions.Clear();
        }
        public bool ContainsElement(IElement element) => _elements.Contains(element);

        public List<IPoint> ElementPoints()
        {
            var pts = new List<IPoint>();
            foreach (var element in _elements)
            {
	            pts.AddRange(element.Points);
            }
            return pts;
        }
        public IEnumerable<IElement> Elements
        {
	        get
	        {
		        foreach (var element in _elements)
		        {
			        yield return element;
		        }
	        }
        }
        public IEnumerable<int> ElementKeys
        {
	        get
	        {
		        foreach (var element in _elements)
		        {
			        yield return element.Key;
		        }
	        }
        }

        public bool HasSelection => !Point.IsEmpty || _elements.Count > 0;
        public bool HasPoint => !Point.IsEmpty;
        public bool HasElement => _elements.Count > 0;


        public SelectionSet(PadKind padKind, SelectionSetKind selectionSetKind)
        {
	        PadKind = padKind;
	        SelectionSetKind = selectionSetKind;
	        Clear();
        }

        public void SetPoint(SKPoint position, IPoint snapPoint = null)
        {
	        Position = position;
	        Point = snapPoint ?? TerminalPoint.Empty;
        }
        public void SetElements(params IElement[] selections)
        {
	        ClearElements();
	        foreach (var selection in selections)
	        {
				AddElement(selection);
	        }
        }
        public void SetElements(IEnumerable<IElement> selection = null)
        {
	        ClearElements();
	        AddElements(selection);
        }
        public void UpdatePositions(SKPoint newPosition)
        {
	        var dif = newPosition - Position;
	        foreach (var element in Elements)
	        {
		        var pts = element.Points;
		        for (int i = 0; i < pts.Count; i++)
		        {
			        pts[i].Position = _elementPositions[i] + dif;
		        }
	        }
            Point.Position = Position + dif; // this may or may not be in Points - maybe convert to list and always add it if not empty.
        }

	    public void Clear()
	    {
			Position = SKPoint.Empty;
            //SnapPosition = Position.Empty;
            Point = TerminalPoint.Empty;
            ClearElements();
        }

	    public List<IPoint> AllPoints()
	    {
		    var result = ElementPoints();
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
