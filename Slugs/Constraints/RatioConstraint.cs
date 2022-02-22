﻿using Slugs.Entities;
using Slugs.Primitives;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // this probably includes directional locking (causation), limits, confidence, complexity etc
    // - lock points to a value (lock zeros on single bonds, focal start points, trait x positions etc)
    // - lock ratios on double bonds
    // - lock unit or focal lengths across traits
    // - lock trait angles (perpendicular, collinear) and display lengths/origins/ends to other traits. (traits are sort of view only, so meaning is less).
    // - perhaps merged points could just be equal constrained? (mechanically, virtually they already are this)
    public enum ConstraintTarget
    {
        None,
        Full,
        X,
        Y,
        T,
        Length,
        StartPoint,
        EndPoint,
        Parallel,
        Perpendicular,
        Tangent,
        Angle,
    }

    public class RatioConstraint : IConstraint
    {
	    public IElement StartElement { get; private set; }
	    public IElement EndElement { get; private set; }
	    public IElement OtherElement(int originalKey) => originalKey == StartElement.Key ? EndElement : StartElement;
	    public bool HasElement(int key) => StartElement.Key == key || EndElement.Key == key;

        public ConstraintTarget ConstraintTarget{get; private set; }

	    private Slug _ratio;
        public Slug Ratio { get => _ratio; set => _ratio = IsRatioLocked ? _ratio : value; }
        public bool IsRatioLocked { get; set; } 

        public RatioConstraint(IElement start, IElement end, ConstraintTarget constraintTarget, Slug ratio)
	    {
		    StartElement = start;
		    EndElement = end;
		    ConstraintTarget = constraintTarget;
            _ratio = ratio;
	    }

        public void OnElementChanged(IElement changedElement)
        {
	        if (changedElement.Key == StartElement.Key)
	        {
		        OnStartChanged();
	        }
            else if (changedElement.Key == EndElement.Key)
	        {
		        OnEndChanged();
	        }
        }
        public void OnStartChanged()
        {
	        if (ConstraintTarget == ConstraintTarget.T && StartElement is ITValue start && EndElement is ITValue end)
	        {
		        end.T = start.T * (float)_ratio.DirectedLength();
	        }
        }
	    public void OnEndChanged()
	    {
		    if (ConstraintTarget == ConstraintTarget.T && StartElement is ITValue start && EndElement is ITValue end)
		    {
			    var dl = (float) _ratio.DirectedLength();
			    start.T = (dl == 0) ? float.MaxValue : end.T / (float)_ratio.DirectedLength();
		    }
        }
    }
}