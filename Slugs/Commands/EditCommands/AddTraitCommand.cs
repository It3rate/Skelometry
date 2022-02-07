using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.EditCommands
{
    // CreateTerminalPointTask
    // CreateRefPointTask
    // CreatePointOnTraitTask
    // CreateTraitTask
    // CreateEntityTask
    // MoveElementTask
    // MoveVPointTask
    // StretchSegmentTask
    public class AddTraitCommand : EditCommand, IDraggableCommand
    {
	    public IPointTask StartPointTask { get; }
	    public IPointTask EndPointTask { get; }
	    public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;
	    public bool HasDraggablePoint => !DraggablePoint.IsEmpty;

        public CreateTraitTask TraitTask { get; set; }

        public Trait AddedTrait => TraitTask?.Trait ?? Trait.Empty;
        public int EntityKey { get; }

        public AddTraitCommand(Pad pad, int entityKey, SKPoint start) :
	        this(entityKey, new CreateTerminalPointTask(pad.PadKind, start), new CreateTerminalPointTask(pad.PadKind, start)) { }

        public AddTraitCommand(int entityKey, IPoint startPoint) : 
	        this(entityKey, new CreateRefPointTask(startPoint.PadKind, startPoint.Key)) { }

        public AddTraitCommand(int entityKey, IPointTask startPointTask, IPointTask endPointTask = null) : base(startPointTask.Pad)
        {
            StartPointTask = startPointTask;
            AddTaskAndRun(StartPointTask);
            EndPointTask = endPointTask ?? new CreateTerminalPointTask(Pad.PadKind, Pad.PointAt(startPointTask.PointKey).Position);
	        AddTaskAndRun(EndPointTask);
	        EntityKey = entityKey;
	        if (Pad.EntityAt(EntityKey).IsEmpty)
	        {
                var entityTask = new CreateEntityTask(Pad.PadKind);
                AddTaskAndRun(entityTask);
                EntityKey = entityTask.EntityKey;
	        }
        }

        // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
        // this makes tasks continuous like animation or transform commands.

        public override void Execute()
        {
	        base.Execute();
	        TraitTask = new CreateTraitTask(Pad.PadKind, EntityKey, StartPointTask.PointKey, EndPointTask.PointKey);
	        AddTaskAndRun(TraitTask);
        }

        public override void Update(SKPoint point)
        {
        }

        public override void Completed()
        {
        }
    }
}
