using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Input;
using Slugs.Pads;
using Slugs.Renderer;
using Slugs.Slugs;

namespace Slugs.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SlugAgent : IAgent
    {
	    public static SlugAgent ActiveAgent { get; private set; }
        public static readonly Dictionary<int, SlugPad> Pads = new Dictionary<int, SlugPad>();

	    private double _unitPull = 0;
	    public double _unitPush = 10;

	    private readonly SlugRenderer _renderer;
	    public RenderStatus Status { get; }

	    public readonly SlugPad WorkingPad = new SlugPad(PadKind.Working);
	    public readonly SlugPad InputPad = new SlugPad(PadKind.Drawn);

	    private SKPoint DownPoint;
	    private SKPoint CurrentPoint;
	    private SKPoint SnapPoint;
	    private readonly List<SKPoint> DragSegment = new List<SKPoint>();
	    private readonly List<SKPoint> ClickData = new List<SKPoint>();
	    private readonly List<SKPoint> DragPath = new List<SKPoint>();

	    public DragRef DragRef = new DragRef();
	    private bool IsDragging => DragRef.Count > 0;
        public PointRef DraggingPoint => IsDraggingPoint ? DragRef[0] : PointRef.Empty;
        private bool IsDraggingPoint => DragRef.Count == 1;
	    //public SegmentRef DraggingLine = SegmentRef.Empty;
	    private bool IsDraggingLine => DragRef.Count > 1;

        private bool IsDown => DownPoint != SKPoint.Empty;
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

	    public SlugAgent(SlugRenderer renderer)
	    {
		    ActiveAgent = this;
		    SlugAgent.Pads[WorkingPad.PadIndex] = WorkingPad;
		    SlugAgent.Pads[InputPad.PadIndex] = InputPad;

            _renderer = renderer;
		    SlugPad.ActiveSlug = new Slug(UnitPull, UnitPush);
		    //var pl = (new DataPoints(new SKPoint(_renderer.Width / 2.0f, 20f), new SKPoint(_renderer.Width * 3.0f / 4.0f, 20f)));
		    var pl = DataMap.CreateIn(InputPad, new SKPoint(200, 100), new SKPoint(400, 300));
		    //InputPad.Add(pl);
		    _renderer.Pads.Add(WorkingPad);
		    _renderer.Pads.Add(InputPad);
		    ClearMouse();
	    }

	    public SKPoint this[PointRef pointRef]
	    {
		    get
		    {
			    SKPoint result;
			    if (!pointRef.IsEmpty)
			    {
				    var pad = SlugAgent.Pads[pointRef.PadIndex];
				    var dataMap = pad.InputFromIndex(pointRef.DataMapIndex);
				    result = dataMap[pointRef];
			    }
			    else
			    {
				    result = SKPoint.Empty;
                }
			    return result;
		    } 
		    set
		    {
			    var pad = SlugAgent.Pads[pointRef.PadIndex];
			    var dataMap = pad.InputFromIndex(pointRef.DataMapIndex);
			    dataMap[pointRef] = value;
		    }
	    }

	    public void UpdatePointRef(PointRef target, PointRef value)
	    {
		    var pad = SlugAgent.Pads[target.PadIndex];
		    var dataMap = pad.InputFromIndex(target.DataMapIndex);
		    dataMap[target.PointIndex] = value;
	    }

	    public void Clear()
        {
        }

        public void ClearMouse()
        {
	        DownPoint = SKPoint.Empty;
	        CurrentPoint = SKPoint.Empty;
	        SnapPoint = SKPoint.Empty;
            DragSegment.Clear();
	        ClickData.Clear();
            DragPath.Clear();
	        WorkingPad.Clear();
	        DragRef.Clear();

            DownPoint = SKPoint.Empty;
	        //DraggingPoint = PointRef.Empty;
	        //DraggingLine = SegmentRef.Empty;
        }

        public void Draw()
	    {
		    _renderer.Draw();
	    }

	    public bool MouseDown(MouseEventArgs e)
	    {
		    CurrentPoint = e.Location.ToSKPoint();
            SetHighlight();
		    DownPoint = SnapPoint;
		    DragRef.Origin = CurrentPoint;
		    if (!InputPad.HighlightPoint.IsEmpty && CurrentKey != Keys.ControlKey)
		    {
                DragRef.Add(InputPad.HighlightPoint);
			    //DraggingPoint = InputPad.HighlightPoint;
		    }
		    else if(!InputPad.HighlightLine.IsEmpty && CurrentKey != Keys.ControlKey)
		    {
			    DragRef.Add(InputPad.HighlightLine.StartRef, InputPad.HighlightLine.EndRef);
                //DraggingLine = InputPad.HighlightLine;
		    }
		    else
            {
			    DragSegment.Add(SnapPoint);
			    DragPath.Add(SnapPoint);
		    }

            return true;
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
            WorkingPad.Clear();
		    CurrentPoint = e.Location.ToSKPoint();
            SetHighlight();
            SetDragging();
            SetCreating();

            return true;
        }

	    public bool MouseUp(MouseEventArgs e)
	    {
		    CurrentPoint = e.Location.ToSKPoint();
		    SetHighlight(true);
            SetDragging();
		    SetCreating(true);

            ClearMouse();
            return true;
        }

	    private Keys CurrentKey;
	    public bool KeyDown(KeyEventArgs e)
	    {
		    CurrentKey = e.KeyCode;
		    return true;
        }

	    public bool KeyUp(KeyEventArgs e)
	    {
		    CurrentKey = Keys.None;
            return true;
        }

	    private bool SetHighlight(bool final = false)
	    {
		    bool hasChange = false;
		    var snap = InputPad.GetSnapPoints(CurrentPoint, DraggingPoint);
		    if (snap.Length > 0)
		    {
			    hasChange = true;
			    InputPad.HighlightPoint = snap[0];
			    InputPad.HighlightLine = SegmentRef.Empty;
		    }
		    else
		    {
			    InputPad.HighlightPoint = PointRef.Empty;
			    var snapLine = InputPad.GetSnapLine(CurrentPoint);
			    if (snapLine.IsEmpty)
			    {
				    InputPad.HighlightLine = SegmentRef.Empty;
			    }
			    else
			    {
				    hasChange = true;
				    InputPad.HighlightLine = snapLine;
			    }
		    }
		    SnapPoint = InputPad.HighlightPoint.IsEmpty ? CurrentPoint : InputPad.GetHighlightPoint();

		    return hasChange;
	    }

	    private bool SetCreating(bool final = false)
	    {
		    var result = false;
		    if (IsDragging)
		    {
			    if (final)
			    {
				    DragRef.OffsetValues(CurrentPoint);
				    if (IsDraggingPoint && !InputPad.HighlightPoint.IsEmpty)
				    {
					    UpdatePointRef(DraggingPoint, InputPad.HighlightPoint);
				    }
				    result = true;
			    }
		    }
		    //else if (IsDraggingLine)
		    //{
			   // if (final)
			   // {
				  //  UpdatePointRef(DraggingPoint, InputPad.HighlightPoint);
				  //  result = true;
			   // }
		    //}
            else if (IsDown)
		    {
			    DragPath.Add(SnapPoint);
			    DataMap.CreateIn(WorkingPad, DownPoint, SnapPoint);
			    if (final)
			    {
				    DragSegment.Add(SnapPoint);
				    ClickData.Add(SnapPoint);
				    DragPath.Add(SnapPoint);
					DataMap.CreateIn(InputPad, DragSegment);
                }
			    result = true;
		    }
		    return result;
	    }
        private bool SetDragging()
	    {
		    var result = false;
		    if (IsDragging)
		    {
			    DragRef.OffsetValues(CurrentPoint);
                //InputPad.UpdatePoint(DraggingPoint, SnapPoint);
			    result = true;
		    }
      //      else if (IsDraggingLine)
		    //{
			   // InputPad.UpdateLine(DraggingLine, SnapPoint);
			   // result = true;
		    //}
            return result;
	    }
    }
}
