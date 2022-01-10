using System.Collections;
using System.Dynamic;
using SkiaSharp;
using Slugs.Agent;
using Slugs.Pads;
using Slugs.Slugs;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct T // maybe t is just a slug
    {
	    public double Tn { get; }
	    public double Ti { get; }
    }
    public struct Bond
    {
	    public int SegIndex0 { get; }
	    public int SegIndex1 { get; }
	    public Slug Slug0 { get; }
	    public Slug Slug1 { get; }
    }
    public class Motor : IEnumerable<SKPoint>
    {
        public static readonly Motor Empty = new Motor();
	    public bool IsEmpty = false;

        public int PadIndex;
	    public int MotorIndex;

        // ISample - pointRef, Motor/t - returns an xy point when sampled with t (can also sample a t between any two segrefs)
        // Ordered SegRefs - each side is an ISample. Eventually can have min/max, behaviour at limits and past, accuracy, precision type etc.
        // Bonds (2 per segref pair) - these are t based floats, not points
        // BondDirections - can be one way, both ways, cacheable'

        // refs do not need to point to the internal point store, but will normally start this way for user input.
        //private readonly List<SKPoint> _inputPoints = new List<SKPoint>(); 
	    //private readonly List<IPointRef> _inputRefs = new List<IPointRef>();
	    public int Count => _segments.Count;

	    private readonly List<SegmentRef> _segments = new List<SegmentRef>();
	    private readonly List<Slug> _bonds = new List<Slug>();

        private Motor()
	    {
		    IsEmpty = true;
		    // SKPoint.Empty, SkPointExtension.MinPoint, SkPointExtension.MaxPoint
        }

	    private Motor(SlugPad pad, params SKPoint[] points)
	    {
		    pad?.Add(this);
		    foreach (var point in points)
		    {
			    this.Add(point);
		    }
	    }
	    private Motor(SlugPad pad, params IPointRef[] pointRefs)
        {
	        pad.Add(this);
            foreach (var pointRef in pointRefs)
		    {
			    this.Add(pointRef);
		    }
	    }

	    public static Motor CreateIn(SlugPad pad, params SKPoint[] points)
	    {
		    return new Motor(pad, points);
	    }
	    public static Motor CreateIn(SlugPad pad, List<SKPoint> points)
	    {
		    return new Motor(pad, points.ToArray());
	    }
        public static Motor CreateIn(SlugPad pad, params IPointRef[] pointRefs)
	    {
		    return new Motor(pad, pointRefs);
	    }

        public bool HasSlug(SlugRef slugRef) =>
	        slugRef.PadIndex == PadIndex && slugRef.MotorIndex == MotorIndex && HasSlugAtIndex(slugRef.SlugIndex);
        public bool HasSlugAtIndex(int index) => index >= 0 && index < _bonds.Count;
        public int SlugCount => _bonds.Count;
        public Slug SlugAt(int index) => (index >= 0 && index < _bonds.Count) ? _bonds[index] : Slug.Empty; // default to unit slug instead of empty?
        public SlugRef AddSlug(Slug item)
        {
	        _bonds.Add(item);
	        return new SlugRef(PadIndex, MotorIndex, _bonds.Count - 1);
        }
        public bool RemoveSlugAt(int index)
        {
	        bool result = false;
	        if (HasSlugAtIndex(index))
	        {
		        result = true;
		        _bonds.RemoveAt(index);
	        }
	        return result;
        }
        public bool RemoveSlug(SlugRef slugRef)
        {
	        bool result = false;
	        if (HasSlug(slugRef))
	        {
		        result = true;
                _bonds.RemoveAt(slugRef.SlugIndex);
	        }
	        return result;
        }

        private bool IsOwn(IPointRef pointRef) => pointRef.PadIndex == PadIndex && pointRef.MotorIndex == MotorIndex;
        private SlugPad PadAt(int index) => SlugAgent.Pads[index];

        public SKPoint GetPointAt(double t)
        {

        }
        //public SKPoint this[IPointRef pointRef]
        //{
	       // get => IsOwn(pointRef) ? _inputPoints[pointRef.TIndex] : SlugAgent.ActiveAgent.GetPointAt(pointRef);
	       // set
	       // {
		      //  if (IsOwn(pointRef))
		      //  {
			     //   _inputPoints[pointRef.TIndex] = value;
		      //  }
		      //  else
		      //  {
			     //   SlugAgent.ActiveAgent.SetPointAt(pointRef, value);
        //        }
	       // }
        //}
        public IPointRef this[int index]
        {
	        get => _inputRefs[index];
	        set => _inputRefs[index] = value;
        }
        public IPointRef FirstRef => _inputRefs.Count > 0 ? _inputRefs[0] : PointRef.Empty;
        public IPointRef LastRef => _inputRefs.Count > 0 ? _inputRefs[_inputRefs.Count - 1] : PointRef.Empty;
        public SKPoint PointAt(int index)
        {
	        SKPoint result;
	        if (index < 0 || index > Count - 1)
	        {
		        result = SKPoint.Empty;
	        }
	        else
	        {
		        result = this[_inputRefs[index]];
	        }
	        return result;
        }

        public void Add(SKPoint point)
        {
	        IPointRef pointRef = new PointRef(PadIndex, MotorIndex, _inputPoints.Count);
	        _inputPoints.Add(point);
	        _inputRefs.Add(pointRef);
        }
        public void Add(IPointRef pointRef)
        {
	        _inputPoints.Add(SKPoint.Empty);
	        _inputRefs.Add(pointRef);
        }

        public void Clear()
	    {
		    _inputPoints.Clear();
		    _inputRefs.Clear();
	    }

        public SegmentRef SegmentAt(int startIndex, int endIndex = -1)
        {
            SegmentRef result;
            endIndex = (endIndex == -1) ? startIndex + 1 : endIndex;
            if (startIndex < 0 || startIndex > Count - 1 || endIndex < 0 || endIndex > Count - 1)
            {
                result = SegmentRef.Empty;
            }
            else
            {
                result = new SegmentRef(_inputRefs[startIndex], _inputRefs[endIndex]);

            }
            return result;
        }
        public SKSegment Line => new SKSegment(PointAt(0), PointAt(1));
        public SKSegment LineAt(int start) => new SKSegment(PointAt(start), PointAt(start + 1));
        public SKSegment LineSegment(int start, int end) => new SKSegment(PointAt(start), PointAt(end));

        public float Length(int startIndex) => SegmentAt(startIndex).Length();
        public SKPoint PointAlongLine(int startIndex, int endIndex, float t) => SegmentAt(startIndex, endIndex).PointAlongLine(t);
        public SKPoint SKPointFromStart(int startIndex, float dist) => SegmentAt(startIndex).SKPointFromStart(dist);
        public SKPoint SKPointFromEnd(int startIndex, float dist) => SegmentAt(startIndex).SKPointFromEnd(dist);
        public SKPoint OrthogonalPoint(int startIndex, SKPoint pt, float offset) => SegmentAt(startIndex).OrthogonalPoint(pt, offset);

        public int GetSnapPoint(SKPoint input, float maxDist = 6.0f)
        {
            var result = -1;
            var dist = maxDist * maxDist;
            int index = 0;
            foreach (var inputRef in _inputRefs)
            {
	            var skPoint = this[inputRef];
                if (input.SquaredDistanceTo(skPoint) < dist)
                {
                    result = index;
                    break;
                }
                index++;
            }
            return result;
        }

        public SKPoint[] EndArrow(int startIndex, float dist = 8f) => SegmentAt(startIndex).EndArrow(dist);

        public IEnumerator<SKPoint> GetEnumerator()
        {
	        for (int i = 0; i < Count; i++)
	        {
		        yield return (PointAt(i));
	        }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
	        for (int i = 0; i < Count; i++)
	        {
		        yield return (PointAt(i));
	        }
        }
    }
}
