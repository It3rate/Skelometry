using Slugs.Entities;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Commands.Tasks
{
	public class CreateTraitTask : EditTask, ICreateTask
	{
		public IPoint StartPoint { get; }
		public IPoint EndPoint { get; }
		public ElementKind SegmentKind => ElementKind.Trait;
		public TraitKind TraitKind { get; private set; }

        public Trait Trait { get; private set; } = Trait.Empty;
        public int TraitKey => Trait.Key;

		public CreateTraitTask(PadKind padKind, TraitKind traitKind, IPoint startPoint, IPoint endPoint) : base(padKind)
		{
			TraitKind = traitKind;
			StartPoint = startPoint;
			EndPoint = endPoint;
		}

		public override void RunTask()
		{
			if (Trait.IsEmpty)
			{
				Trait = new Trait(TraitKind, StartPoint, EndPoint);
            }
			else
			{
				Pad.AddElement(Trait);
			}
		}

		public override void UnRunTask()
		{
			Pad.RemoveElement(TraitKey);
        }
    }
}