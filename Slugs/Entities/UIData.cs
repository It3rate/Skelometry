using SkiaSharp;
using Slugs.Extensions;
using Slugs.Input;
using Slugs.Pads;
using Slugs.Slugs;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UIData
    {
	    public readonly Dictionary<int, EntityPad> Pads = new Dictionary<int, EntityPad>();
	    public EntityPad PadAt(int key) => Pads[key];

        public SKPoint DownPoint;
	    public SKPoint CurrentPoint;
	    public SKPoint SnapPoint;
	    public IPointRef StartHighlight;
	    public DragRef DragRef = new DragRef();

	    public bool IsDown => DownPoint != SKPoint.Empty;
	    public bool IsDragging => !DragRef.IsEmpty;
	    public bool IsDraggingPoint => !DragRef.IsEmpty && !DragRef.IsLine;

	    public readonly List<SKPoint> DragSegment = new List<SKPoint>();
	    public readonly List<SKPoint> ClickData = new List<SKPoint>();
        public readonly List<SKPoint> DragPath = new List<SKPoint>();

        public List<IPointRef> HighlightPoints = new List<IPointRef>();
        public bool HasHighlightPoint => HighlightPoints.Count > 0;
        public IPointRef FirstHighlightPoint => HasHighlightPoint ? HighlightPoints[0] : PtRef.Empty;
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
		        EntityPad.ActiveSlug = new Slug(_unitPull, _unitPush);
	        }
        }
        public double UnitPush
        {
	        get => _unitPush;
	        set
	        {
		        _unitPush = value;
		        EntityPad.ActiveSlug = new Slug(_unitPull, _unitPush);
	        }
        }
    }
}
