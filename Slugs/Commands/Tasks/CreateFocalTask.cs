using Slugs.Agents;
using Slugs.Entities;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Commands.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateFocalTask : EditTask, ICreateTask
    {
	    public int TraitKey { get; set; }
	    public Trait Trait => Pad.TraitAt(TraitKey);

        public ElementKind SegmentKind => ElementKind.Focal;
	    public int StartPointKey { get; }
	    public int EndPointKey { get; }
        public SegmentBase Segment;
        public Focal Focal { get; private set; }

	    public CreateFocalTask(PadKind padKind, int traitKey, PointOnTrait startPoint, PointOnTrait endPoint) : base(padKind)
	    {
		    TraitKey = traitKey;
		    StartPointKey = startPoint.Key;
		    EndPointKey = endPoint.Key;
        }

	    public override void RunTask()
	    {
		    base.RunTask();
		    Focal = new Focal(PadKind, TraitKey, StartPointKey, EndPointKey);
			Trait.AddFocal(Focal);
	    }
    }
}
