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

    public class SetSelectionCommand : EditCommand
    {
	    public SetSelectionTask SetSelectionTask { get; }

	    public SetSelectionCommand(Pad pad, int pointKey, params int[] elementKeys) : base(pad)
	    {
		    SetSelectionTask = new SetSelectionTask(Pad.PadKind, pointKey, elementKeys);
	    }
	    public override void Execute()
	    {
		    AddTaskAndRun(SetSelectionTask);
	    }
	    public override void Update(SKPoint point)
	    {
	    }
	    public override void Completed()
	    {
	    }
    }
}
