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
	    private CreateSegmentTask TraitTask { get; set; }
	    //private MoveElementTask MoveEndPoint { get; } // or just directly change Endpoint?
	    private MergePointsTask MergeEndPoint { get; }

        public Trait AddedTrait => (Trait)TraitTask?.Segment ?? Trait.Empty;

        public AddTraitCommand(Pad pad, SKPoint start):base(pad)
	    {
		    StartPointTask = new CreateTerminalPointTask(PadKind.Input, start);
		    EndPointTask = new CreateTerminalPointTask(PadKind.Input, start);
            Tasks.Add(StartPointTask);
            Tasks.Add(EndPointTask);
        }
	    public AddTraitCommand(Pad pad, IPointTask startPointTask, IPointTask endTask) : base(pad)
        {
	        StartPointTask = startPointTask;
	        EndPointTask = endTask;
            Tasks.Add(StartPointTask);
            Tasks.Add(EndPointTask);
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
	        StartPointTask.RunTask();
	        EndPointTask.RunTask();
	        TraitTask = new CreateSegmentTask(PadKind.Input, StartPointTask.PointKey, EndPointTask.PointKey, ElementKind.Trait);
            TraitTask.RunTask();
        }
    }
}
