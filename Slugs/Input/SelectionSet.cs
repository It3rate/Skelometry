﻿using SkiaSharp;
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

	// Eventaully selection sets are the basis of commands
    public class SelectionSet
    {
	    public PadKind PadKind { get; private set; }
	    public Pad Pad => Agent.Current.PadAt(PadKind);
        // todo: all origin/snaps are lists, used for undo. Any multiple VPoint selections would probably be temp groups, but maybe collections too.
        // will still need the idea of a focused point, eg snapping a group of elements to a point based on a single point in them
        public SKPoint OriginPoint { get; private set; }
        private SKPoint _originSnap;
        public SKPoint OriginSnap
        {
	        get => Kind.HasSnap() ? _originSnap : OriginPoint;
	        private set => _originSnap = value;
        }
	    public VPoint Snap { get; private set; } // todo: this is an element key. If multi-select, it is a key to a temp group.
	    public ElementKind Kind { get; set; }
	    //public SelectionExtent Extent { get; private set; }
	    public float T { get; set; } = 1;

        public IPoint[] Points
        {
	        get
	        {
		        var result = new List<IPoint>();
		        switch (Kind)
		        {
			        case ElementKind.Terminal:
                    case ElementKind.RefPoint:
				        if (Snap.ElementKind == ElementKind.VPoint)
				        {
					        result.Add(T > 0.5f ? Snap.GetEndPoint() : Snap.GetStartPoint());
				        }
				        else
				        {
                            result.Add(Pad.PointAt(Snap.Key));
				        }
				        break;
			        case ElementKind.Trait:
				        var trait = Snap.GetTrait();
				        result.Add(trait.StartRef);
				        result.Add(trait.EndRef);
                        break;
                }
		        return result.ToArray();
	        }
        }

        public SelectionSet(PadKind padKind)
        {
	        PadKind = padKind;
		    Snap = new VPoint(padKind, -1, -1, -1);
		    OriginPoint = new SKPoint(0, 0);
		    Kind = ElementKind.None;
	    }

        public void Set(SKPoint position)
        {
	        OriginPoint = position;
	        OriginSnap = position;
	        Kind = ElementKind.None;
        }

        public void Update(SKPoint position)
	    {
		    OriginPoint = position;
		    OriginSnap = position;
		    Kind = ElementKind.Terminal;
	    }
	    public void Update(SKPoint position, IPoint snap, ElementKind kind = ElementKind.None)
	    {
		    OriginPoint = position;
		    Kind = (kind == ElementKind.None) ? Kind : kind;
		    OriginSnap = snap.SKPoint;
		    Snap.CopyValuesFrom(snap);
		    T = 1;
        }
	    public void Update(SKPoint position, int entityKey, int traitKey, int focalKey, float t, ElementKind kind = ElementKind.None)
	    {
		    OriginPoint = position;
		    Snap.Update(PadKind, entityKey, traitKey, focalKey);
		    OriginSnap = Snap.SKPoint;
            T = t;
		    Kind = (kind == ElementKind.None) ? Kind : kind;
	    }

	    public void Clear()
	    {
		    OriginPoint = SKPoint.Empty;
		    OriginSnap = SKPoint.Empty;
		    Kind = ElementKind.None;
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
