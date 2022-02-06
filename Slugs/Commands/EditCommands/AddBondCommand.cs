using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Entities;

namespace Slugs.Commands.EditCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AddBondCommand : EditCommand
    {
	    public CreatePointOnFocalTask StartPointTask { get; }
	    public CreatePointOnFocalTask EndPointTask { get; }
	    public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;
	    public CreateBondTask BondTask { get; set; }

	    public Bond AddedBond => (Bond)BondTask?.AddedBond ?? Bond.Empty;

	    public int FocalKey { get; }
	    public Focal Focal => Pad.FocalAt(FocalKey);

	    public AddBondCommand(Focal startFocal, float startT, Focal endFocal, float endT) :
		    this(new CreatePointOnFocalTask(startFocal, startT), new CreatePointOnFocalTask(endFocal, endT))
	    { }

	    public AddBondCommand(CreatePointOnFocalTask startPointTask, CreatePointOnFocalTask endPointTask) : base(startPointTask.Pad)
	    {
		    StartPointTask = startPointTask;
		    AddTaskAndRun(StartPointTask);
		    EndPointTask = endPointTask;
		    AddTaskAndRun(EndPointTask);
	    }

	    // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
	    // this makes tasks continuous like animation or transform commands.

	    public override void Execute()
	    {
		    base.Execute();
		    BondTask = new CreateBondTask(StartPointTask.PointOnFocal, EndPointTask.PointOnFocal);
		    AddTaskAndRun(BondTask);
	    }

	    public override void Update(SKPoint point)
	    {
	    }

	    public override void Completed()
	    {
	    }
    }
}
