using System.Collections;
using SkiaSharp;
using Slugs.Pads;

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
        //public Dictionary<PointRef, SKPoint> MovingPoints { get; } = new Dictionary<PointRef, SKPoint>();
        private List<PointRef> PointRefs = new List<PointRef>();
        private List<SKPoint> OriginalPoints = new List<SKPoint>();
        public int Count => PointRefs.Count;

        public DragRef()
        {
        }

        public DragRef(SKPoint origin, params PointRef[] startPoints)
	    {
		    Origin = origin;
		    Add(startPoints);
	    }

        public PointRef this[int index]
        {
	        get => index >= 0 && index < PointRefs.Count ? PointRefs[index] : PointRef.Empty;
        }
        public SKPoint OriginalValue(int index) => index >= 0 && index < OriginalPoints.Count ? OriginalPoints[index] : SKPoint.Empty;

        public void Clear()
        {
	        PointRefs.Clear();
	        OriginalPoints.Clear();
        }

        public void Add(params PointRef[] points)
	    {
		    foreach (var startPoint in points)
		    {
			    PointRefs.Add(startPoint);
			    OriginalPoints.Add(startPoint.SKPoint);
		    }
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
