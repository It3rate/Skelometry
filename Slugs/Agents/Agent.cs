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
	    private readonly Dictionary<int, Focal> _focalMap = new Dictionary<int, Focal>();

        public readonly Pad WorkingPad;
        public readonly Pad InputPad;
        public Pad PadAt(int index) => _data.PadAt(index);

        private UIData _data = new UIData();
        private readonly SlugRenderer _renderer;
        public double UnitPull { get => _data.UnitPull; set => _data.UnitPull = value; }
        public double UnitPush { get => _data.UnitPush; set => _data.UnitPush = value; }
        public RenderStatus RenderStatus { get; }

        private int _traitIndexCounter = 4096;

        public Agent(SlugRenderer renderer)
        {
            Current = this;
            WorkingPad = new Pad(PadKind.Working, this);
            InputPad = new Pad(PadKind.Working, this);
            _data.Pads[WorkingPad.PadIndex] = WorkingPad;
            _data.Pads[InputPad.PadIndex] = InputPad;

            _renderer = renderer;
            _renderer.UIData = _data;

            Pad.ActiveSlug = new Slug(_data.UnitPull, _data.UnitPush);
            var (index, entity, trait) = InputPad.AddEntity(new SKSegment( 200, 100, 400, 300), 1);
            InputPad.AddTrait(index, new SKSegment(290, 100, 490, 300), 1);

            ClearMouse();
        }

		#region Points
        public IEnumerable<IPoint> Points => _pointMap.Values;
        public IPoint PointAt(int key)
        {
	        var success = _pointMap.TryGetValue(key, out IPoint result);
	        return success ? result : Point.Empty;
        }
        public IPoint TerminalPointAt(int key)
        {
	        var success = _pointMap.TryGetValue(key, out IPoint result);
	        while (success && result.Kind == PointKind.Pointer)
	        {
		        success = _pointMap.TryGetValue(key, out result);
	        }
	        return success ? result : Point.Empty;
        }
        public void SetPointAt(int key, IPoint value)
        {
	        _pointMap[key] = value;
        }
        public Point CreateTerminalPoint(int padIndex, SKPoint pt)
        {
	        var ptRef = new Point(padIndex, pt);
	        _pointMap.Add(ptRef.Key, ptRef);
	        return ptRef;
        }
        public void MergePointRefs(List<IPoint> fromList, IPoint to, SKPoint position)
        {
	        to.SKPoint = position;
	        var terminal = TerminalPointAt(to.Key);
	        foreach (var from in fromList)
	        {
		        from.ReplaceWith(terminal);
	        }
        }

        public SKPoint SKPointFor(IPoint point)
        {
	        SKPoint result;
	        switch (point.Kind)
	        {
                case PointKind.Terminal:
	                result = point.SKPoint;
	                break;
                case PointKind.Pointer:
	                result = TerminalPointAt(point.Key).SKPoint;
	                break;
                case PointKind.Virtual:
                default:
	                var p = (VPoint) point;
	                var trait = PadAt(p.PadIndex).EntityAt(p.EntityKey).TraitAt(p.TraitKey);
	                var focal = FocalAt(p.FocalKey);
	                result = trait.PointAlongLine(focal.T);
	                break;
	        }
	        return result;
        }
        #endregion

		#region Focal
	    public IEnumerable<Focal> Focals => _focalMap.Values;
	    public Focal FocalAt(int key)
	    {
		    var success = _focalMap.TryGetValue(key, out Focal result);
		    return success ? result : Focal.Empty;
	    }
	    public Focal TerminalFocalAt(int key)
	    {
		    var success = _focalMap.TryGetValue(key, out Focal result);
		    while (success && result.Kind != PointKind.Terminal)
		    {
			    success = _focalMap.TryGetValue(key, out result);
		    }

		    return success ? result : Focal.Empty;
	    }
	    public void SetFocalAt(int key, Focal value)
	    {
		    _focalMap[key] = value;
	    }
	    public Focal CreateTerminalFocal(int padIndex, Slug slug, float t)
	    {
		    var focal = new Focal(padIndex, t, slug);
		    _focalMap.Add(focal.Key, focal);
		    return focal;
	    }
	    public void MergeFocalRefs(Focal from, Focal to)
	    {
		    var terminal = TerminalFocalAt(to.Key);
		    from.Kind = PointKind.Pointer;
		    from.Key = terminal.Key;
	    }
#endregion

        #region Segments

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

#endregion

	    public void Clear()
        {
        }
	    public void Draw()
        {
            _renderer.Draw();
        }

		#region Mouse and Keyboard

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
            _data.StartHighlight = Point.Empty;
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

#endregion

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
                        else if (_data.HasHighlightLine)
                        {

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
