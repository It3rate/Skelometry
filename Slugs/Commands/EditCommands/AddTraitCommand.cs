using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.EditCommands
{
    // CreateTerminalPointTask
    // CreateRefPointTask
    // CreateVPointTask
    // CreateSegmentTask
    // CreateEntityTask
    // MoveElementTask
    // MoveVPointTask
    // StretchSegmentTask
    public class AddTraitCommand : EditCommand
    {
	    private IPointTask StartPointTask { get; }
	    private IPointTask EndPointTask { get; }
	    private CreateEntityTask EntityTask { get; set; }
	    private CreateSegmentTask TraitTask { get; set; }
        //private MoveElementTask MoveEndPoint { get; } // or just directly change Endpoint?
        private MergePointsTask MergeEndPoint { get; set; }

        public Trait AddedTrait => (Trait)TraitTask?.Segment ?? Trait.Empty;
        public int EntityKey { get; }

        public AddTraitCommand(Pad pad, int entityKey, SKPoint start):
	        this(entityKey, new CreateTerminalPointTask(pad.PadKind, start), new CreateTerminalPointTask(pad.PadKind, start)){}

	    public AddTraitCommand(int entityKey, IPointTask startPointTask, IPointTask endPointTask) : base(startPointTask.Pad)
        {
            StartPointTask = startPointTask;
	        EndPointTask = endPointTask;
	        AddTasksAndRun(StartPointTask, EndPointTask);
	        EntityKey = entityKey;
	        if (Pad.EntityAt(EntityKey).IsEmpty)
	        {
                EntityTask = new CreateEntityTask(Pad.PadKind);
                AddTaskAndRun(EntityTask);
                EntityKey = EntityTask.EntityKey;
	        }
        }

        // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
        // this makes tasks continuous like animation or transform commands.

        public void MoveEndPoint(SKPoint location)
	    {
		    IPoint point = (IPoint)ElementAt(EndPointTask.PointKey);
		    point.SKPoint = location;
	    }

        public override void Execute()
        {
	        base.Execute();
	        TraitTask = new CreateSegmentTask(Pad.PadKind, EntityKey, StartPointTask.PointKey, EndPointTask.PointKey, ElementKind.Trait);
	        AddTaskAndRun(TraitTask);

	        MergeEndPoint = new MergePointsTask(Pad.PadKind, -1, -1);
        }

        public override void Update(SKPoint point)
        {
        }

        public override void Completed()
        {
        }
    }
}
