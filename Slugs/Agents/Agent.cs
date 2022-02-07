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
using Slugs.UI;

namespace Slugs.Agents
{
	public class Agent : IAgent
    {
	    public static Agent Current { get; private set; }

        public UIData Data;
        private CommandStack<EditCommand> _editCommands;
        private Entity _activeEntity;
        private EditCommand _activeCommand;

        public readonly Dictionary<PadKind, Pad> Pads = new Dictionary<PadKind, Pad>();

	    public Pad WorkingPad => PadFor(PadKind.Working);
        public Pad InputPad => PadFor(PadKind.Input);
        public Pad PadFor(PadKind kind) => Pads[kind];

        private readonly SlugRenderer _renderer;
        public RenderStatus RenderStatus { get; }

        public int ScrollLeft { get; set; }
        public int ScrollRight { get; set; }

        public bool IsDown { get; private set; }
        public bool IsDragging { get; private set; }

        private List<int> _ignoreList = new List<int>();
        private ElementKind _selectableKind = ElementKind.Any;
        public event EventHandler OnModeChange;

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
            var (entity, trait) = InputPad.AddEntity(new SKPoint(100, 100), new SKPoint(700, 100), 1);
            _activeEntity = entity;
            trait.SetLock(true);
            InputPad.AddTrait(entity.Key, new SKPoint(100, 140), new SKPoint(700, 140), 1).SetLock(true);
            var trait2 = InputPad.AddTrait(entity.Key, new SKPoint(100, 180), new SKPoint(700, 180), 1);
            trait2.SetLock(true);
            InputPad.AddTrait(entity.Key, new SKPoint(100, 220), new SKPoint(700, 220), 1).SetLock(true);
            var fc1 = new AddFocalCommand(_activeEntity, trait, 0.3f, 0.8f);
            fc1.Execute();
            var fc2 = new AddFocalCommand(_activeEntity, trait2, 0.1f, 0.5f);
            fc2.Execute();
            var bc = new AddBondCommand(fc1.AddedFocal, .2f, fc2.AddedFocal, 0.3f);
            bc.Execute();
            var bc2 = new AddBondCommand(fc1.AddedFocal, .4f, fc2.AddedFocal, 0.8f);
            bc2.Execute();

            UIMode = UIMode.Any;
            ClearMouse();
            var t = new RefPoint();
        }
        public Pad AddPad(PadKind padKind)
        {
	        Pads[padKind] = new Pad(padKind, this);
	        return Pads[padKind];
        }

#region Position and Keyboard

        public bool MouseDown(MouseEventArgs e)
        {
            // Add to selection if ctrl down etc.

	        var mousePoint = e.Location.ToSKPoint();

	        Data.Begin.Position = mousePoint;
            Data.GetHighlight(mousePoint, Data.Begin, _ignoreList, _selectableKind);
            Data.GetHighlight(mousePoint, Data.Highlight, _ignoreList, _selectableKind);
            Data.Selected.SetPoint(mousePoint, Data.Highlight.Point);
            Data.Selected.SetElements(Data.Begin.Elements);

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
            Data.GetHighlight(mousePoint, Data.Highlight, _ignoreList, _selectableKind);
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
					bool overrideMove = KeyIsDownControl || UIMode.IsCreate();
					var createObject = !Data.Begin.HasSelection || KeyIsDownControl;
					if (createObject)
					{
                        // create Trait if terminal or ref point, create AddedBond or AddedFocal it PointOnTrait.
                        // trait to same trait is focal, trait to other trait is bond, all others are new traits.
                        var pKind = Data.Begin.Point.ElementKind;
                        var eKind = Data.Begin.ElementKind;
                        var makeTrait = eKind.IsNone() || eKind.IsPoint();
                        var makeBond =  eKind == ElementKind.Focal;
                        var makeFocal = eKind == ElementKind.Trait;
                        var point = eKind.IsPoint() ? (IPoint) Data.Begin.FirstElement : Data.Begin.Point;

                        if (eKind.IsNone() || eKind.IsPoint()) // make trait if starting new or connecting to an existing point (maybe not second)
                        {
	                        var traitCmd = Data.Begin.HasPoint ? 
		                        new AddTraitCommand(-1, Data.Begin.Point) :
		                        new AddTraitCommand(Data.Begin.Pad, -1, Data.Begin.Position);
							_activeCommand = _editCommands.Do(traitCmd);
							_ignoreList.Add(traitCmd.AddedTrait.Key);
							_ignoreList.Add(traitCmd.AddedTrait.EndKey);
							Data.Selected.Point = traitCmd.AddedTrait.EndPoint;
                            // need to ignore 
                        }
                        else if (eKind == ElementKind.Trait) // make focal if creating something on a trait
                        {
	                        var trait = (Trait)Data.Begin.FirstElement;
	                        var startT = trait.TFromPoint(mousePoint).Item1;
	                        var focalCmd = new AddFocalCommand(_activeEntity, trait, startT, startT);
	                        _activeCommand = _editCommands.Do(focalCmd);
	                        Data.Selected.Point = focalCmd.AddedFocal.EndPoint;// new TerminalPoint(PadKind.Input, mousePoint);// 
	                        Data.Selected.ClearElements();
	                        _selectableKind = ElementKind.TraitPart;
                        }
                        else if (eKind == ElementKind.Focal) // make focal if creating something on a trait
                        {
	                        var startFocal = (Focal)Data.Begin.FirstElement;
	                        var nearest = _activeEntity.NearestFocal(mousePoint, startFocal.Key);
	                        if (!nearest.IsEmpty)
	                        {
		                        var startT = startFocal.TFromPoint(Data.Begin.Position).Item1;
		                        var endT = nearest.TFromPoint(mousePoint).Item1;
	                            var bondCmd = new AddBondCommand(startFocal, startT, nearest, endT);
		                        _activeCommand = _editCommands.Do(bondCmd);
		                        Data.Selected.Point = bondCmd.AddedBond.EndPoint;// new TerminalPoint(PadKind.Input, mousePoint);// 
		                        Data.Selected.ClearElements();
		                        _selectableKind = ElementKind.FocalPart;
                            }
                        }
                        else
                        {
                        }
					}
					else if(Data.Begin.HasSelection) // drag existing object
					{
						if (Data.Begin.HasPoint)
						{
							MoveElementCommand cmd = new MoveElementCommand(Data.Begin.Pad, Data.Begin.Point.Key);
							Data.Selected.SetElements(Data.Begin.Point);
							_activeCommand = _editCommands.Do(cmd);
							_ignoreList.Add(Data.Begin.Point.Key);
							_selectableKind = Data.Begin.Point.ElementKind.AttachableElements();
						}
						else if(!Data.Begin.FirstElement.IsLocked)
						{
							MoveElementCommand cmd = new MoveElementCommand(Data.Begin.Pad, Data.Begin.FirstElement.Key);
							Data.Selected.SetElements(Data.Begin.FirstElement); // todo: move to keys only, and move sets vs objects
							_activeCommand = _editCommands.Do(cmd);
							_ignoreList.Add(Data.Begin.FirstElement.Key);
							_selectableKind = ElementKind.Any;
						}
					}
					else // rect select
					{

					}
					IsDragging = true;
				}
		    }

		    Data.Selected.UpdatePositions(mousePoint);
		    if (_activeCommand is AddBondCommand abc)
		    {
			    var focal = _activeEntity.NearestFocal(mousePoint, abc.StartPointTask.FocalKey);
			    if (!focal.IsEmpty && focal.Key != abc.EndPointTask.FocalKey)
			    {
					abc.UpdateEndPointFocal(focal);
			    }
		    }
            return true;
	    }

	    public bool MouseUp(MouseEventArgs e)
	    {
            // If dragging or creating, check for last point merge
            // If rect select, add contents to selection (also done in move).
            // If not dragging or creating and dist < selMax, click select
            var mousePoint = e.Location.ToSKPoint();

		    Data.Current.UpdatePositions(mousePoint);
		    Data.GetHighlight(mousePoint, Data.Highlight, _ignoreList, _selectableKind);

            // Merge points if needed.
            if (Data.HasHighlightPoint && _activeCommand is IDraggableCommand cmd && cmd.HasDraggablePoint)
            {
	            if (cmd.DraggablePoint.CanMergeWith(Data.HighlightPoint))
	            {
		            var fromKey = cmd.DraggablePoint.Key;
		            var toKey = Data.Highlight.Point.Key;
		            if (fromKey != ElementBase.EmptyKeyValue && toKey != ElementBase.EmptyKeyValue && fromKey != toKey)
		            {
			            cmd.AddTaskAndRun(new MergePointsTask(cmd.Pad.PadKind, fromKey, toKey)); 
                    }
                }
	            cmd.AddTaskAndRun(new SetSelectionTask(cmd.Pad.PadKind, ElementBase.EmptyKeyValue));
            }
            else if (!IsDragging && _activeCommand == null)  // clicked
            {
	            var selCmd = new SetSelectionCommand(Data.Begin.Pad, Data.Highlight.PointKey, Data.Highlight.ElementKeysCopy);
                selCmd.Execute();
            }

            ClearMouse();

            return true;
	    }

        private Keys CurrentKey;
	    private bool KeyIsDownControl => (CurrentKey & Keys.ControlKey) != 0;
	    private bool KeyIsDownAlt => (CurrentKey & Keys.Alt) != 0;
	    private bool KeyIsDownShift => (CurrentKey & Keys.ShiftKey) != 0;
	    private UIMode _uiMode = UIMode.Any;
	    public UIMode UIMode
	    {
		    get => _uiMode;
		    set
		    {
			    if (value != _uiMode)
			    {
				    _uiMode = value;
					OnModeChange?.Invoke(this, new EventArgs());
                    SetSelectable(UIMode);
			    }
		    }
        }

	    private void SetSelectable(UIMode uiMode)
	    {
		    switch (uiMode)
		    {
			    case UIMode.None:
			    case UIMode.Any:
                    _selectableKind = ElementKind.Any;
				    break;
			    case UIMode.CreateEntity:
				    _selectableKind = ElementKind.Any;
				    break;
			    case UIMode.CreateTrait:
				    _selectableKind = ElementKind.TraitPart;
				    break;
			    case UIMode.CreateFocal:
				    _selectableKind = _isControlDown ? ElementKind.TraitPart : ElementKind.FocalPart;
				    break;
			    case UIMode.CreateBond:
				    _selectableKind = _isControlDown ? ElementKind.FocalPart : ElementKind.BondPart;
				    break;
		    }
	    }

	    private bool _isControlDown;
	    private bool _isShiftDown;
	    private bool _isAltDown;
        // When Bond is selected, focals can be highlighted (but not moved), bonds can be created or edited and have precedence in conflict.
        // ctrl defaults to 'create' causing select to be exclusive to focals or bond points.
        public bool KeyDown(KeyEventArgs e)
	    {
		    CurrentKey = e.KeyCode;
		    _isControlDown = e.Control;
		    _isShiftDown = e.Shift;
		    _isAltDown = e.Alt;
            var curMode = UIMode;
		    switch (CurrentKey)
		    {
			    case Keys.Escape:
				    UIMode = UIMode.Any;
				    break;
                case Keys.E:
				    UIMode = UIMode.CreateEntity;
				    break;
			    case Keys.T:
				    UIMode = UIMode.CreateTrait;
				    break;
			    case Keys.F:
				    UIMode = UIMode.CreateFocal;
                    break;
			    case Keys.B:
				    UIMode = UIMode.CreateBond;
                    break;
            }
		    SetSelectable(UIMode);
            return true;
	    }

        public bool KeyUp(KeyEventArgs e)
        {
            CurrentKey = Keys.None;
            _isControlDown = e.Control;
            _isShiftDown = e.Shift;
            _isAltDown = e.Alt;
            //_selectableKind = ElementKind.Any;
            SetSelectable(UIMode);
            return true;
        }


#endregion

	    public void ClearMouse()
	    {
		    IsDown = false;
		    IsDragging = false;
            _activeCommand = null;
            _ignoreList.Clear();
            Data.Current.Clear();
		    Data.Begin.Clear();
		    WorkingPad.Clear();
		    SetSelectable(UIMode);
        }
	    public void Clear()
        {
        }

    }
}
