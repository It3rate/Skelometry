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
	    public Pad Pad => Agent.Current.PadAt(PadKind);
        public SelectionSetKind SelectionSetKind { get; }

	    public SKPoint Position { get; set; }
        public SKPoint SnapPosition => Point.IsEmpty ? Position : Point.Position;
        private IPoint _point = TerminalPoint.Empty;
        public IPoint Point
        {
	        get => _point;
	        set { _point = value; Position = _point.Position; }
        }
        private readonly List<SKPoint> _elementPositions = new List<SKPoint>();
        private IElement _element;
        public IElement Element
        {
	        get => _element;
	        set
	        {
		        _element = value;
		        _elementPositions.Clear();
		        _elementPositions.AddRange(Element.SKPoints);
	        }
        }

        public bool HasSelection => !Point.IsEmpty || !Element.IsEmpty;
        public bool HasPoint => !Point.IsEmpty;
        public bool HasElement => !Element.IsEmpty;


        public SelectionSet(PadKind padKind, SelectionSetKind selectionSetKind)
        {
	        PadKind = padKind;
	        SelectionSetKind = selectionSetKind;
	        Clear();
        }

        public void Set(SKPoint position, IPoint snapPoint = null, IElement selection = null)
        {
            Position = position;
	        //SnapPosition = snapPoint?.Position ?? position;
	        Point = snapPoint ?? TerminalPoint.Empty;
	        Element = selection ?? TerminalPoint.Empty;
            _elementPositions.Clear();
	        _elementPositions.AddRange(Element.SKPoints);
        }
        public void UpdatePositions(SKPoint newPosition)
        {
	        var dif = newPosition - Position;
	        var pts = Element.Points;
	        for (int i = 0; i < pts.Count; i++)
	        {
		        pts[i].Position = _elementPositions[i] + dif;
	        }
            Point.Position = Position + dif; // this may or may not be in Points - maybe convert to list and always add it if not empty.
        }

	    public void Clear()
	    {
			Position = SKPoint.Empty;
            //SnapPosition = Position.Empty;
            Point = TerminalPoint.Empty;
            Element = TerminalPoint.Empty;
		    _elementPositions.Clear();
        }

	    public List<IPoint> AllPoints()
	    {
		    var result = Element.Points;
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
