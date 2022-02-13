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
	    public CreateDoubleBondTask DoubleBondTask { get; private set; }

	    public Focal StartFocal { get; }
	    public Focal EndFocal { get; }

        public AddDoubleBondCommand(Focal startFocal, Focal endFocal) : base(startFocal.Pad)
        {
	        StartFocal = startFocal;
	        EndFocal = endFocal;
        }
	    public override void Execute()
	    {
		    base.Execute();
		    if (DoubleBondTask == null)
		    {
			    DoubleBondTask = new CreateDoubleBondTask(StartFocal, EndFocal);
            }
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
