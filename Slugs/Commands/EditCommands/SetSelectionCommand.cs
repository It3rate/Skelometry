using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Entities;
using Slugs.Input;

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

	    public SetSelectionCommand(SelectionSet selSet, int pointKey, params int[] elementKeys) : base(selSet.Pad)
	    {
		    SetSelectionTask = new SetSelectionTask(selSet, pointKey, elementKeys);
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
