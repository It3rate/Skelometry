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

    public class SelectionSet
    {
	    public PadKind PadKind => Snap.PadKind;
	    public SKPoint ActualPoint { get; private set; }
	    public VPoint Snap { get; private set; }
	    public SelectionKind Kind { get; set; }
	    public SelectionExtent Extent { get; private set; }
	    public SKPoint SnapPoint => HasSnap ? Snap.SKPoint : ActualPoint;
	    public float T { get; set; } = 1;
	    public bool HasSnap => Kind != SelectionKind.None;

        public SelectionSet(PadKind padKind)
	    {
		    Snap = new VPoint(padKind, -1, -1, -1);
		    ActualPoint = new SKPoint(0, 0);
		    Kind = SelectionKind.None;
	    }

	    public void Update(SKPoint position)
	    {
		    ActualPoint = position;
		    Kind = SelectionKind.None;
	    }
	    public void Update(SKPoint position, IPoint snap, SelectionKind kind = SelectionKind.None)
	    {
		    ActualPoint = position;
		    Snap.CopyValuesFrom(snap);
		    T = 1;
		    Kind = (kind == SelectionKind.None) ? Kind : kind;
        }
	    public void Update(SKPoint position, int entityKey, int traitKey, int focalKey, float t, SelectionKind kind = SelectionKind.None)
	    {
		    ActualPoint = position;
		    Snap.Update(PadKind, entityKey, traitKey, focalKey);
		    T = t;
		    Kind = (kind == SelectionKind.None) ? Kind : kind;
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
    public enum SelectionKind
    {
	    None,
	    Point,
	    Trait,
	    Focal,
	    Bond,
        Entity,
	    Unit,
	    Grid,
    }
}
