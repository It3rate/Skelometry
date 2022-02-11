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

    public class AddDoubleBondCommand : EditCommand
    {
	    public CreateDoubleBondTask DoubleBondTask { get; }

	    public Focal StartFocal => DoubleBondTask.AddedDoubleBond.StartFocal;
	    public Focal EndFocal => DoubleBondTask.AddedDoubleBond.StartFocal;

	    public AddDoubleBondCommand(Focal startFocal, Focal endFocal) : base(startFocal.Pad)
	    {
            DoubleBondTask = new CreateDoubleBondTask(startFocal, endFocal);
	    }
	    public override void Execute()
	    {
		    base.Execute();
		    AddTaskAndRun(DoubleBondTask);
	    }

	    public override void Update(SKPoint point)
	    {
	    }

	    public override void Completed()
	    {
	    }

	    public void UpdateEndFocal(Focal focal)
	    {
		    DoubleBondTask.SetEndFocal(focal);
	    }
    }
}
