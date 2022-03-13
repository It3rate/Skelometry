using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Commands;
using Slugs.Commands.EditCommands;
using Slugs.Commands.Tasks;
using Slugs.Constraints;
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

        private readonly RendererBase _renderer;
        public RenderStatus RenderStatus { get; }

        public int ScrollLeft { get; set; }
        public int ScrollRight { get; set; }

        public bool IsDown { get; private set; }
        public bool IsDragging { get; private set; }

        private List<int> _ignoreList = new List<int>();
        private ElementKind _selectableKind = ElementKind.Any;
        public event EventHandler OnModeChange;
        public event EventHandler OnDisplayModeChange;
        public event EventHandler OnSelectionChange;

        public Agent(RendererBase renderer)
        {
            Current = this;
            AddPad(PadKind.None); // for empty elements
            AddPad(PadKind.Working);
            AddPad(PadKind.Input);
            Data = new UIData(this);
            _editCommands = new CommandStack<EditCommand>(this);

            _renderer = renderer;
            _renderer.Data = Data;

            var entityCmd = new CreateEntityCommand(InputPad);
            _editCommands.Do(entityCmd);
            _activeEntity = entityCmd.Entity;

            MakeXY();
            //MakeLines();

            UIMode = UIMode.Any;
            ClearMouse();
        }

        private void MakeXY()
        {
            var center = new SKPoint(500, 400);
            var xAxis = new SKPoint(400, 0);
            var yAxis = new SKPoint(0, 400);
            var traitX = new AddTraitCommand(InputPad, TraitKind.XAxis, center - xAxis, center + xAxis);
            var traitY = new AddTraitCommand(InputPad, TraitKind.YAxis, center + yAxis, center - yAxis);
            _editCommands.Do(traitX, traitY);
            //cmd = new AddConstraintCommand(InputPad, new MidpointConstraint(traitX.AddedTrait, traitY.AddedTrait));
            //_editCommands.Do(cmd);
            traitX.AddedTrait.IsLocked = true;
            traitY.AddedTrait.IsLocked = true;

            var fc1 = new AddFocalCommand(_activeEntity, traitX.AddedTrait, 0.5f, 0.9f);
            var fc2 = new AddFocalCommand(_activeEntity, traitY.AddedTrait, 0.5f, 0.9f);
            var fc3 = new AddFocalCommand(_activeEntity, traitX.AddedTrait, 0.5f, 0.1f);
            _editCommands.Do(fc1, fc2, fc3);
            fc1.AddedFocal.IsLocked = true;
            fc2.AddedFocal.IsLocked = true;
            fc3.AddedFocal.IsLocked = true;
            InputPad.SetUnit(fc1.AddedFocal);
            InputPad.SetUnit(fc2.AddedFocal);

            var bc1 = new AddSingleBondCommand(fc1.AddedFocal, 0.3f, fc2.AddedFocal, 1f);
            var bc2 = new AddSingleBondCommand(fc2.AddedFocal, 0.3f, fc3.AddedFocal, 1f);
            _editCommands.Do(bc1, bc2);
            bc1.EndPointTask.IPoint.IsLocked = true;
            bc2.EndPointTask.IPoint.IsLocked = true;

            var constraint = new RatioConstraint(
	            bc1.StartPointTask.BondPoint, bc2.StartPointTask.BondPoint,
	            ConstraintTarget.T, new Slug(-0.5, 0.5));
            var rc = new AddConstraintCommand(InputPad, constraint);
            _editCommands.Do(rc);

            var t0 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(50, 50), new SKPoint(80, 250));
            var t1 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(90, 150), new SKPoint(140, 200));
            var t2 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(120, 10), new SKPoint(100, 220));
            var t3 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(110, 40), new SKPoint(130, 140));
            var t4 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(220, 80), new SKPoint(190, 170));
            var t5 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(260, 80), new SKPoint(290, 150));
            var t6 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(270, 80), new SKPoint(300, 300));
            var t7 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(270, 30), new SKPoint(330, 140));
            var t8 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(360, 30), new SKPoint(370, 140));

            var t10 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(10, 10), new SKPoint(80, 10));
            var t11 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(20, 20), new SKPoint(190, 30));
            _editCommands.Do(t0, t1, t2, t3, t4, t5, t6, t7, t8, t10, t11);

            var dupCmd = new DuplicateElementCommand(InputPad, t8.AddedTrait);
            _editCommands.Do(dupCmd);

            //t4.AddedTrait.StartFocalPoint.IsLocked = true;
            var collCommand = new AddConstraintCommand(InputPad, new CollinearConstraint(t0.AddedTrait, t1.StartPointTask.IPoint));
            var collCommand2 = new AddConstraintCommand(InputPad, new CollinearConstraint(t10.AddedTrait, t11.AddedTrait));
            var coinCommand = new AddConstraintCommand(InputPad, new CoincidentConstraint(t1.EndPointTask.IPoint, t2.StartPointTask.IPoint));
            var midCommand = new AddConstraintCommand(InputPad, new MidpointConstraint(t2.AddedTrait, t3.EndPointTask.IPoint));
            var parCommand = new AddConstraintCommand(InputPad, new ParallelConstraint(t3.AddedTrait, t4.AddedTrait, LengthLock.Ratio));
            var eqCommand = new AddConstraintCommand(InputPad, 
	            new EqualConstraint(t5.AddedTrait, t6.AddedTrait, LengthLock.None, DirectionLock.Perpendicular));
            var hCommand = new AddConstraintCommand(InputPad, new HorizontalVerticalConstraint(t7.AddedTrait, true));
            var vCommand = new AddConstraintCommand(InputPad, new HorizontalVerticalConstraint(t8.AddedTrait, false));

            _editCommands.Do(collCommand, collCommand2, coinCommand, midCommand, parCommand, eqCommand, hCommand, vCommand);

            var remCommand = new RemoveElementCommand(InputPad, t3.AddedTrait);
            _editCommands.Do(remCommand);
        }

        private void MakeLines()
        {
	        var traitCmd1 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(100, 100), new SKPoint(700, 100), true);
	        var traitCmd2 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(100, 140), new SKPoint(700, 140), true);
	        var traitCmd3 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(100, 180), new SKPoint(700, 180), true);
	        var traitCmd4 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(100, 220), new SKPoint(700, 220), true);
	        var traitCmd5 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(100, 260), new SKPoint(700, 260), true);
	        var traitCmd6 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(100, 300), new SKPoint(700, 300), true);
	        var traitCmd7 = new AddTraitCommand(InputPad, TraitKind.Default, new SKPoint(100, 340), new SKPoint(700, 340), true);
	        _editCommands.Do(traitCmd1, traitCmd2, traitCmd3, traitCmd4, traitCmd5, traitCmd6, traitCmd7);
	        var trait1 = traitCmd1.AddedTrait;
	        var trait2 = traitCmd2.AddedTrait;
	        var trait3 = traitCmd3.AddedTrait;
	        var fc1 = new AddFocalCommand(_activeEntity, trait2, 0.1f, 0.3f);
	        var fc2 = new AddFocalCommand(_activeEntity, trait3, 0.0f, 0.4f);
	        var fc3 = new AddFocalCommand(_activeEntity, trait1, 0.0f, 0.2f);
	        _editCommands.Do(fc1, fc2, fc3);
	        InputPad.SetUnit(fc3.AddedFocal);

	        var bc = new AddSingleBondCommand(fc1.AddedFocal, -0.3f, fc2.AddedFocal, 0.3f);
	        var db = new AddDoubleBondCommand(fc1.AddedFocal, fc2.AddedFocal);
	        _editCommands.Do(bc, db);
        }

        public Pad AddPad(PadKind padKind)
        {
	        Pads[padKind] = new Pad(padKind);
	        return Pads[padKind];
        }

#region Position and Keyboard

	    private bool _creatingOnDown = false;
        public bool MouseDown(MouseEventArgs e)
        {
            // Add to selection if ctrl down etc.

	        var mousePoint = e.Location.ToSKPoint();

            Data.GetHighlight(mousePoint, Data.Begin, _ignoreList, false, _selectableKind);
	        Data.Begin.Position = mousePoint; // gethighlight clears position so this must be second.

            Data.GetHighlight(mousePoint, Data.Highlight, _ignoreList, false, _selectableKind);
            Data.GetHighlight(mousePoint, Data.Selected, _ignoreList, false, _selectableKind);
            Data.Selected.Position = mousePoint;
            if (UIMode == UIMode.SetUnit && Data.Selected.FirstElement is Focal focal)
            {
                InputPad.SetUnit(focal);
            }

            IsDown = true;
            _creatingOnDown = _isControlDown;
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

            // CreateTrait, AddBond, AddFocal, MoveElement, SelectRect 
            var result = false;
		    var mousePoint = e.Location.ToSKPoint();
		    Data.Current.UpdatePositions(mousePoint);
            Data.GetHighlight(mousePoint, Data.Highlight, _ignoreList, false, _selectableKind);
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
					bool overrideMove = _isControlDown || UIMode.IsCreate();

					bool createPointObject = !Data.Begin.HasSelection || _isControlDown;
					bool createDoubleBond = (UIMode == UIMode.CreateDoubleBond || _isControlDown) && 
					                        Data.Begin.FirstElement.ElementKind == ElementKind.Focal;

                    if (createDoubleBond)
					{
                        // create doubleBond object
						var startFocal = (Focal)Data.Begin.FirstElement;
						var dbc = new AddDoubleBondCommand(startFocal, startFocal);
						_activeCommand = _editCommands.Do(dbc);
						Data.Selected.Clear();
						Data.Selected.SetElements();
                    }
                    else if (createPointObject) // create point to point object
					{
                        // create Trait if terminal or ref point, create AddedSingleBond or AddedFocal it FocalPoint.
                        // trait to same trait is focal, trait to other trait is singleBond, all others are new traits.
                        var pKind = Data.Begin.Point.ElementKind;
                        var eKind = Data.Begin.ElementKind;
                        var makeTrait = eKind.IsNone() || eKind.IsPoint();
                        var makeBond =  eKind == ElementKind.Focal;
                        var makeFocal = eKind == ElementKind.Trait;
                        var point = eKind.IsPoint() ? (IPoint) Data.Begin.FirstElement : Data.Begin.Point;
                        var pointTrait = InputPad.TraitWithPoint(point);

                        if (eKind == ElementKind.Trait || !pointTrait.IsEmpty) // make focal if creating something on a trait
                        {
	                        var trait = (eKind == ElementKind.Trait) ? (Trait)Data.Begin.FirstElement : pointTrait;
	                        var startT = trait.TFromPoint(Data.Begin.Position).Item1;
	                        var focalCmd = new AddFocalCommand(_activeEntity, trait, startT, startT);
	                        _activeCommand = _editCommands.Do(focalCmd);
	                        Data.Selected.Point = focalCmd.AddedFocal.EndFocalPoint;// new TerminalPoint(PadKind.Input, mousePoint);// 
	                        Data.Selected.ClearElements();
	                        _selectableKind = ElementKind.TraitPart;
                        }
                        else if (eKind.IsNone() || eKind.IsPoint()) // make trait if starting new or connecting to an existing point (maybe not second)
                        {
	                        var traitCmd = Data.Begin.HasPoint ? 
		                        new AddTraitCommand(TraitKind.Default, Data.Begin.Point) :
		                        new AddTraitCommand(Data.Begin.Pad, TraitKind.Default, Data.Begin.Position, Data.Current.Position);
							_activeCommand = _editCommands.Do(traitCmd);
							_ignoreList.Add(traitCmd.AddedTrait.Key);
							_ignoreList.Add(traitCmd.AddedTrait.EndKey);
							Data.Selected.Point = traitCmd.AddedTrait.EndPoint;
                            // need to ignore 
                        }
                        else if (eKind == ElementKind.Focal) // make focal if creating something on a trait
                        {
	                        var startFocal = (Focal)Data.Begin.FirstElement;
	                        var nearest = _activeEntity.NearestFocal(mousePoint, startFocal.Key);
	                        if (!nearest.IsEmpty)
	                        {
		                        var startT = startFocal.TFromPoint(Data.Begin.Position).Item1;
		                        var endT = nearest.TFromPoint(mousePoint).Item1;
	                            var bondCmd = new AddSingleBondCommand(startFocal, startT, nearest, endT);
		                        _activeCommand = _editCommands.Do(bondCmd);
		                        Data.Selected.Point = bondCmd.AddedSingleBond.EndPoint;// new TerminalPoint(PadKind.Input, mousePoint);// 
		                        Data.Selected.ClearElements();
		                        _selectableKind = ElementKind.FocalPart;
                            }
                        }
					}
					else if(Data.Begin.HasSelection) // drag existing object
					{
						if (Data.Begin.HasPoint)
						{
							MoveElementCommand cmd = new MoveElementCommand(Data.Begin.Pad, Data.Begin.Point, new SKPoint(0, 0)); 
							//Data.Selected.SetElements(Data.Begin.Point);
                            Data.Selected.SetPoint(Data.Begin.Position, Data.Begin.Point);
							_activeCommand = _editCommands.Do(cmd);
							_ignoreList.Add(Data.Begin.Point.Key);
							_selectableKind = Data.Begin.Point.ElementKind.AttachableElements();
						}
						else if(!Data.Begin.FirstElement.IsLocked)
						{
							MoveElementCommand cmd = new MoveElementCommand(Data.Begin.Pad, Data.Begin.FirstElement, new SKPoint(0, 0));
							Data.Selected.SetElements(Data.Begin.FirstElement);
							_activeCommand = _editCommands.Do(cmd);
							_ignoreList.Add(Data.Begin.FirstElement.Key);
							_selectableKind = ElementKind.None;
						}

					}
					
					IsDragging = true;
				}
		    }

		    var dontDragAtStart = !IsDragging && _creatingOnDown;
		    if (!dontDragAtStart)
		    {
			    Data.Selected.UpdatePositions(mousePoint);
		    }
		    
		    if (_activeCommand is AddSingleBondCommand abc)
		    {
			    var focal = _activeEntity.NearestFocal(mousePoint, abc.StartPointTask.FocalKey);
			    if (!focal.IsEmpty && focal.Key != abc.EndPointTask.FocalKey)
			    {
				    abc.UpdateEndPointFocal(focal);
			    }
		    }
		    else if (_activeCommand is AddDoubleBondCommand adb)
		    {
			    var focal = _activeEntity.NearestFocal(mousePoint, adb.StartFocal.Key);
			    if (!focal.IsEmpty && focal.Key != adb.EndFocal.Key)
			    {
				    adb.UpdateEndFocal(focal);
			    }
			    Data.SetWorkingPoints(adb.StartFocal.StartPosition, adb.StartFocal.EndPosition, adb.EndFocal.EndPosition, adb.EndFocal.StartPosition);
		    }
		    else if (_activeCommand is MoveElementCommand mec)
		    {
			    if (mec.Element is FocalPoint focalPoint)
			    {
				    var changed = InputPad.DoubleBondsWithPoint(focalPoint);
                    var appliedBondKeys = new HashSet<int>();
				    foreach (var (db, focal) in changed)
				    {
					    db.ApplyRatioRecursively(focal.Key, appliedBondKeys);
				    }
			    }

			    var adjustedElements = new Dictionary<int, SKPoint>();
			    InputPad.UpdateConstraints(mec.Element, adjustedElements);
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
		    Data.GetHighlight(mousePoint, Data.Highlight, _ignoreList, false, _selectableKind);

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
	            cmd.AddTaskAndRun(new SetSelectionTask(Data.Selected, ElementBase.EmptyKeyValue));
            }
            else if (!IsDragging && _activeCommand == null)  // clicked
            {
	            var selCmd = new SetSelectionCommand(Data.Selected, Data.Highlight.PointKey, Data.Highlight.ElementKeysCopy);
                selCmd.Execute();
            }

		    OnSelectionChange?.Invoke(this, new EventArgs());
            ClearMouse();

            return true;
	    }

        private Keys CurrentKey;
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
			    case UIMode.SetUnit:
				    _selectableKind = ElementKind.Focal;
				    break;
			    case UIMode.Equal:
				    _selectableKind = ElementKind.Focal;
                    Data.Selected.Clear();
				    break;
            }
	    }

	    private bool _isControlDown;
	    private bool _isShiftDown;
	    private bool _isAltDown;
	    private UIMode PreviousMode = UIMode.Any;
        // When SingleBond is selected, focals can be highlighted (but not moved), bonds can be created or edited and have precedence in conflict.
        // ctrl defaults to 'create' causing select to be exclusive to focals or singleBond points.
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
			    case Keys.D:
				    UIMode = UIMode.CreateDoubleBond;
				    break;
			    case Keys.B:
				    UIMode = UIMode.CreateBond;
				    break;
			    case Keys.U:
				    UIMode = UIMode.SetUnit;
				    break;
			    case Keys.Oemplus:
				    UIMode = UIMode.Equal;
				    break;
			    case Keys.I:
				    ToggleShowNumbers();
				    break;
                case Keys.Z:
				    if (_isShiftDown && _isControlDown)
				    {
					    _editCommands.Redo();
				    }
				    else if (_isControlDown)
				    {
					    _editCommands.Undo();
				    }
                    break;
			    case Keys.R:
				    if (_isControlDown)
				    {
					    // redo
					    _editCommands.Redo();
                    }
				    else
				    {
					    _editCommands.Repeat();
				    }
				    break;
            }
		    SetSelectable(UIMode);
		    if (curMode != UIMode)
		    {
			    PreviousMode = curMode;
		    }
            return true;
	    }

        public bool KeyUp(KeyEventArgs e)
        {
            CurrentKey = Keys.None;
            _isControlDown = e.Control;
            _isShiftDown = e.Shift;
            _isAltDown = e.Alt;
            //_selectableKind = ElementKind.Any;
            if (UIMode.IsMomentary())
            {
	            UIMode = PreviousMode;
            }
            SetSelectable(UIMode);
            return true;
        }


        #endregion

	    public DisplayMode DisplayMode
        {
		    get => Data.DisplayMode;
		    set
		    {
			    Data.DisplayMode = value;
			    OnDisplayModeChange?.Invoke(this, new EventArgs());
		    }
	    }

	    public void ToggleShowNumbers()
	    {
	        if (DisplayMode.HasFlag(DisplayMode.ShowLengths))
	        {
		        DisplayMode &= ~(DisplayMode.ShowAllValues);
	        }
	        else
	        {
		        DisplayMode |= DisplayMode.ShowAllValues;
	        }
	    }
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
            Data.SetWorkingPoints();
        }
	    public void Clear()
        {
        }

    }
}
