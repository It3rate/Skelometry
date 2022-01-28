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

        public float T { get; set; } = 1;
	    public SKPoint MousePosition { get; private set; }
        public SKPoint SnapPosition { get; private set; }
        private readonly List<SKPoint> _selectionPositions = new List<SKPoint>();
        public IPoint SnapPoint { get; private set; }
        public IElement Selection { get; set; }

        public SelectionSet(PadKind padKind, SelectionSetKind selectionSetKind)
        {
	        PadKind = padKind;
	        SelectionSetKind = selectionSetKind;
	        Clear();
        }

        public void Set(SKPoint position, IPoint snapPoint = null, IElement selection = null)
        {
            MousePosition = position;
	        SnapPosition = snapPoint?.SKPoint ?? position;
	        SnapPoint = snapPoint ?? TerminalPoint.Empty;
	        Selection = selection ?? TerminalPoint.Empty;
            _selectionPositions.Clear();
	        _selectionPositions.AddRange(Selection.SKPoints);
        }
        public void Update(SKPoint newPosition)
        {
	        var dif = newPosition - MousePosition;
	        var pts = Selection.Points;
	        for (int i = 0; i < pts.Length; i++)
	        {
		        pts[i].SKPoint = _selectionPositions[i] + dif;
	        }
            SnapPoint.SKPoint = SnapPosition + dif; // this may or may not be in Points - maybe convert to list and always add it if not empty.
        }

	    public void Clear()
	    {
		    T = 1;
			MousePosition = SKPoint.Empty;
            SnapPosition = SKPoint.Empty;
            SnapPoint = TerminalPoint.Empty;
            Selection = TerminalPoint.Empty;
		    _selectionPositions.Clear();
        }

	    public List<IPoint> AllPoints()
	    {
		    var result = new List<IPoint>();
		    if (!SnapPoint.IsEmpty)
		    {
			    result.Add(SnapPoint);
		    }
		    if (!Selection.IsEmpty)
		    {
			    result.AddRange(Selection.Points);
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
