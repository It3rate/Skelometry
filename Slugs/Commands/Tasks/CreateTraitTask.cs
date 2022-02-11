using Slugs.Entities;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Commands.Tasks
{
	public class CreateTraitTask : EditTask, ICreateTask
	{
		public int EntityKey { get; set; }
		public int StartPointKey { get; set; }
		public int EndPointKey { get; set; }
		public ElementKind SegmentKind => ElementKind.Trait;

		public Trait Trait;

		public CreateTraitTask(PadKind padKind, int entityKey, int startPointKey, int endPointKey) : 
			base(padKind)
		{
			EntityKey = entityKey;
			StartPointKey = startPointKey;
			EndPointKey = endPointKey;
			// create segment, assign key
		}

		public override void RunTask()
		{
			base.RunTask();
			Trait = Pad.AddTrait(EntityKey, StartPointKey, EndPointKey, 66);
		}

		public override void UnRunTask()
		{
			base.UnRunTask();
            Pad.RemoveElement(Trait.Key);
            foreach (var entity in Pad.Entities)
            {
	            entity.RemoveTrait(Trait.Key);
            }
		}
	}
}