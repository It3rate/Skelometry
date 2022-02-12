using Slugs.Entities;

namespace Slugs.Commands.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateBondPointTask : EditTask, IPointTask, ICreateTask
    {
	    public BondPoint BondPoint { get; set; } = BondPoint.Empty;

	    public int FocalKey { get; private set; }
	    public float TStore { get; }
	    public IPoint IPoint => BondPoint;
	    public int PointKey => IPoint.Key;


	    public CreateBondPointTask(Focal focal, float t) : base(focal.PadKind)
	    {
		    FocalKey = focal.Key;
		    TStore = t;
	    }

	    public override void RunTask()
	    {
		    base.RunTask();
		    if (BondPoint.IsEmpty)
		    {
			    BondPoint = new BondPoint(PadKind, FocalKey, TStore);
            }
		    else
		    {
			    Pad.AddElement(BondPoint);
		    }

	    }

	    public override void UnRunTask()
	    {
		    base.UnRunTask();
		    Pad.RemoveElement(PointKey);
	    }

	    public void UpdateFocal(Focal focal)
	    {
		    FocalKey = focal.Key;
		    if (!(BondPoint is null))
		    {
			    BondPoint.FocalKey = focal.Key;
		    }
	    }
    }
}
