using SkiaSharp;
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
        private readonly Dictionary<int, SKPoint> _changes = new Dictionary<int, SKPoint>();

        public AddConstraintTask(PadKind padKind, IConstraint constraint) : base(padKind)
        {
	        Constraint = constraint;
        }

	    public override void RunTask()
	    {
		    base.RunTask();
		    Pad.AddConstraint(Constraint);
		    Constraint.OnElementChanged(Constraint.StartElement, _changes);

	    }
	    public override void UnRunTask()
	    {
		    base.UnRunTask();
		    Pad.RemoveConstraint(Constraint);
        }
    }
}
