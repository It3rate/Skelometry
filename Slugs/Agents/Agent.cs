using System.Collections.Generic;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Entities;
using Slugs.Extensions;
using Slugs.Input;
using Slugs.Pads;
using Slugs.Renderer;
using Slugs.Slugs;

namespace Slugs.Agents
{
	public class Agent : IAgent
    {
	    public static Agent Current { get; private set; }

	    private readonly Dictionary<int, IPoint> _pointMap = new Dictionary<int, IPoint>();
	    public IEnumerable<IPoint> Points => _pointMap.Values;
	    public IPoint PointAt(int key)
	    {
		    var success = _pointMap.TryGetValue(key, out IPoint result);
		    return success ? result : Point.Empty;
	    }
	    public void SetPointAt(int key, IPoint value)
	    {
		    value.Kind = PointKind.Dirty;
		    _pointMap[key] = value;
	    }
	    public Point CreateTerminalPoint(int padIndex, SKPoint pt)
	    {
		    var ptRef = new Point(padIndex, pt);
		    _pointMap.Add(ptRef.Key, ptRef);
		    return ptRef;
	    }
	    public SegRef CreateTerminalSegRef(int padIndex, SKSegment skSegment)
	    {
		    var a = CreateTerminalPoint(padIndex, skSegment.StartPoint);
		    var b = CreateTerminalPoint(padIndex, skSegment.EndPoint);
		    return new SegRef(a, b);
	    }
	    public SegRef[] CreateTerminalSegRefs(int padIndex, params SKSegment[] segs)
	    {
		    var result = new List<SegRef>(segs.Length);
		    foreach (var skSegment in segs)
		    {
			    result.Add(CreateTerminalSegRef(padIndex, skSegment));
		    }
		    return result.ToArray();
	    }

        public readonly EntityPad WorkingPad;
        public readonly EntityPad InputPad;
        public EntityPad PadAt(int index) => _data.PadAt(index);

        private readonly SlugRenderer _renderer;
        public RenderStatus RenderStatus { get; }

        private int _traitIndexCounter = 4096;

        private UIData _data = new UIData();
        public double UnitPull { get => _data.UnitPull; set => _data.UnitPull = value; }
        public double UnitPush { get => _data.UnitPush; set => _data.UnitPush = value; }

        public Agent(SlugRenderer renderer)
        {
            Current = this;
            WorkingPad = new EntityPad(PadKind.Working, this);
            InputPad = new EntityPad(PadKind.Working, this);
            _data.Pads[WorkingPad.PadIndex] = WorkingPad;
            _data.Pads[InputPad.PadIndex] = InputPad;

            _renderer = renderer;
            _renderer.UIData = _data;

            EntityPad.ActiveSlug = new Slug(_data.UnitPull, _data.UnitPush);
            //var pl = InputPad.AddSegmentEntity(new SKPoint(200, 100), new SKPoint(400, 300));
            var (index, entity, trait) = InputPad.AddEntity(new SKSegment( 200, 100, 400, 300), 1);
            InputPad.AddTrait(index, new SKSegment(290, 100, 490, 300), 1);

            //InputPad.AddEntity(pl);
            //_renderer.Pads.AddEntity(WorkingPad);
            //_renderer.Pads.AddEntity(InputPad);
            ClearMouse();
        }

        //public SKPoint this[IPoint point]
        //{
        //    get
        //    {
	       //     return point.SKPoint;
        //        SKPoint result;
        //        if (!point.IsEmpty)
        //        {
	       //         var pad = PadAt(point.PadIndex);
	       //         var entity = pad.EntityAt(point.EntityKey);
	       //         if (entity.IsEmpty)
	       //         {
		      //          result = point.c;
	       //         }
        //            var dataMap = pad.InputFromIndex(point.EntityKey);
        //            result = dataMap[point];
        //        }
        //        else
        //        {
        //            result = SKPoint.Empty;
        //        }
        //        return result;
        //    }
        //    set
        //    {
        //        var pad = _data.Pads[point.PadIndex];
        //        var dataMap = pad.InputFromIndex(point.EntityKey);
        //        dataMap[point] = value;
        //    }
        //}

        public void MergePointRefs(List<IPoint> fromList, IPoint to, SKPoint position)
        {
            to.SKPoint = position;
            foreach (var from in fromList)
            {
                from.ReplaceWith(to);
            }
        }

        //public void UpdatePointRef(IPoint from, IPoint to)
        //{
        //    var pad = _data.Pads[from.PadIndex];
        //    var dataMap = pad.InputFromIndex(from.EntityKey);
        //    dataMap[from.FocalKey] = to;
        //}

        public void Clear()
        {
        }

        public void ClearMouse()
        {
            _data.DownPoint = SKPoint.Empty;
            _data.CurrentPoint = SKPoint.Empty;
            _data.SnapPoint = SKPoint.Empty;
            _data.DragSegment.Clear();
            _data.ClickData.Clear();
            _data.DragPath.Clear();
            WorkingPad.Clear();
            _data.DragRef.Clear();

            _data.DownPoint = SKPoint.Empty;
            _data.StartHighlight = VPoint.Empty;
        }

        public void Draw()
        {
            _renderer.Draw();
        }

        public bool MouseDown(MouseEventArgs e)
        {
            _data.CurrentPoint = e.Location.ToSKPoint();
            SetHighlight();
            _data.DownPoint = _data.SnapPoint;
            _data.DragRef.Origin = _data.CurrentPoint;
            if (_data.HasHighlightPoint && CurrentKey != Keys.ControlKey)
            {
                _data.DragRef.Add(_data.HighlightPoints);
            }
            else if (!_data.HighlightLine.IsEmpty && CurrentKey != Keys.ControlKey)
            {
                _data.DragRef.Add(_data.HighlightLine.Start, _data.HighlightLine.End, true);
            }
            else
            {
                _data.DragSegment.Add(_data.SnapPoint);
                _data.DragPath.Add(_data.SnapPoint);
                _data.StartHighlight = _data.FirstHighlightPoint;
            }

            return true;
        }

        public bool MouseMove(MouseEventArgs e)
        {
            WorkingPad.Clear();
            _data.CurrentPoint = e.Location.ToSKPoint();
            SetHighlight();
            SetDragging();
            SetCreating();

            return true;
        }

        public bool MouseUp(MouseEventArgs e)
        {
            _data.CurrentPoint = e.Location.ToSKPoint();
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
            var snap = InputPad.GetSnapPoints(_data.CurrentPoint, _data.DragRef);
            if (snap.Count > 0)
            {
                hasChange = true;
                _data.HighlightPoints = snap;
                _data.HighlightLine = Trait.Empty;
            }
            else
            {
                _data.HighlightPoints.Clear();
                var snapLine = InputPad.GetSnapLine(_data.CurrentPoint);
                if (snapLine.IsEmpty)
                {
                    _data.HighlightLine = Trait.Empty;
                }
                else
                {
                    hasChange = true;
                    _data.HighlightLine = snapLine;
                }
            }

            _data.SnapPoint = _data.CurrentPoint;
            if (_data.HasHighlightPoint)
            {
                _data.SnapPoint = _data.GetHighlightPoint();
            }
            else if (_data.HasHighlightLine)
            {
                _data.SnapPoint = _data.GetHighlightLine().ProjectPointOnto(_data.CurrentPoint);
            }

            return hasChange;
        }

        private bool SetCreating(bool final = false)
        {
            var result = false;
            if (_data.IsDragging)
            {
                if (final)
                {
                    _data.DragRef.OffsetValues(_data.SnapPoint);
                    if (_data.IsDraggingPoint && _data.HasHighlightPoint)
                    {
                        MergePointRefs(_data.DragRef.PointRefs, _data.FirstHighlightPoint, _data.SnapPoint);
                    }
                    else if (_data.IsDraggingPoint && _data.HasHighlightLine)
                    {
                        var dragPoint = _data.DragRef.PointRefs[0];
                        //var vp = _data.HighlightLine.GetVirtualPointFor(_data.SnapPoint);
                        //UpdatePointRef(dragPoint, vp);
                        // replace last point with Virutal SKPoint Ref for line being snapped to.
                    }
                    result = true;
                }
            }
            else if (_data.IsDown)
            {
                _data.DragPath.Add(_data.SnapPoint);
                WorkingPad.AddEntity(new SKSegment(_data.DownPoint, _data.SnapPoint), 0);
                //DataMap.CreateIn(WorkingPad, _data.DownPoint, _data.SnapPoint);
                if (final)
                {
                    _data.DragSegment.Add(_data.SnapPoint);
                    _data.ClickData.Add(_data.SnapPoint);
                    _data.DragPath.Add(_data.SnapPoint);
                    if (_data.DragSegment[0].DistanceTo(_data.DragSegment[1]) > 10)
                    {
	                    var (key, entity, trait) = InputPad.AddEntity(new SKSegment(_data.DownPoint,  _data.DragSegment[1]), _traitIndexCounter++);
                        //var newDataMap = DataMap.CreateIn(InputPad, _data.DragSegment);
                        if (!_data.StartHighlight.IsEmpty)
                        {
                            MergePointRefs(new List<IPoint>() { trait.StartRef }, _data.StartHighlight, _data.StartHighlight.SKPoint);
                        }
                        if (_data.HasHighlightPoint)
                        {
                            MergePointRefs(new List<IPoint>() { trait.EndRef }, _data.FirstHighlightPoint, _data.SnapPoint);
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
            if (_data.IsDragging)
            {
                _data.DragRef.OffsetValues(_data.CurrentPoint);
                result = true;
            }
            return result;
        }
    }
}
