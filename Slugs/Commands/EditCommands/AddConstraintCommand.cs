using System.Drawing;
using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Constraints;
using Slugs.Entities;

namespace Slugs.Commands.EditCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AddConstraintCommand : EditCommand
    {
	    public AddConstraintTask ConstraintTask { get; }
	    public IConstraint Constraint => ConstraintTask.Constraint;

	    public AddConstraintCommand(Pad pad, IConstraint constraint) : base(pad)
	    {
            ConstraintTask = new AddConstraintTask(pad.PadKind, constraint);
	    }

	    // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
	    // this makes tasks continuous like animation or transform commands.

	    public override void Execute()
	    {
		    base.Execute();
		    AddTaskAndRun(ConstraintTask);
	    }

	    public override void Unexecute()
	    {
		    base.Unexecute();
	    }

	    public override void Update(SKPoint point)
	    {
	    }

	    public override void Completed()
	    {
	    }
    }
}
