using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.EditCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AddFocalCommand : EditCommand
    {
        public int EntityKey { get; }
        public Entity Entity => Pad.EntityAt(EntityKey);
        public int TraitKey => StartPointTask.TraitKeyStore;
        public Trait Trait => Pad.TraitAt(TraitKey);

        public CreatePointOnTraitTask StartPointTask { get; }
        public CreatePointOnTraitTask EndPointTask { get; }
        public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;
        public CreateFocalTask FocalTask { get; set; }

        public Focal AddedFocal => (Focal)FocalTask?.AddedFocal ?? Focal.Empty;


        public AddFocalCommand(Entity entity, Trait trait, float startT, float endT) :
            this(entity.Key, new CreatePointOnTraitTask(trait, startT), new CreatePointOnTraitTask(trait, endT))
        { }

        public AddFocalCommand(int entityKey, CreatePointOnTraitTask startPointTask, CreatePointOnTraitTask endPointTask) : base(startPointTask.Pad)
        {
	        EntityKey = entityKey;

	        StartPointTask = startPointTask;
	        AddTaskAndRun(StartPointTask);
	        EndPointTask = endPointTask;
	        AddTaskAndRun(EndPointTask);
        }

        // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
        // this makes tasks continuous like animation or transform commands.

        public override void Execute()
        {
            base.Execute();
            FocalTask = new CreateFocalTask(Entity, StartPointTask.PointOnTrait, EndPointTask.PointOnTrait);
            AddTaskAndRun(FocalTask);
        }

        public override void Update(SKPoint point)
        {
        }

        public override void Completed()
        {
        }
    }
}
