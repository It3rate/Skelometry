using System.Drawing;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreatePointOnTraitTask : EditTask, IPointTask, ICreateTask
	{
        public PointOnTrait PointOnTrait { get; set; }

		public int TraitKey { get; }
		public float TStore { get; }
        public IPoint IPoint => PointOnTrait;
        public int PointKey => PointOnTrait.Key;

        public CreatePointOnTraitTask(Trait trait, float t) : base(trait.PadKind)
		{
			TraitKey = trait.Key;
			TStore = t;
		}

		public override void RunTask()
		{
			base.RunTask();
			PointOnTrait = new PointOnTrait(PadKind, TraitKey, TStore);
		}
    }
}