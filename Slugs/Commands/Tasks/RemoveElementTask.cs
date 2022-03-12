using Slugs.Agents;
using Slugs.Constraints;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RemoveElementTask : EditTask, IChangeTask
    {
	    public int ElementKey { get; }
	    private IElement _removedElement;
	    private IEnumerable<IConstraint> _removedConstraints = new List<IConstraint>();

	    public RemoveElementTask(PadKind padKind, IElement element) : base(padKind)
	    {
		    ElementKey = element.Key;
	    }

	    public override void RunTask()
	    {
		    base.RunTask();
		    _removedElement = Pad.ElementAt(ElementKey);
            Pad.RemoveElement(ElementKey);
            _removedConstraints = Pad.GetRelatedConstraints(_removedElement);
            Pad.RemoveConstraints(_removedConstraints);
	    }

	    public override void UnRunTask()
	    {
		    Pad.AddElement(_removedElement);
            Pad.AddConstraints(_removedConstraints);
	    }
    }
}
