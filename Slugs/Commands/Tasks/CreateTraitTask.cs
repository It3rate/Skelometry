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
		public TraitKind TraitKind { get; private set; }

        public Trait Trait { get; private set; } = Trait.Empty;
        public int TraitKey => Trait.Key;

		public CreateTraitTask(PadKind padKind, int entityKey, int startPointKey, int endPointKey, TraitKind traitKind) : base(padKind)
		{
			EntityKey = entityKey;
			StartPointKey = startPointKey;
			EndPointKey = endPointKey;
			TraitKind = traitKind;
			// create segment, assign key
		}

		public override void RunTask()
		{
			if (Trait.IsEmpty)
			{
				Trait = Pad.CreateTrait(EntityKey, StartPointKey, EndPointKey, TraitKind);
            }
			else
			{
				Pad.AddElement(Trait);
			}
		}

		public override void UnRunTask()
		{
			Pad.RemoveElement(TraitKey);
			foreach (var entity in Pad.Entities) // todo: Don't add traits to entities
			{
				Pad.RemoveElement(Trait.Key);
			}
        }
    }
}