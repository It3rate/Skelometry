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

        public UIData Data;
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
            Data = new UIData(this);

            _renderer = renderer;
            _renderer.Data = Data;
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
            Data.Reset();
            WorkingPad.Clear();
        }

        public bool MouseDown(MouseEventArgs e)
        {
	        var curPt = e.Location.ToSKPoint();
            Data.Start(curPt);
            //UpdateHighlight(curPt, Data.Origin);
		    //Data.MousePosition = e.Location.ToSKPoint();
		    //SetHighlight();
		    //Data.DownPoint = Data.SnapPosition;
		    //Data.DragRef.Origin = Data.MousePosition;
		    //if (Data.HasHighlightPoint && CurrentKey != Keys.ControlKey)
		    //{
      //          //Data.DragRef.Add(Data.HighlightPoints);
      //          //Data.IsDraggingElement = true;
      //      }
		    //else if (!Data.HighlightLine.IsEmpty && CurrentKey != Keys.ControlKey)
		    //{
			   // //Data.DragRef.Add(Data.HighlightLine.StartRef, Data.HighlightLine.EndRef, true);
			   // //Data.IsDraggingElement = true;
      //      }
		    //else
		    //{
			   // //Data.DragSegment.Add(Data.SnapPosition);
			   // //Data.DownHighlight = Data.HighlightPoint;
		    //}

		    return true;
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
		    //WorkingPad.Clear();
		    Data.Move(e.Location.ToSKPoint());
		    SetCreating();

		    return true;
	    }

	    public bool MouseUp(MouseEventArgs e)
	    {
		    WorkingPad.Clear();
            Data.Move(e.Location.ToSKPoint());
            SetCreating(true);
		    Data.End(e.Location.ToSKPoint());

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
	            if (Data.IsDraggingPoint)
	            {
		            if (Data.HasHighlightPoint)
		            {
			            InputPad.MergePoints(Data.Current.SnapPoint, Data.Highlight.SnapPoint, Data.Highlight.SnapPosition);
		            }
		            else if (Data.IsDraggingPoint && Data.HasHighlightLine)
		            {
			            //var dragPoint = Data.DragRef.PointRefs[0];
		            }
		            result = true;
	            }
                else if (Data.IsDown)
	            {
		            var p0 = Data.Current.SnapPosition;
		            var p1 = Data.Current.SnapPoint.SKPoint;
		            if (p0.DistanceTo(p1) > 10)
		            {
			            var (entity, trait) = InputPad.AddEntity(new SKSegment(p0, p1), _traitIndexCounter++);
			            if (!Data.Begin.SnapPoint.IsEmpty)
			            {
				            InputPad.MergePoints(trait.StartRef, Data.Begin.SnapPoint, Data.Begin.SnapPosition);
			            }

			            if (Data.HasHighlightPoint)
			            {
				            InputPad.MergePoints(trait.EndRef, Data.HighlightPoint);
			            }
			            else if (Data.HasHighlightLine)
			            {
				            var highlightLine = Data.HighlightLine;
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
