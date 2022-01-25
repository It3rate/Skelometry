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
	    public static bool IsEmptyKey(int key) => ElementBase.IsEmptyKey(key);

	    public float T { get; set; } = 1;
	    public SKPoint OriginPosition { get; private set; }
        public SKPoint SnapPosition
        {
	        get => SelectionKind.HasSnap() ? _snapPosition : OriginPosition;
	        private set => _snapPosition = value;
        }
        public IPoint OriginIPoint
	    {
		    get
		    {
			    IPoint result;
			    if (!IsEmptyKey(_originIPointKey))
			    {
				    result = Pad.PointAt(_originIPointKey);
			    }
			    else
			    {
				    var pt = Pad.ElementAt(_selectionKey);
				    result = pt.ElementKind.IsPoint() ? (IPoint) pt : TerminalPoint.Empty;
			    }
			    return result;
		    }
		    set => _originIPointKey = value.Key;
        }
        public IElement Selection
	    {
		    get => IsEmptyKey(_selectionKey) ? TerminalPoint.Empty : Pad.ElementAt(_selectionKey);
		    set => _selectionKey = value.Key;
	    }
	    public ElementKind SelectionKind => Selection.ElementKind;

        private SKPoint _snapPosition;
        private int _originIPointKey = TerminalPoint.Empty.Key;
        private int _selectionKey { get; set; } // todo: this is an element key. If multi-select, it is a key to a temp group, not a list?
        private List<SKPoint> _startPositions = new List<SKPoint>();


        public SelectionSet(PadKind padKind)
        {
	        PadKind = padKind;
	        Clear();
        }

        private SKPoint _currentPosition;
        public void Set(SKPoint position, IPoint snapPoint = null, IElement selection = null)
        {
            _currentPosition = position;
            OriginPosition = position;
	        SnapPosition = snapPoint?.SKPoint ?? position;
	        OriginIPoint = snapPoint ?? TerminalPoint.Empty;
	        Selection = selection ?? TerminalPoint.Empty;
            _startPositions.Clear();
	        _startPositions.AddRange(Selection.SKPoints);
        }
        public void Update(SKPoint newPosition)
        {
	        _currentPosition = newPosition;
	        var dif = newPosition - OriginPosition;
	        var pts = Selection.Points;
	        for (int i = 0; i < pts.Length; i++)
	        {
		        pts[i].SKPoint = _startPositions[i] + dif;
	        }
            OriginIPoint.SKPoint = SnapPosition + dif; // this may or may not be in Points - maybe convert to list and always add it if not empty.
		    T = 1;
        }
	    //public void Update(SKPoint newPosition, int entityKey, int traitKey, int focalKey, float t, ElementKind kind = ElementKind.None)
	    //{
		   // _currentPosition = newPosition;
     //       OriginPosition = newPosition;
		   // Selection.Update(PadKind, entityKey, traitKey, focalKey);
		   // SnapPosition = Selection.SKPoint;
     //       T = t;
		   // SelectionKind = (kind == ElementKind.None) ? SelectionKind : kind;
	    //}

	    public void Clear()
	    {
		    _currentPosition = SKPoint.Empty;
			OriginPosition = SKPoint.Empty; 
            _snapPosition = SKPoint.Empty;
		    _startPositions.Clear();

		    OriginIPoint = TerminalPoint.Empty;
		    Selection = TerminalPoint.Empty;
        }
        // All selectable elements can be represented by their points? Need for offset, highlighting, 
        public IPoint[] GetElementPoints()
	    {
		    throw new NotImplementedException();
	    }
    }

    public enum SelectionExtent
    {
	    None,
	    Point,
	    Line,
	    Linkages,
    }
}
