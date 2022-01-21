using System.Collections;
using SkiaSharp;
using Slugs.Entities;
using Slugs.Pads;
using IPoint = Slugs.Entities.IPoint;

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
        public List<IPoint> PointRefs { get; private set; } = new List<IPoint>();
        private List<SKPoint> OriginalPoints { get; } = new List<SKPoint>();
        public int Count => PointRefs.Count;
        public bool IsLine { get; private set; }

        public DragRef()
        {
        }

        public DragRef(SKPoint origin, params IPoint[] startPoints)
        {
	        Origin = origin;
	        Add(startPoints);
        }

        public IPoint this[int index]
        {
	        get => index >= 0 && index < PointRefs.Count ? PointRefs[index] : Point.Empty;
        }

        public void Clear()
        {
	        IsLine = false;
	        PointRefs.Clear();
	        OriginalPoints.Clear();
        }

        public bool IsEmpty => PointRefs.Count == 0;
        public bool Contains(IPoint point) => PointRefs.Contains(point);

        public void Add(params IPoint[] points)
	    {
		    foreach (var startPoint in points)
		    {
			    PointRefs.Add(startPoint);
			    OriginalPoints.Add(startPoint.SKPoint);
		    }
        }
        public void Add(List<IPoint> points)
        {
            Add(points.ToArray());
        }
        public void Add(IPoint start, IPoint end, bool isLine)
        {
	        Add(start, end);
	        IsLine = isLine;
        }
        public void AddToFront(IPoint point)
        {
	        PointRefs.Insert(0, point);
	        OriginalPoints.Insert(0, point.SKPoint);
        }

        public void OffsetValues(SKPoint currentPoint)
	    {
		    var diff = Origin - currentPoint;
		    for (int i = 0; i < PointRefs.Count; i++)
		    {
			    var pt = PointRefs[i];
                pt.SKPoint = OriginalPoints[i] - diff;
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
