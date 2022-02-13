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
        public int TraitKey => StartPointTask.TraitKey;
        public Trait Trait => Pad.TraitAt(TraitKey);

        public CreateFocalPointTask StartPointTask { get; }
        public CreateFocalPointTask EndPointTask { get; }
        public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;
        public CreateFocalTask FocalTask { get; set; }

        public Focal AddedFocal => (Focal)FocalTask?.AddedFocal ?? Focal.Empty;


        public AddFocalCommand(Entity entity, Trait trait, float startT, float endT) :
            this(entity.Key, new CreateFocalPointTask(trait, startT), new CreateFocalPointTask(trait, endT))
        { }

        public AddFocalCommand(int entityKey, CreateFocalPointTask startPointTask, CreateFocalPointTask endPointTask) : base(startPointTask.Pad)
        {
	        EntityKey = entityKey;
	        StartPointTask = startPointTask;
	        EndPointTask = endPointTask;
        }

        // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
        // this makes tasks continuous like animation or transform commands.

        public override void Execute()
        {
            base.Execute();
            AddTaskAndRun(StartPointTask);
            AddTaskAndRun(EndPointTask);
            if (FocalTask == null)
            {
	            FocalTask = new CreateFocalTask(Entity, StartPointTask.FocalPoint, EndPointTask.FocalPoint);
            }
            AddTaskAndRun(FocalTask);
        }
        public override void Unexecute()
        {
	        base.Unexecute();
        }

        public override void Update(SKPoint point)
        {
        }

        public override void Completed()
        {
        }
    }
}
