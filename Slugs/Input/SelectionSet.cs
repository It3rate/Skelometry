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
        public SKPoint SnapPosition => Point.IsEmpty ? Position : Point.SKPoint;
        public IPoint Point { get; set; } = TerminalPoint.Empty;
        public IElement Element { get; set; }

        private readonly List<SKPoint> _elementPositions = new List<SKPoint>();

        public SelectionSet(PadKind padKind, SelectionSetKind selectionSetKind)
        {
	        PadKind = padKind;
	        SelectionSetKind = selectionSetKind;
	        Clear();
        }

        public void Set(SKPoint position, IPoint snapPoint = null, IElement selection = null)
        {
            Position = position;
	        //SnapPosition = snapPoint?.SKPoint ?? position;
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
		        pts[i].SKPoint = _elementPositions[i] + dif;
	        }
            Point.SKPoint = Position + dif; // this may or may not be in Points - maybe convert to list and always add it if not empty.
        }

	    public void Clear()
	    {
			Position = SKPoint.Empty;
            //SnapPosition = SKPoint.Empty;
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
