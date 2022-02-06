﻿using Slugs.Agents;
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
        public Focal AddedFocal { get; private set; }

        public Entity Entity { get; }
        public PointOnTrait StartPoint { get; }
        public PointOnTrait EndPoint { get; }

        public int StartPointKey => StartPoint.Key;
        public int EndPointKey => EndPoint.Key;

        public int TraitKey => StartPoint.TraitKey;
        public Trait Trait => Pad.TraitAt(TraitKey);

        public float StartT => AddedFocal.StartPoint.T;
	    public float EndT => AddedFocal.EndPoint.T;

        public ElementKind SegmentKind => ElementKind.Focal;
        public SegmentBase Segment;
    
        public CreateFocalTask(Entity entity, PointOnTrait startPoint, PointOnTrait endPoint) : base(entity.PadKind)
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
		    AddedFocal = new Focal(Entity, StartPoint, EndPoint);
			Trait.AddFocal(AddedFocal);
	    }
    }
}
