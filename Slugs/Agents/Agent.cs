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
            AddPad(PadKind.None); // for empty elements
            AddPad(PadKind.Working);
            AddPad(PadKind.Input);
            _data = new UIData(this);

            _renderer = renderer;
            _renderer.Data = _data;
            var (entity, trait) = InputPad.AddEntity(new SKSegment( 200, 100, 400, 300), 1);
            InputPad.AddTrait(entity.Key, new SKSegment(290, 100, 490, 300), 1);

            ClearMouse();
            var t = new RefPoint();
        }
        public Pad AddPad(PadKind padKind)
        {
	        Pads[padKind] = new Pad(padKind, this);
	        return Pads[padKind];
        }

#region MousePosition and Keyboard

        public void ClearMouse()
        {
            _data.Reset();
            WorkingPad.Clear();
        }

        public bool MouseDown(MouseEventArgs e)
        {
	        var curPt = e.Location.ToSKPoint();
            _data.Start(curPt);
            //UpdateHighlight(curPt, _data.Origin);
		    //_data.MousePosition = e.Location.ToSKPoint();
		    //SetHighlight();
		    //_data.DownPoint = _data.SnapPosition;
		    //_data.DragRef.Origin = _data.MousePosition;
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
			   // //_data.DownHighlight = _data.HighlightPoint;
		    //}

		    return true;
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
		    //WorkingPad.Clear();
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
            if (final)
            {
	            if (_data.IsDraggingPoint)
	            {
		            if (_data.HasHighlightPoint)
		            {
			            InputPad.MergePoints(_data.Current.SnapPoint, _data.Highlight.SnapPoint, _data.Highlight.SnapPosition);
		            }
		            else if (_data.IsDraggingPoint && _data.HasHighlightLine)
		            {
			            //var dragPoint = _data.DragRef.PointRefs[0];
		            }
		            result = true;
	            }
                else if (_data.IsDown)
	            {
		            var p0 = _data.Current.SnapPosition;
		            var p1 = _data.Current.SnapPoint.SKPoint;
		            if (p0.DistanceTo(p1) > 10)
		            {
			            var (entity, trait) = InputPad.AddEntity(new SKSegment(p0, p1), _traitIndexCounter++);
			            if (!_data.Drag.SnapPoint.IsEmpty)
			            {
				            InputPad.MergePoints(trait.StartRef, _data.Drag.SnapPoint, _data.Drag.SnapPosition);
			            }

			            if (_data.HasHighlightPoint)
			            {
				            InputPad.MergePoints(trait.EndRef, _data.HighlightPoint);
			            }
			            else if (_data.HasHighlightLine)
			            {
				            var highlightLine = _data.HighlightLine;
				            var (t, pt) = highlightLine.TFromPoint(p1);
				            var focal = InputPad.CreateFocal(t, Slug.Unit);
				            var vp = new VPoint(InputPad.PadKind, highlightLine.EntityKey, highlightLine.Key, focal.Key);
				            InputPad.SetPointAt(trait.EndRef.Key, vp);
			            }
		            }

		            result = true;
	            }
            }

            return result;
        }
    }
}
