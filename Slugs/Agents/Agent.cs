using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Commands;
using Slugs.Commands.EditCommands;
using Slugs.Commands.Tasks;
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

        public UIData Data;
        private CommandStack<EditCommand> _editCommands;
        private EditCommand _activeCommand;

	    public readonly Dictionary<PadKind, Pad> Pads = new Dictionary<PadKind, Pad>();

	    public Pad WorkingPad => PadAt(PadKind.Working);
        public Pad InputPad => PadAt(PadKind.Input);
        public Pad PadAt(PadKind kind) => Pads[kind];

        private readonly SlugRenderer _renderer;
        public RenderStatus RenderStatus { get; }

        public int ScrollLeft { get; set; }
        public int ScrollRight { get; set; }

        private int _traitIndexCounter = 4096;
        public bool IsDown { get; private set; }
        public bool IsDragging { get; private set; }

        public Agent(SlugRenderer renderer)
        {
            Current = this;
            AddPad(PadKind.None); // for empty elements
            AddPad(PadKind.Working);
            AddPad(PadKind.Input);
            Data = new UIData(this);
            _editCommands = new CommandStack<EditCommand>(this);

            _renderer = renderer;
            _renderer.Data = Data;
            var (entity, trait) = InputPad.AddEntity(new SKPoint(200, 100), new SKPoint(400, 300), 1);
            InputPad.AddTrait(entity.Key, new SKPoint(290, 100), new SKPoint( 490, 300), 1);

            ClearMouse();
            var t = new RefPoint();
        }
        public Pad AddPad(PadKind padKind)
        {
	        Pads[padKind] = new Pad(padKind, this);
	        return Pads[padKind];
        }

#region Position and Keyboard

        public void ClearMouse()
        {
            Data.Reset();
            WorkingPad.Clear();
        }
        
        public bool MouseDown(MouseEventArgs e)
        {
            // Add to selection if ctrl down etc.

	        var mousePoint = e.Location.ToSKPoint();

            Data.GetHighlight(mousePoint, Data.Begin, null);
            Data.GetHighlight(mousePoint, Data.Highlight, null);
            Data.Selected.Set(mousePoint, Data.Highlight.Point, Data.Begin.Element);

            IsDown = true;
            return true;
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
            // if > that min length, either create element, or drag existing, based on highlight - p0 based on highlight, keys and selection
            // Create:
            //      - creating tool, p0 is either terminal (no highlight), refPoint if highlight point, or vPoint if line selected (also check key mods)
            //      - if highlighting point or element, start dragging it
            //      - if no highlight, start selection rect
            // Drag Element:
            //      - may be rect select if mode isn't create, p0 is start point
            // Rect Select:
            //      - live reset contents to selection by bounds (also done in up).
            // Suppress Select Element:
            //      - cancels click select if length > select max

            // AddTrait, AddBond, AddFocal, MoveElement, SelectRect 
            var result = false;
		    var mousePoint = e.Location.ToSKPoint();
		    Data.Current.UpdatePositions(mousePoint);
            Data.GetHighlight(mousePoint, Data.Highlight, Data.Selected);
            if (IsDown)
            {
                result = MouseDrag(mousePoint);
            }
            return true;
	    }

	    private float _minDragDistance = 10f;
	    public bool MouseDrag(SKPoint mousePoint)
	    {
		    if (!IsDragging)
		    {
				var dist = (mousePoint - Data.Begin.Position).Length;
				if (dist > _minDragDistance)
				{
					var createObject = !Data.Begin.HasSelection || KeyIsDownControl;
					if (createObject)
					{
                        // create Trait if terminal or ref point, create Bond or Focal it VPoint.
	                    var cmd = Data.Begin.Point.IsEmpty ? 
							new AddTraitCommand(Data.Begin.Pad, -1, Data.Begin.Position) :
							new AddTraitCommand(-1, Data.Begin.Point);
						_activeCommand = _editCommands.Do(cmd);
						Data.Selected.Point = cmd.AddedTrait.EndRef;
					}
					else if(Data.Begin.HasSelection) // drag existing object
					{
						IElement sel = !Data.Begin.Point.IsEmpty ? Data.Begin.Point : Data.Begin.Element;
                        var cmd = new MoveElementCommand(Data.Begin.Pad, sel.Key);
                        _activeCommand = _editCommands.Do(cmd);
                        Data.Selected.Element = sel;
					}
					else // rect select
					{

					}
					IsDragging = true;
				}
		    }
		    Data.Selected.UpdatePositions(mousePoint);
            return true;
	    }

	    public bool MouseUp(MouseEventArgs e)
	    {
            // If dragging or creating, check for last point merge
            // If rect select, add contents to selection (also done in move).
            // If not dragging or creating and dist < selMax, click select

            // MergeEndPoint, SetSelection (Click or Rect)
		    WorkingPad.Clear();
            var mousePoint = e.Location.ToSKPoint();

		    Data.Current.UpdatePositions(mousePoint);
		    Data.GetHighlight(mousePoint, Data.Highlight, Data.Selected);
            FinalizeCommand(true);
            if (!Data.Highlight.Point.IsEmpty)
            {
	            if (_activeCommand is AddTraitCommand atc)
	            {
                    atc.AddTaskAndRun(new MergePointsTask(atc.Pad.PadKind, atc.EndPointTask.PointKey, Data.Highlight.Point.Key));
	            }
            }

            _activeCommand = null;
            Data.Current.Clear();
		    Data.Begin.Clear();
		    Data.Selected.Clear();

            ClearMouse();
		    IsDown = false;
		    IsDragging = false;
            return true;
	    }


	    private Keys CurrentKey;
	    private bool KeyIsDownControl => (CurrentKey & Keys.ControlKey) != 0;
	    private bool KeyIsDownAlt => (CurrentKey & Keys.Alt) != 0;
	    private bool KeyIsDownShift => (CurrentKey & Keys.ShiftKey) != 0;
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

        private bool FinalizeCommand(bool final = false)
        {
            var result = false;
            // merge points if dragging any kind of point onto a valid existing point.
            if (Data.HasHighlightPoint && _activeCommand is IDraggablePointCommand cmd)
            {
	            cmd.AddTaskAndRun(new MergePointsTask(cmd.Pad.PadKind, cmd.DraggablePoint.Key, Data.Highlight.Point.Key));
                //cmd.Pad.MergePoints(cmd.DraggablePoint, Data.Highlight.Point, Data.Highlight.SnapPosition);
	            result = true;
            }

            return result;
        }
    }
}
