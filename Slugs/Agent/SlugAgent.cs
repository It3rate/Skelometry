using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Extensions;
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
	    public static IAgent ActiveAgent { get; private set; }

	    public static readonly Dictionary<int, SlugPad> Pads = new Dictionary<int, SlugPad>();
	    public readonly SlugPad WorkingPad = new SlugPad(PadKind.Working);
	    public readonly SlugPad InputPad = new SlugPad(PadKind.Drawn);

	    private readonly SlugRenderer _renderer;
	    public RenderStatus Status { get; }


	    private double _unitPull = 0;
	    public double _unitPush = 10;

	    private bool IsDown => DownPoint != SKPoint.Empty;
	    private SKPoint DownPoint;
	    private SKPoint CurrentPoint;
	    private SKPoint SnapPoint;
	    private IPointRef StartHighlight;
	    private readonly List<SKPoint> DragSegment = new List<SKPoint>();
	    private readonly List<SKPoint> ClickData = new List<SKPoint>();
	    private readonly List<SKPoint> DragPath = new List<SKPoint>();
	    public DragRef DragRef = new DragRef();
	    private bool IsDragging => !DragRef.IsEmpty;
	    private bool IsDraggingPoint => !DragRef.IsEmpty && !DragRef.IsLine;

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

	    public SKPoint this[IPointRef pointRef]
	    {
		    get
		    {
			    SKPoint result;
			    if (!pointRef.IsEmpty)
			    {
				    var pad = SlugAgent.Pads[pointRef.PadIndex];
				    var dataMap = pad.InputFromIndex(pointRef.EntityIndex);
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
			    var dataMap = pad.InputFromIndex(pointRef.EntityIndex);
			    dataMap[pointRef] = value;
		    }
	    }

	    public void MergePointRefs(List<IPointRef> fromList, IPointRef to, SKPoint position)
	    {
		    to.SKPoint = position;
		    foreach (var from in fromList)
		    {
			    UpdatePointRef(from, to);
		    }
	    }

	    public void UpdatePointRef(IPointRef from, IPointRef to)
	    {
		    var pad = SlugAgent.Pads[from.PadIndex];
		    var dataMap = pad.InputFromIndex(from.EntityIndex);
		    dataMap[from.FocalIndex] = to;
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
            StartHighlight = PointRef.Empty;
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
		    if (InputPad.HasHighlightPoint && CurrentKey != Keys.ControlKey)
		    {
                DragRef.Add(InputPad.HighlightPoints);
		    }
		    else if(!InputPad.HighlightLine.IsEmpty && CurrentKey != Keys.ControlKey)
		    {
			    DragRef.Add(InputPad.HighlightLine.StartRef, InputPad.HighlightLine.EndRef, true);
		    }
		    else
            {
			    DragSegment.Add(SnapPoint);
			    DragPath.Add(SnapPoint);
			    StartHighlight = InputPad.FirstHighlightPoint;
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
		    var snap = InputPad.GetSnapPoints(CurrentPoint, DragRef);
		    if (snap.Count > 0)
		    {
			    hasChange = true;
			    InputPad.HighlightPoints = snap;
			    InputPad.HighlightLine = SegRef.Empty;
		    }
		    else
		    {
			    InputPad.HighlightPoints.Clear();
			    var snapLine = InputPad.GetSnapLine(CurrentPoint);
			    if (snapLine.IsEmpty)
			    {
				    InputPad.HighlightLine = SegRef.Empty;
			    }
			    else
			    {
				    hasChange = true;
				    InputPad.HighlightLine = snapLine;
			    }
		    }

		    SnapPoint = CurrentPoint;
		    if (InputPad.HasHighlightPoint)
		    {
			    SnapPoint = InputPad.GetHighlightPoint();
		    }
            else if (InputPad.HasHighlightLine)
		    {
			    SnapPoint = InputPad.GetHighlightLine().ProjectPointOnto(CurrentPoint);
		    }

		    return hasChange;
	    }

	    private bool SetCreating(bool final = false)
	    {
		    var result = false;
		    if (IsDragging)
		    {
			    if (final)
			    {
				    DragRef.OffsetValues(SnapPoint);
				    if (IsDraggingPoint && InputPad.HasHighlightPoint)
				    {
                        MergePointRefs(DragRef.PointRefs, InputPad.FirstHighlightPoint, SnapPoint);
				    }
                    else if (IsDraggingPoint && InputPad.HasHighlightLine)
				    {
					    var dragPoint = DragRef.PointRefs[0];
					    var vp = InputPad.HighlightLine.GetVirtualPointFor(SnapPoint);
                        //UpdatePointRef(dragPoint, vp);
					    // replace last point with Virutal SKPoint Ref for line being snapped to.
				    }
				    result = true;
			    }
		    }
            else if (IsDown)
		    {
			    DragPath.Add(SnapPoint);
			    DataMap.CreateIn(WorkingPad, DownPoint, SnapPoint);
			    if (final)
			    {
				    DragSegment.Add(SnapPoint);
				    ClickData.Add(SnapPoint);
				    DragPath.Add(SnapPoint);
				    if (DragSegment[0].DistanceTo(DragSegment[1]) > 10)
				    {
					    var newDataMap = DataMap.CreateIn(InputPad, DragSegment);
					    if (!StartHighlight.IsEmpty)
					    {
						    MergePointRefs(new List<IPointRef>() { newDataMap.FirstRef }, StartHighlight, StartHighlight.SKPoint);
					    }
					    if (InputPad.HasHighlightPoint)
					    {
						    MergePointRefs(new List<IPointRef>() { newDataMap.LastRef }, InputPad.FirstHighlightPoint, SnapPoint);
					    }
                    }
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
			    result = true;
		    }
            return result;
	    }
    }
}
