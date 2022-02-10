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

    public class AddSingleBondCommand : EditCommand
    {
	    public CreateBondPointTask StartPointTask { get; }
	    public CreateBondPointTask EndPointTask { get; }
	    public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;
	    public CreateSingleBondTask SingleBondTask { get; set; }

	    public SingleBond AddedSingleBond => (SingleBond)SingleBondTask?.AddedSingleBond ?? SingleBond.Empty;

	    public AddSingleBondCommand(Focal startFocal, float startT, Focal endFocal, float endT) :
		    this(new CreateBondPointTask(startFocal, startT), new CreateBondPointTask(endFocal, endT))
	    { }

	    public AddSingleBondCommand(CreateBondPointTask startPointTask, CreateBondPointTask endPointTask) : base(startPointTask.Pad)
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
		    SingleBondTask = new CreateSingleBondTask(StartPointTask.BondPoint, EndPointTask.BondPoint);
		    AddTaskAndRun(SingleBondTask);
	    }

	    public override void Update(SKPoint point)
	    {
	    }

	    public override void Completed()
	    {
	    }
    }
}
