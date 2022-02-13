using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.EditCommands
{
    // CreateTerminalPointTask
    // CreateRefPointTask
    // CreateFocalPointTask
    // CreateTraitTask
    // CreateEntityTask
    // MoveElementTask
    // MoveVPointTask
    // StretchSegmentTask
    public class AddTraitCommand : EditCommand, IDraggableCommand
    {
	    public IPointTask StartPointTask { get; private set; }
	    public IPointTask EndPointTask { get; private set; }
	    public bool Locked { get; }

	    public CreateTraitTask TraitTask { get; set; }
        public TraitKind TraitKind { get; private set; }

        public Trait AddedTrait => TraitTask?.Trait ?? Trait.Empty;
	    public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;
	    public bool HasDraggablePoint => !DraggablePoint.IsEmpty;

        public AddTraitCommand(TraitKind traitKind, IPoint startPoint) :
		    this(traitKind, new CreateRefPointTask(startPoint.PadKind, startPoint.Key), 
			    new CreateTerminalPointTask(startPoint.PadKind, startPoint.Position)) { }

        public AddTraitCommand(Pad pad, TraitKind traitKind, SKPoint start, SKPoint end, bool locked = false) :
	        this(traitKind, new CreateTerminalPointTask(pad.PadKind, start), new CreateTerminalPointTask(pad.PadKind, end), locked) { }

        public AddTraitCommand(TraitKind traitKind, IPointTask startPointTask, IPointTask endPointTask, bool locked = false) : base(startPointTask.Pad)
        {
            StartPointTask = startPointTask;
            EndPointTask = endPointTask ?? new CreateTerminalPointTask(Pad.PadKind, Pad.PointAt(startPointTask.PointKey).Position);
            TraitKind = traitKind;
            Locked = locked;
        }

        // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
        // this makes tasks continuous like animation or transform commands.

        public override void Execute()
        {
	        base.Execute();
            AddTaskAndRun(StartPointTask);
	        AddTaskAndRun(EndPointTask);
	        if (TraitTask == null)
	        {
				TraitTask = new CreateTraitTask(Pad.PadKind, TraitKind, StartPointTask.IPoint, EndPointTask.IPoint);
	        }
	        AddTaskAndRun(TraitTask);
	        AddedTrait.IsLocked = Locked;
        }

        public override void Unexecute()
        {
	        base.Unexecute();
        }

        public override void Update(SKPoint point)
        {
        }

        public override void Completed()
        {
        }
    }
}
