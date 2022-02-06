using Slugs.Entities;

namespace Slugs.Commands.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreatePointOnFocalTask : EditTask, IPointTask, ICreateTask
    {
        public PointOnFocal PointOnFocal { get; set; }

	    public int FocalKeyStore { get; }
	    public float TStore { get; }
	    public IPoint IPoint => PointOnFocal;
	    public int PointKey => PointOnFocal.Key;


	    public CreatePointOnFocalTask(Focal focal, float t) : base(focal.PadKind)
	    {
		    FocalKeyStore = focal.Key;
		    TStore = t;
	    }

	    public override void RunTask()
	    {
		    base.RunTask();
		    PointOnFocal = new PointOnFocal(PadKind, FocalKeyStore, TStore);
	    }
    }
}
