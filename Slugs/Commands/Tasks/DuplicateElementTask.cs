using System.Drawing;
using SkiaSharp;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DuplicateElementTask : EditTask
    {
	    public IElement ElementToDuplicate { get; }
	    public IElement DuplicateElement { get; private set; }

	    public DuplicateElementTask(PadKind padKind, IElement elementToDuplicate) : base(padKind)
	    {
		    ElementToDuplicate = elementToDuplicate;
	    }

	    public override void RunTask()
	    {
		    if (DuplicateElement == null)
		    {
			    DuplicateElement = ElementToDuplicate.Duplicate(true);
		    }
		    else
		    {
			    Pad.AddElement(DuplicateElement);
		    }
	    }

	    public override void UnRunTask()
	    {
		    Pad.RemoveElement(DuplicateElement.Key);
	    }
    }
}
