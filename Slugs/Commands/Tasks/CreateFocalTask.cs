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
	    public int EntityKey { get; set; }
	    public int StartPointKey { get; set; }
	    public int EndPointKey { get; set; }
	    public ElementKind SegmentKind => ElementKind.Focal;

	    public SegmentBase Segment;
        public Focal Focal { get; private set; }

	    public CreateFocalTask(PadKind padKind, int entityKey, int startPointKey, int endPointKey) :
		    base(padKind)
	    {
		    EntityKey = entityKey;
		    StartPointKey = startPointKey;
		    EndPointKey = endPointKey;
		    // create segment, assign key
	    }

	    public override void RunTask()
	    {
		    base.RunTask();
		    //Segment = Pad.AddTrait(EntityKey, StartPointKey, EndPointKey, 66);
	    }
    }
}
