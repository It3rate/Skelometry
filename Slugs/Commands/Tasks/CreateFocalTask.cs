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
        public Focal AddedFocal { get; private set; } = Focal.Empty;

        public Entity Entity { get; }
        public FocalPoint StartPoint { get; }
        public FocalPoint EndPoint { get; }

        public int StartPointKey => StartPoint.Key;
        public int EndPointKey => EndPoint.Key;

        public int TraitKey => StartPoint.TraitKey;
        public Trait Trait => Pad.TraitAt(TraitKey);

        public float StartT => AddedFocal.StartFocalPoint.T;
	    public float EndT => AddedFocal.EndFocalPoint.T;

        public ElementKind SegmentKind => ElementKind.Focal;
        public SegmentBase Segment;
    
        public CreateFocalTask(Entity entity, FocalPoint startPoint, FocalPoint endPoint) : base(entity.PadKind)
        {
	        if (startPoint.TraitKey != endPoint.TraitKey)
	        {
                throw new ArgumentException("AddedFocal points must be on the same trait.");
	        }
	        Entity = entity;
	        StartPoint = startPoint;
	        EndPoint = endPoint;
        }

        public override void RunTask()
	    {
		    base.RunTask();
		    if (AddedFocal.IsEmpty)
		    {
			    AddedFocal = new Focal(Entity, StartPoint, EndPoint);
		    }
		    else
		    {
                Pad.AddElement(AddedFocal);
		    }
		    Trait.AddFocal(AddedFocal);
		    Entity.AddFocal(AddedFocal);
        }
        public override void UnRunTask()
        {
	        base.UnRunTask();
            Entity.RemoveFocal(AddedFocal);
            Trait.RemoveFocal(AddedFocal);
            Pad.RemoveElement(AddedFocal.Key);
        }
    }
}
