//using System.Collections;
//using System.Dynamic;
//using SkiaSharp;
//using Slugs.Agent;
//using Slugs.Entities;
//using Slugs.Extensions;
//using Slugs.Pads;
//using Slugs.Slugs;

//namespace Slugs.Input
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;

//    public class DataMap : IEnumerable<SKPoint>
//    {
//        public static readonly DataMap Empty = new DataMap();
//	    public bool IsEmpty = false;

//        public int PadIndex;
//	    public int DataMapIndex;

//	    // refs do not need to point to the internal point store, but will normally start this way for user input.
//        private readonly List<SKPoint> _inputPoints = new List<SKPoint>(); 
//	    private readonly List<IPointRef> _inputRefs = new List<IPointRef>();
//	    public int Count => _inputRefs.Count;

//	    private readonly List<Slug> _slugs = new List<Slug>();

//        private DataMap()
//	    {
//		    IsEmpty = true;
//		    // SKPoint.Empty, SkPointExtension.MinPoint, SkPointExtension.MaxPoint
//        }

//	    private DataMap(SlugPad pad, params SKPoint[] points)
//	    {
//		    pad?.AddEntity(this);
//		    foreach (var point in points)
//		    {
//			    this.AddEntity(point);
//		    }
//	    }
//	    private DataMap(SlugPad pad, params IPointRef[] pointRefs)
//        {
//	        pad.AddEntity(this);
//            foreach (var pointRef in pointRefs)
//		    {
//			    this.AddEntity(pointRef);
//		    }
//	    }

//	    public static DataMap CreateIn(SlugPad pad, params SKPoint[] points)
//	    {
//		    return new DataMap(pad, points);
//	    }
//	    public static DataMap CreateIn(SlugPad pad,List<SKPoint> points)
//	    {
//		    return new DataMap(pad, points.ToArray());
//	    }
//        public static DataMap CreateIn(SlugPad pad, params IPointRef[] pointRefs)
//	    {
//		    return new DataMap(pad, pointRefs);
//	    }

//        public bool HasSlug(SlugRef slugRef) =>
//	        slugRef.PadIndex == PadIndex && slugRef.DataMapIndex == DataMapIndex && HasSlugAtIndex(slugRef.SlugIndex);
//        public bool HasSlugAtIndex(int index) => index >= 0 && index < _slugs.Count;
//        public int SlugCount => _slugs.Count;
//        public Slug SlugAt(int index) => (index >= 0 && index < _slugs.Count) ? _slugs[index] : Slug.Empty; // default to unit slug instead of empty?
//        public SlugRef AddSlug(Slug item)
//        {
//	        _slugs.AddEntity(item);
//	        return new SlugRef(PadIndex, DataMapIndex, _slugs.Count - 1);
//        }
//        public bool RemoveSlugAt(int index)
//        {
//	        bool result = false;
//	        if (HasSlugAtIndex(index))
//	        {
//		        result = true;
//		        _slugs.RemoveAt(index);
//	        }
//	        return result;
//        }
//        public bool RemoveSlug(SlugRef slugRef)
//        {
//	        bool result = false;
//	        if (HasSlug(slugRef))
//	        {
//		        result = true;
//                _slugs.RemoveAt(slugRef.SlugIndex);
//	        }
//	        return result;
//        }

//        private bool IsOwn(IPointRef pointRef) => pointRef.PadIndex == PadIndex && pointRef.EntityKey == DataMapIndex;
//        private SlugPad PadAt(int index) => SlugAgent.ActiveAgent.PadAt(index);
//        public SKPoint this[IPointRef pointRef]
//        {
//	        get => IsOwn(pointRef) ? _inputPoints[pointRef.FocalKey] : SlugAgent.ActiveAgent[pointRef];
//	        set
//	        {
//		        if (IsOwn(pointRef))
//		        {
//			        _inputPoints[pointRef.FocalKey] = value;
//		        }
//		        else
//		        {
//			        SlugAgent.ActiveAgent[pointRef] = value;
//                }
//	        }
//        }
//        public IPointRef this[int index]
//        {
//	        get => _inputRefs[index];
//	        set => _inputRefs[index] = value;
//        }
//        public IPointRef FirstRef => _inputRefs.Count > 0 ? _inputRefs[0] : PointRef.Empty;
//        public IPointRef LastRef => _inputRefs.Count > 0 ? _inputRefs[_inputRefs.Count - 1] : PointRef.Empty;
//        public SKPoint PointAt(int index)
//        {
//	        SKPoint result;
//	        if (index < 0 || index > Count - 1)
//	        {
//		        result = SKPoint.Empty;
//	        }
//	        else
//	        {
//		        result = this[_inputRefs[index]];
//	        }
//	        return result;
//        }

//        public void AddEntity(SKPoint point)
//        {
//	        IPointRef pointRef = new PointRef(PadIndex, DataMapIndex, _inputPoints.Count);
//	        _inputPoints.AddEntity(point);
//	        _inputRefs.AddEntity(pointRef);
//        }
//        public void AddEntity(IPointRef pointRef)
//        {
//	        _inputPoints.AddEntity(SKPoint.Empty);
//	        _inputRefs.AddEntity(pointRef);
//        }

//        public void Clear()
//	    {
//		    _inputPoints.Clear();
//		    _inputRefs.Clear();
//	    }

//        public SegRef SegmentAt(int startIndex, int endIndex = -1)
//        {
//            SegRef result;
//            endIndex = (endIndex == -1) ? startIndex + 1 : endIndex;
//            if (startIndex < 0 || startIndex > Count - 1 || endIndex < 0 || endIndex > Count - 1)
//            {
//                result = SegRef.Empty;
//            }
//            else
//            {
//                result = new SegRef(_inputRefs[startIndex], _inputRefs[endIndex]);

//            }
//            return result;
//        }
//        public SKSegment Line => new SKSegment(PointAt(0), PointAt(1));
//        public SKSegment LineAt(int start) => new SKSegment(PointAt(start), PointAt(start + 1));
//        public SKSegment LineSegment(int start, int end) => new SKSegment(PointAt(start), PointAt(end));

//        public float Length(int startIndex) => SegmentAt(startIndex).Length();
//        public SKPoint PointAlongLine(int startIndex, int endIndex, float t) => SegmentAt(startIndex, endIndex).PointAlongLine(t);
//        public SKPoint SKPointFromStart(int startIndex, float dist) => SegmentAt(startIndex).SKPointFromStart(dist);
//        public SKPoint SKPointFromEnd(int startIndex, float dist) => SegmentAt(startIndex).SKPointFromEnd(dist);
//        public SKPoint OrthogonalPoint(int startIndex, SKPoint pt, float offset) => SegmentAt(startIndex).OrthogonalPoint(pt, offset);

//        public int GetSnapPoint(SKPoint input, float maxDist = 6.0f)
//        {
//            var result = -1;
//            var dist = maxDist * maxDist;
//            int index = 0;
//            foreach (var inputRef in _inputRefs)
//            {
//	            var skPoint = this[inputRef];
//                if (input.SquaredDistanceTo(skPoint) < dist)
//                {
//                    result = index;
//                    break;
//                }
//                index++;
//            }
//            return result;
//        }

//        public SKPoint[] EndArrow(int startIndex, float dist = 8f) => SegmentAt(startIndex).EndArrow(dist);

//        public IEnumerator<SKPoint> GetEnumerator()
//        {
//	        for (int i = 0; i < Count; i++)
//	        {
//		        yield return (PointAt(i));
//	        }
//        }
//        IEnumerator IEnumerable.GetEnumerator()
//        {
//	        for (int i = 0; i < Count; i++)
//	        {
//		        yield return (PointAt(i));
//	        }
//        }
//    }
//}
