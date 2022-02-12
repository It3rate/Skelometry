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
	    public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;
	    public bool HasDraggablePoint => !DraggablePoint.IsEmpty;

        public CreateTraitTask TraitTask { get; set; }

        public Trait AddedTrait => TraitTask?.Trait ?? Trait.Empty;
        public int EntityKey { get; private set; }

        public AddTraitCommand(Pad pad, int entityKey, SKPoint start) :
	        this(entityKey, new CreateTerminalPointTask(pad.PadKind, start), new CreateTerminalPointTask(pad.PadKind, start)) { }
        public AddTraitCommand(Pad pad, int entityKey, SKPoint start, SKPoint end) :
	        this(entityKey, new CreateTerminalPointTask(pad.PadKind, start), new CreateTerminalPointTask(pad.PadKind, end)) { }

        public AddTraitCommand(int entityKey, IPoint startPoint) : 
	        this(entityKey, new CreateRefPointTask(startPoint.PadKind, startPoint.Key)) { }

        public AddTraitCommand(int entityKey, IPointTask startPointTask, IPointTask endPointTask = null) : base(startPointTask.Pad)
        {
            StartPointTask = startPointTask;
            EndPointTask = endPointTask ?? new CreateTerminalPointTask(Pad.PadKind, Pad.PointAt(startPointTask.PointKey).Position);
	        EntityKey = entityKey;
        }

        // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
        // this makes tasks continuous like animation or transform commands.

        public override void Execute()
        {
	        base.Execute();
            AddTaskAndRun(StartPointTask);
	        AddTaskAndRun(EndPointTask);
	        if (Pad.EntityAt(EntityKey).IsEmpty)
	        {
                var entityTask = new CreateEntityTask(Pad.PadKind);
                AddTaskAndRun(entityTask);
                EntityKey = entityTask.EntityKey;
	        }
	        TraitTask = new CreateTraitTask(Pad.PadKind, EntityKey, StartPointTask.PointKey, EndPointTask.PointKey, TraitKind.Default);
	        AddTaskAndRun(TraitTask);
        }

        public override void Unexecute()
        {
	        base.Unexecute();
	        //StartPointTask = null;
	        //EndPointTask = null;
	        TraitTask = null;
	        //EntityKey = ElementBase.EmptyKeyValue;
        }

        public override void Update(SKPoint point)
        {
        }

        public override void Completed()
        {
        }
    }
}
