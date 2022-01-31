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
    public class AddTraitCommand : EditCommand, IDraggablePointCommand
    {
	    public IPointTask StartPointTask { get; }
	    public IPointTask EndPointTask { get; }
	    public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;

        public CreateSegmentTask TraitTask { get; set; }
	    //public CreateEntityTask EntityTask { get; set; }
        //private MoveElementTask MoveEndPoint { get; } // or just directly change Endpoint?
        //public MergePointsTask MergeEndPoint { get; set; }

        public Trait AddedTrait => (Trait)TraitTask?.Segment ?? Trait.Empty;
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

        public void MoveEndPoint(SKPoint location)
	    {
		    IPoint point = (IPoint)ElementAt(EndPointTask.PointKey);
		    point.Position = location;
	    }

        public override void Execute()
        {
	        base.Execute();
	        TraitTask = new CreateSegmentTask(Pad.PadKind, EntityKey, StartPointTask.PointKey, EndPointTask.PointKey, ElementKind.Trait);
	        AddTaskAndRun(TraitTask);

	        //MergeEndPoint = new MergePointsTask(Pad.PadKind, -1, -1);
        }

        public override void Update(SKPoint point)
        {
        }

        public override void Completed()
        {
        }
    }
}
