using SkiaSharp;
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

    public class UIStatus
    {
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

	    private double _unitPush = 10;
	    private double _unitPull = 0;
        public double UnitPull
        {
	        get => _unitPull;
	        set
	        {
		        _unitPull = value;
		        SlugPad.ActiveSlug = new Slug(_unitPull, _unitPush);
	        }
        }
        public double UnitPush
        {
	        get => _unitPush;
	        set
	        {
		        _unitPush = value;
		        SlugPad.ActiveSlug = new Slug(_unitPull, _unitPush);
	        }
        }
    }
}
