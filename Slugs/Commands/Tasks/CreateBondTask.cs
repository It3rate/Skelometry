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

    public class CreateBondTask : EditTask, ICreateTask
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

        public ElementKind SegmentKind => ElementKind.Bond;
	    public SegmentBase Segment;
	    public Bond AddedBond { get; private set; }

        // bonds need an entity to be stored in?
	    public CreateBondTask(BondPoint startPoint, BondPoint endPoint) : base(startPoint.PadKind)
	    {
		    StartPoint = startPoint;
            EndPoint = endPoint;
	    }

	    public override void RunTask()
	    {
		    base.RunTask();
		    AddedBond = new Bond(StartPoint, EndPoint);
		    StartPoint.Focal.AddBond(AddedBond);
		    EndPoint.Focal.AddBond(AddedBond);
        }
    }
}
