using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreateEntityTask : EditTask, ICreateTask
	{
        public Entity Entity { get; private set; } = Entity.Empty;
		public int EntityKey => Entity.Key;
		public CreateEntityTask(PadKind padKind) : base(padKind)
		{
			// create entity, assign key
		}

		public override void RunTask()
		{
			base.RunTask();
			if (Entity.IsEmpty)
			{
                Entity = new Entity(PadKind);
            }
			else
			{
				Pad.AddElement(Entity);
			}
		}
		public override void UnRunTask()
		{
			base.UnRunTask();
			Pad.RemoveElement(EntityKey);
		}
    }
}