using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Entities;
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
	    public RenderStatus RenderStatus { get; }

        private UIStatus _st = new UIStatus();
        public double UnitPull { get => _st.UnitPull; set => _st.UnitPull = value; }
        public double UnitPush { get => _st.UnitPush; set => _st.UnitPush = value; }

        public SlugAgent(SlugRenderer renderer)
	    {
		    ActiveAgent = this;
		    SlugAgent.Pads[WorkingPad.PadIndex] = WorkingPad;
		    SlugAgent.Pads[InputPad.PadIndex] = InputPad;

            _renderer = renderer;
		    SlugPad.ActiveSlug = new Slug(_st.UnitPull, _st.UnitPush);
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
	        _st.DownPoint = SKPoint.Empty;
	        _st.CurrentPoint = SKPoint.Empty;
	        _st.SnapPoint = SKPoint.Empty;
            _st.DragSegment.Clear();
	        _st.ClickData.Clear();
            _st.DragPath.Clear();
	        WorkingPad.Clear();
	        _st.DragRef.Clear();

	        _st.DownPoint = SKPoint.Empty;
	        _st.StartHighlight = PointRef.Empty;
        }

        public void Draw()
	    {
		    _renderer.Draw();
	    }

	    public bool MouseDown(MouseEventArgs e)
	    {
		    _st.CurrentPoint = e.Location.ToSKPoint();
            SetHighlight();
            _st.DownPoint = _st.SnapPoint;
            _st.DragRef.Origin = _st.CurrentPoint;
		    if (InputPad.HasHighlightPoint && CurrentKey != Keys.ControlKey)
		    {
			    _st.DragRef.Add(InputPad.HighlightPoints);
		    }
		    else if(!InputPad.HighlightLine.IsEmpty && CurrentKey != Keys.ControlKey)
		    {
			    _st.DragRef.Add(InputPad.HighlightLine.StartRef, InputPad.HighlightLine.EndRef, true);
		    }
		    else
            {
			    _st.DragSegment.Add(_st.SnapPoint);
			    _st.DragPath.Add(_st.SnapPoint);
                _st.StartHighlight = InputPad.FirstHighlightPoint;
            }

            return true;
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
            WorkingPad.Clear();
            _st.CurrentPoint = e.Location.ToSKPoint();
            SetHighlight();
            SetDragging();
            SetCreating();

            return true;
        }

	    public bool MouseUp(MouseEventArgs e)
	    {
		    _st.CurrentPoint = e.Location.ToSKPoint();
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
		    var snap = InputPad.GetSnapPoints(_st.CurrentPoint, _st.DragRef);
		    if (snap.Count > 0)
		    {
			    hasChange = true;
			    InputPad.HighlightPoints = snap;
			    InputPad.HighlightLine = SegRef.Empty;
		    }
		    else
		    {
			    InputPad.HighlightPoints.Clear();
			    var snapLine = InputPad.GetSnapLine(_st.CurrentPoint);
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

		    _st.SnapPoint = _st.CurrentPoint;
		    if (InputPad.HasHighlightPoint)
		    {
			    _st.SnapPoint = InputPad.GetHighlightPoint();
		    }
            else if (InputPad.HasHighlightLine)
		    {
			    _st.SnapPoint = InputPad.GetHighlightLine().ProjectPointOnto(_st.CurrentPoint);
		    }

		    return hasChange;
	    }

	    private bool SetCreating(bool final = false)
	    {
		    var result = false;
		    if (_st.IsDragging)
		    {
			    if (final)
			    {
				    _st.DragRef.OffsetValues(_st.SnapPoint);
				    if (_st.IsDraggingPoint && InputPad.HasHighlightPoint)
				    {
                        MergePointRefs(_st.DragRef.PointRefs, InputPad.FirstHighlightPoint, _st.SnapPoint);
				    }
                    else if (_st.IsDraggingPoint && InputPad.HasHighlightLine)
				    {
					    var dragPoint = _st.DragRef.PointRefs[0];
					    var vp = InputPad.HighlightLine.GetVirtualPointFor(_st.SnapPoint);
                        //UpdatePointRef(dragPoint, vp);
					    // replace last point with Virutal SKPoint Ref for line being snapped to.
				    }
				    result = true;
			    }
		    }
            else if (_st.IsDown)
		    {
			    _st.DragPath.Add(_st.SnapPoint);
			    DataMap.CreateIn(WorkingPad, _st.DownPoint, _st.SnapPoint);
			    if (final)
			    {
				    _st.DragSegment.Add(_st.SnapPoint);
				    _st.ClickData.Add(_st.SnapPoint);
				    _st.DragPath.Add(_st.SnapPoint);
				    if (_st.DragSegment[0].DistanceTo(_st.DragSegment[1]) > 10)
				    {
					    var newDataMap = DataMap.CreateIn(InputPad, _st.DragSegment);
					    if (!_st.StartHighlight.IsEmpty)
					    {
						    MergePointRefs(new List<IPointRef>() { newDataMap.FirstRef }, _st.StartHighlight, _st.StartHighlight.SKPoint);
					    }
					    if (InputPad.HasHighlightPoint)
					    {
						    MergePointRefs(new List<IPointRef>() { newDataMap.LastRef }, InputPad.FirstHighlightPoint, _st.SnapPoint);
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
		    if (_st.IsDragging)
		    {
			    _st.DragRef.OffsetValues(_st.CurrentPoint);
			    result = true;
		    }
            return result;
	    }
    }
}
