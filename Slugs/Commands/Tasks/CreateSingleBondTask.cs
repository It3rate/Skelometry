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

    public class CreateSingleBondTask : EditTask, ICreateTask
    {
	    public BondPoint StartPoint { get; }
        public BondPoint EndPoint { get; }

	    public int StartPointKey => StartPoint.Key;
	    public int StartTraitKey => StartPoint.TraitKey;
	    public Trait StartTrait => StartPoint.Trait;
	    public float StartT => StartPoint.T;

        public int EndPointKey => EndPoint.Key;
	    public int EndTraitKey => EndPoint.TraitKey;
	    public Trait EndTrait => EndPoint.Trait;
	    public float EndT => EndPoint.T;

	    public SingleBond AddedSingleBond { get; private set; } = SingleBond.Empty;
	    public int AddedSingleBondKey => AddedSingleBond.Key;

        public ElementKind SegmentKind => ElementKind.SingleBond;
	    public SegmentBase Segment;

        // bonds need an entity to be stored in?
        public CreateSingleBondTask(BondPoint startPoint, BondPoint endPoint) : base(startPoint.PadKind)
	    {
		    StartPoint = startPoint;
            EndPoint = endPoint;
	    }

	    public override void RunTask()
	    {
		    base.RunTask();
		    if (AddedSingleBond.IsEmpty)
		    {
			    AddedSingleBond = new SingleBond(StartPoint, EndPoint);
            }
		    else
		    {
			    Pad.AddElement(AddedSingleBond);
		    }
		    StartPoint.Focal.AddBond(AddedSingleBond);
		    EndPoint.Focal.AddBond(AddedSingleBond);
	    }
	    public override void UnRunTask()
	    {
		    base.UnRunTask();
		    EndPoint.Focal.RemoveBond(AddedSingleBond);
		    StartPoint.Focal.RemoveBond(AddedSingleBond);
            Pad.RemoveElement(AddedSingleBondKey);
	    }
    }
}
