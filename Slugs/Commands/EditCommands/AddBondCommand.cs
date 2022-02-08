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
	    public CreateBondPointTask StartPointTask { get; }
	    public CreateBondPointTask EndPointTask { get; }
	    public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;
	    public CreateBondTask BondTask { get; set; }

	    public Bond AddedBond => (Bond)BondTask?.AddedBond ?? Bond.Empty;

	    public AddBondCommand(Focal startFocal, float startT, Focal endFocal, float endT) :
		    this(new CreateBondPointTask(startFocal, startT), new CreateBondPointTask(endFocal, endT))
	    { }

	    public AddBondCommand(CreateBondPointTask startPointTask, CreateBondPointTask endPointTask) : base(startPointTask.Pad)
	    {
		    StartPointTask = startPointTask;
		    AddTaskAndRun(StartPointTask);
		    EndPointTask = endPointTask;
		    AddTaskAndRun(EndPointTask);
	    }

	    public void UpdateEndPointFocal(Focal focal)
	    {
		    EndPointTask.UpdateFocal(focal);
	    }
        // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
        // this makes tasks continuous like animation or transform commands.

        public override void Execute()
	    {
		    base.Execute();
		    BondTask = new CreateBondTask(StartPointTask.BondPoint, EndPointTask.BondPoint);
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
