using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Entities;
using Slugs.Input;
using Slugs.Pads;
using Slugs.Primitives;
using Slugs.Renderer;

namespace Slugs.Agents
{
	public class Agent : IAgent
    {
	    public static Agent Current { get; private set; }

        // todo: Move all collections to agents, maybe duplicate keys in entities etc if useful, though filters amount to the same.
	    private readonly Dictionary<int, IPoint> _pointMap = new Dictionary<int, IPoint>();
	    private readonly Dictionary<int, Focal> _focalMap = new Dictionary<int, Focal>();

        public readonly Pad WorkingPad;
        public readonly Pad InputPad;
        public Pad PadAt(PadKind kind) => _data.PadFrom(kind);

        private UIData _data;
        private readonly SlugRenderer _renderer;
        public RenderStatus RenderStatus { get; }

        public int ScrollLeft { get; set; }
        public int ScrollRight { get; set; }

        private int _traitIndexCounter = 4096;

        public Agent(SlugRenderer renderer)
        {
            Current = this;
            WorkingPad = new Pad(PadKind.Working, this);
            InputPad = new Pad(PadKind.Input, this);
            _data = new UIData();

            _data.Pads[WorkingPad.PadKind] = WorkingPad;
            _data.Pads[InputPad.PadKind] = InputPad;

            _renderer = renderer;
            _renderer.Data = _data;
            var (entity, trait) = InputPad.AddEntity(new SKSegment( 200, 100, 400, 300), 1);
            InputPad.AddTrait(entity.Key, new SKSegment(290, 100, 490, 300), 1);

            ClearMouse();
        }

#region Points
        //public IEnumerable<IPoint> Points => _pointMap.Values;
        //public IPoint PointAt(int key)
        //{
	       // var success = _pointMap.TryGetValue(key, out IPoint result);
	       // return success ? result : RefPoint.Empty;
        //}
        //public IPoint TerminalPointAt(int key)
        //{
	       // var success = _pointMap.TryGetValue(key, out IPoint result);
	       // while (success && result.Kind == PointKind.Reference)
	       // {
		      //  success = _pointMap.TryGetValue(key, out result);
	       // }
	       // return success ? result : RefPoint.Empty;
        //}
        //public void SetPointAt(int key, IPoint value)
        //{
	       // _pointMap[key] = value;
        //}
        //public RefPoint CreateTerminalPoint(PadKind padKind, SKPoint pt)
        //{
	       // var ptRef = new RefPoint(padKind, pt);
	       // _pointMap.Add(ptRef.Key, ptRef);
	       // return ptRef;
        //}
        //public void MergePoints(IPoint from, IPoint to, SKPoint position)
        //{
	       // to.SKPoint = position;
	       // var terminal = from.Pad.TerminalPointAt(to.Key);
	       // from.ReplaceWith(terminal);
        //}
        public SKPoint SKPointFor(IPoint point)
        {
	        SKPoint result;
	        switch (point.ElementKind)
	        {
                case ElementKind.Terminal:
	                result = point.SKPoint;
	                break;
                case ElementKind.RefPoint:
	                result = point.Pad.TerminalPointAt(point.Key).SKPoint;
	                break;
                case ElementKind.VPoint:
                default:
	                var p = (VPoint) point;
	                var trait = PadAt(p.PadKind).EntityAt(p.EntityKey).TraitAt(p.TraitKey);
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
		    while (success && result.ElementKind != ElementKind.Terminal)
		    {
			    success = _focalMap.TryGetValue(key, out result);
		    }

		    return success ? result : Focal.Empty;
	    }
	    public void SetFocalAt(int key, Focal value)
	    {
		    _focalMap[key] = value;
	    }
	    public Focal CreateTerminalFocal(PadKind padKind, float t, Slug slug)
	    {
		    var focal = new Focal(padKind, t, slug);
		    _focalMap.Add(focal.Key, focal);
		    return focal;
	    }
	    //public void MergeFocalRefs(Focal from, Focal to)
	    //{
		   // var terminal = TerminalFocalAt(to.Key);
		   // from.Kind = PointKind.Reference;
		   // from.Key = terminal.Key;
	    //}
#endregion
#region Segments

        //public SegmentBase CreateTerminalSegRef(PadKind padKind, SKSegment skSegment)
        //{
	       // var a = PadAt(padKind).CreateTerminalPoint(skSegment.StartPoint);
	       // var b = PadAt(padKind).CreateTerminalPoint(skSegment.EndPoint);
	       // return new SegmentBase(padKind, a, b);
        //}
        //public SegmentBase[] CreateTerminalSegRefs(PadKind padKind, params SKSegment[] segs)
        //{
	       // var result = new List<SegmentBase>(segs.Length);
	       // foreach (var skSegment in segs)
	       // {
		      //  result.Add(CreateTerminalSegRef(padKind, skSegment));
	       // }
	       // return result.ToArray();
        //}

#endregion
#region Mouse and Keyboard

        public void ClearMouse()
        {
            _data.Reset();
            //_data.DownPoint = SKPoint.Empty;
            //_data.OriginPoint = SKPoint.Empty;
            //_data.OriginSnap = SKPoint.Empty;
            //_data.DownPoint = SKPoint.Empty;
            //_data.StartHighlight = RefPoint.Empty;

            _data.DragSegment.Clear(); 
            WorkingPad.Clear();
            //_data.DragRef.Clear();

        }

        public bool MouseDown(MouseEventArgs e)
        {
	        var curPt = e.Location.ToSKPoint();
            _data.Start(curPt);
            //UpdateHighlight(curPt, _data.Origin);
		    //_data.OriginPoint = e.Location.ToSKPoint();
		    //SetHighlight();
		    //_data.DownPoint = _data.OriginSnap;
		    //_data.DragRef.Origin = _data.OriginPoint;
		    if (_data.HasHighlightPoint && CurrentKey != Keys.ControlKey)
		    {
                //_data.DragRef.Add(_data.HighlightPoints);
                //_data.IsDraggingElement = true;
            }
		    else if (!_data.HighlightLine.IsEmpty && CurrentKey != Keys.ControlKey)
		    {
			    //_data.DragRef.Add(_data.HighlightLine.StartRef, _data.HighlightLine.EndRef, true);
			    //_data.IsDraggingElement = true;
            }
		    else
		    {
			    //_data.DragSegment.Add(_data.OriginSnap);
			    //_data.StartHighlight = _data.HighlightPoint;
		    }

		    return true;
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
		    WorkingPad.Clear();
		    _data.Move(e.Location.ToSKPoint());
		    SetCreating();

		    return true;
	    }

	    public bool MouseUp(MouseEventArgs e)
	    {
		    _data.End(e.Location.ToSKPoint());
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

	    public void Clear()
        {
        }

        private bool SetCreating(bool final = false)
        {
            var result = false;
            if (_data.IsDragging)
            {
                if (final)
                {
                    //_data.DragRef.OffsetValues(_data.OriginSnap);
                    if (_data.IsDraggingPoint && _data.HasHighlightPoint)
                    {
                        PadAt(PadKind.Input).MergePoints(_data.Origin.Snap, _data.HighlightPoint, _data.SnapPoint);
                        //MergePoints(_data.Origin.Snap, _data.HighlightPoint, _data.SnapPoint);
                    }
                    else if (_data.IsDraggingPoint && _data.HasHighlightLine)
                    {
                        //var dragPoint = _data.DragRef.PointRefs[0];
                    }
                    result = true;
                }
            }
            else if (_data.IsDown)
            {
                WorkingPad.AddEntity(new SKSegment(_data.DownPoint, _data.SnapPoint), 0);
                if (final)
                {
                    _data.DragSegment.Add(_data.SnapPoint);
                    if (_data.DragSegment[0].DistanceTo(_data.DragSegment[1]) > 10)
                    {
	                    var (entity, trait) = InputPad.AddEntity(new SKSegment(_data.DownPoint,  _data.DragSegment[1]), _traitIndexCounter++);
                        //var newDataMap = DataMap.CreateIn(InputPad, _data.DragSegment);
                        if (!_data.StartHighlight.IsEmpty)
                        {
	                        PadAt(PadKind.Input).MergePoints(trait.StartRef, _data.StartHighlight, _data.StartHighlight.SKPoint);
                        }
                        if (_data.HasHighlightPoint)
                        {
	                        PadAt(PadKind.Input).MergePoints(trait.EndRef, _data.HighlightPoint, _data.SnapPoint);
                        }
                        else if (_data.HasHighlightLine)
                        {
	                        var highlightLine = _data.HighlightLine;
	                        var (t, pt) = highlightLine.TFromPoint(_data.DragSegment[1]);
                            var focal = CreateTerminalFocal(InputPad.PadKind, t, Slug.Unit);
                            var vp = new VPoint(InputPad.PadKind, highlightLine.EntityKey, highlightLine.Key, focal.Key);
                            _pointMap.Add(vp.Key, vp);
                            PadAt(PadKind.Input).SetPointAt(trait.EndRef.Key, vp);
                            //trait.EndRef.ReplaceWith(vp);
                        }
                    }
                }
                result = true;
            }
            return result;
        }
    }
}
