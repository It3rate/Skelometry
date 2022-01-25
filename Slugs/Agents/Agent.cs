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

	    public readonly Dictionary<PadKind, Pad> Pads = new Dictionary<PadKind, Pad>();

        // todo: Move all collections to agents, maybe duplicate keys in entities etc if useful, though filters amount to the same.
        //private readonly Dictionary<int, IPoint> _pointMap = new Dictionary<int, IPoint>();
        private readonly Dictionary<int, Focal> _focalMap = new Dictionary<int, Focal>();

	    public Pad WorkingPad => PadAt(PadKind.Working);
        public Pad InputPad => PadAt(PadKind.Input);
        public Pad PadAt(PadKind kind) => Pads[kind];

        private UIData _data;
        private readonly SlugRenderer _renderer;
        public RenderStatus RenderStatus { get; }

        public int ScrollLeft { get; set; }
        public int ScrollRight { get; set; }

        private int _traitIndexCounter = 4096;

        public Agent(SlugRenderer renderer)
        {
            Current = this;
            AddPad(PadKind.Working);
            AddPad(PadKind.Input);
            _data = new UIData(this);

            _renderer = renderer;
            _renderer.Data = _data;
            var (entity, trait) = InputPad.AddEntity(new SKSegment( 200, 100, 400, 300), 1);
            InputPad.AddTrait(entity.Key, new SKSegment(290, 100, 490, 300), 1);

            ClearMouse();
        }
        public Pad AddPad(PadKind padKind)
        {
	        Pads[padKind] = new Pad(padKind, this);
	        return Pads[padKind];
        }

#region Mouse and Keyboard

        public void ClearMouse()
        {
            _data.Reset();
            //_data.DownPoint = SKPoint.Empty;
            //_data.OriginPosition = SKPoint.Empty;
            //_data.SnapPosition = SKPoint.Empty;
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
		    //_data.OriginPosition = e.Location.ToSKPoint();
		    //SetHighlight();
		    //_data.DownPoint = _data.SnapPosition;
		    //_data.DragRef.Origin = _data.OriginPosition;
		    //if (_data.HasHighlightPoint && CurrentKey != Keys.ControlKey)
		    //{
      //          //_data.DragRef.Add(_data.HighlightPoints);
      //          //_data.IsDraggingElement = true;
      //      }
		    //else if (!_data.HighlightLine.IsEmpty && CurrentKey != Keys.ControlKey)
		    //{
			   // //_data.DragRef.Add(_data.HighlightLine.StartRef, _data.HighlightLine.EndRef, true);
			   // //_data.IsDraggingElement = true;
      //      }
		    //else
		    //{
			   // //_data.DragSegment.Add(_data.SnapPosition);
			   // //_data.StartHighlight = _data.HighlightPoint;
		    //}

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
		    WorkingPad.Clear();
            _data.Move(e.Location.ToSKPoint());
            SetCreating(true);
		    _data.End(e.Location.ToSKPoint());

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
                    //_data.DragRef.OffsetValues(_data.SnapPosition);
                    if (_data.IsDraggingPoint && _data.HasHighlightPoint)
                    {
                        PadAt(PadKind.Input).MergePoints(_data.Current.OriginIPoint, _data.Highlight.OriginIPoint, _data.SnapPoint);
                        //MergePoints(_data.Origin.Selection, _data.HighlightPoint, _data.OriginIPoint);
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
                            var focal = InputPad.CreateFocal(t, Slug.Unit);
                            var vp = new VPoint(InputPad.PadKind, highlightLine.EntityKey, highlightLine.Key, focal.Key);
                            //InputPad.AddElement(vp);
                            //_pointMap.Add(vp.Key, vp);
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
