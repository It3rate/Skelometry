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

    public class AddConstraintTask : EditTask, ICreateTask
    {
        public IConstraint Constraint { get; }
        public AddConstraintTask(PadKind padKind, IConstraint constraint) : base(padKind)
        {
	        Constraint = constraint;
        }

	    public override void RunTask()
	    {
		    base.RunTask();
		    Pad.Constraints.Add(Constraint);
		    Constraint.OnElementChanged(Constraint.StartElement);

	    }
	    public override void UnRunTask()
	    {
		    base.UnRunTask();
		    Pad.Constraints.Remove(Constraint);
        }
    }
}
