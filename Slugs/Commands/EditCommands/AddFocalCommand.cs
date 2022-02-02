using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Entities;

namespace Slugs.Commands.EditCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AddFocalCommand : EditCommand
    {
        public IPointTask StartPointTask { get; }
        public IPointTask EndPointTask { get; }
        public IPoint DraggablePoint => EndPointTask?.IPoint ?? TerminalPoint.Empty;

        public CreateFocalTask FocalTask { get; set; }

        public Focal AddedFocal => (Focal)FocalTask?.Focal ?? Focal.Empty;
        public int EntityKey { get; }

        public AddFocalCommand(int entityKey, PointOnTrait startPoint) :
            this(entityKey, new CreateRefPointTask(startPoint.PadKind, startPoint.Key))
        { }

        public AddFocalCommand(int entityKey, IPointTask startPointTask, IPointTask endPointTask = null) : base(startPointTask.Pad)
        {
            StartPointTask = startPointTask;
            AddTaskAndRun(StartPointTask);
            EndPointTask = endPointTask ?? new CreateTerminalPointTask(Pad.PadKind, Pad.PointAt(startPointTask.PointKey).Position);
            AddTaskAndRun(EndPointTask);
            EntityKey = entityKey;
            if (Pad.EntityAt(EntityKey).IsEmpty)
            {
                var entityTask = new CreateEntityTask(Pad.PadKind);
                AddTaskAndRun(entityTask);
                EntityKey = entityTask.EntityKey;
            }
        }

        // maybe tasks need to be start/update/complete, where eg merge endpoints is on complete, and MoveElementTask is available for updates.
        // this makes tasks continuous like animation or transform commands.

        public override void Execute()
        {
            base.Execute();
            //FocalTask = new CreateTraitTask(Pad.PadKind, EntityKey, StartPointTask.PointKey, EndPointTask.PointKey, ElementKind.Trait);
            //AddTaskAndRun(FocalTask);
        }

        public override void Update(SKPoint point)
        {
        }

        public override void Completed()
        {
        }
    }
}
