using System.Collections;
using SkiaSharp;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Slugs;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DragRef
    {
	    public SKPoint Origin { get; set; }
        //public Dictionary<IPointRef, SKPoint> MovingPoints { get; } = new Dictionary<IPointRef, SKPoint>();
        public List<IPointRef> PointRefs { get; private set; } = new List<IPointRef>();
        private List<SKPoint> OriginalPoints { get; } = new List<SKPoint>();
        public int Count => PointRefs.Count;
        public bool IsLine { get; private set; }

        public DragRef()
        {
        }

        public DragRef(SKPoint origin, params IPointRef[] startPoints)
        {
	        Origin = origin;
	        Add(startPoints);
        }

        public IPointRef this[int index]
        {
	        get => index >= 0 && index < PointRefs.Count ? PointRefs[index] : PtRef.Empty;
        }
        public SKPoint OriginalValue(int index) => index >= 0 && index < OriginalPoints.Count ? OriginalPoints[index] : SKPoint.Empty;

        public void Clear()
        {
	        IsLine = false;
	        PointRefs.Clear();
	        OriginalPoints.Clear();
        }

        public bool IsEmpty => PointRefs.Count == 0;
        public bool Contains(IPointRef pointRef) => PointRefs.Contains(pointRef);

        public void Add(params IPointRef[] points)
	    {
		    foreach (var startPoint in points)
		    {
			    PointRefs.Add(startPoint);
			    OriginalPoints.Add(startPoint.SKPoint);
		    }
        }
        public void Add(List<IPointRef> points)
        {
            Add(points.ToArray());
        }
        public void Add(IPointRef start, IPointRef end, bool isLine)
        {
	        Add(start, end);
	        IsLine = isLine;
        }
        public void AddToFront(IPointRef point)
        {
	        PointRefs.Insert(0, point);
	        OriginalPoints.Insert(0, point.SKPoint);
        }

        public void OffsetValues(SKPoint currentPoint)
	    {
		    var diff = Origin - currentPoint;
		    for (int i = 0; i < PointRefs.Count; i++)
		    {
			    var key = PointRefs[i];
                key.SKPoint = OriginalPoints[i] - diff;
            }
	    }
        public void ResetValues()
	    {
		    for (int i = 0; i < PointRefs.Count; i++)
		    {
			    var key = PointRefs[i];
			    key.SKPoint = OriginalPoints[i];
		    }
	    }
    }
}
