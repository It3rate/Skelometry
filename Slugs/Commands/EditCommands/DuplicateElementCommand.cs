using Slugs.Commands.Tasks;
using Slugs.Entities;

namespace Slugs.Commands.EditCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DuplicateElementCommand : EditCommand
    {
	    public DuplicateElementTask DuplicateElementTask { get; set; }
	    public IElement Element;

	    public DuplicateElementCommand(Pad pad, IElement element) : base(pad)
	    {
		    Element = element;
	    }

	    public override void Execute()
	    {
		    base.Execute();
		    if (DuplicateElementTask == null)
		    {
			    DuplicateElementTask = new DuplicateElementTask(Pad.PadKind, Element);
		    }
		    AddTaskAndRun(DuplicateElementTask);
	    }
    }
}
