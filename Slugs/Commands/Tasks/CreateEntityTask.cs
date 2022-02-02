using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreateEntityTask : EditTask, ICreateTask
	{
		public int EntityKey { get; set; }
		public CreateEntityTask(PadKind padKind) : base(padKind)
		{
			// create entity, assign key
		}

		public override void RunTask()
		{
			base.RunTask();
			var entity = new Entity(PadKind);
			EntityKey = entity.Key;
		}
	}
}