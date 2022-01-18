using System.Collections.Generic;
using SkiaSharp;
using Slugs.Entities;
using Slugs.Extensions;
using Slugs.Slugs;
using IPoint = Slugs.Entities.IPoint;

namespace Slugs.Input
{
	public class UIData
    {
	    public readonly Dictionary<int, Pad> Pads = new Dictionary<int, Pad>();
	    public Pad PadAt(int key) => Pads[key];

        public SKPoint DownPoint;
	    public SKPoint CurrentPoint;
	    public SKPoint SnapPoint;
	    public IPoint StartHighlight;
	    public DragRef DragRef = new DragRef();

	    public bool IsDown => DownPoint != SKPoint.Empty;
	    public bool IsDragging => !DragRef.IsEmpty;
	    public bool IsDraggingPoint => !DragRef.IsEmpty && !DragRef.IsLine;

	    public readonly List<SKPoint> DragSegment = new List<SKPoint>();
	    public readonly List<SKPoint> ClickData = new List<SKPoint>();
        public readonly List<SKPoint> DragPath = new List<SKPoint>();

        public List<IPoint> HighlightPoints = new List<IPoint>();
        public bool HasHighlightPoint => HighlightPoints.Count > 0;
        public IPoint FirstHighlightPoint => HasHighlightPoint ? HighlightPoints[0] : VPoint.Empty;
        public Trait HighlightLine = Trait.Empty;
        public bool HasHighlightLine => HighlightLine != Trait.Empty;
        public SKPoint GetHighlightPoint() => HighlightPoints.Count > 0 ? HighlightPoints[0].SKPoint : SKPoint.Empty;
        public SKSegment GetHighlightLine() => HighlightLine.Segment;


        private double _unitPush = 10;
	    private double _unitPull = 0;
        public double UnitPull
        {
	        get => _unitPull;
	        set
	        {
		        _unitPull = value;
		        Pad.ActiveSlug = new Slug(_unitPull, _unitPush);
	        }
        }
        public double UnitPush
        {
	        get => _unitPush;
	        set
	        {
		        _unitPush = value;
		        Pad.ActiveSlug = new Slug(_unitPull, _unitPush);
	        }
        }
    }
}
