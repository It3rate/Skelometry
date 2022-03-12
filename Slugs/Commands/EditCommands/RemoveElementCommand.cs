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
    
    public class RemoveElementCommand : EditCommand
    {
	    public RemoveElementTask RemoveElementTask { get; set; }
	    public IElement Element;

	    public RemoveElementCommand(Pad pad, IElement element) : base(pad)
	    {
		    Element = element;
	    }

	    public override void Execute()
	    {
		    base.Execute();
		    if (RemoveElementTask == null)
		    {
			    RemoveElementTask = new RemoveElementTask(Pad.PadKind, Element);
            }
		    AddTaskAndRun(RemoveElementTask);
	    }
    }
}
