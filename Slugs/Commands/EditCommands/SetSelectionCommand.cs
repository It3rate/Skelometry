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
	    public SetSelectionTask SetSelectionTask { get; private set; }

        public SelectionSet SelectionSet { get; }
        public int PointKey { get; }
        public int[] ElementKeys { get; }

	    public SetSelectionCommand(SelectionSet selSet, int pointKey, params int[] elementKeys) : base(selSet.Pad)
	    {
		    SelectionSet = selSet;
		    PointKey = pointKey;
		    ElementKeys = elementKeys;
	    }
	    public override void Execute()
	    {
		    base.Execute();
		    if (SetSelectionTask == null)
		    {
			    SetSelectionTask = new SetSelectionTask(SelectionSet, PointKey, ElementKeys);
            }
            AddTaskAndRun(SetSelectionTask);
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
