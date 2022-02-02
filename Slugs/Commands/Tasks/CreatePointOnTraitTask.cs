using System.Drawing;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreatePointOnTraitTask : EditTask, IPointTask, ICreateTask
	{
		public int PointKey { get; }
		public int TraitKey { get; }
		public float T { get; }

        public PointOnTrait PointOnTrait { get; set; }
		public IPoint IPoint => PointOnTrait;

		public CreatePointOnTraitTask(Trait trait, float t) : base(trait.PadKind)
		{
			TraitKey = trait.Key;
			T = t;
		}

		public override void RunTask()
		{
			base.RunTask();
			PointOnTrait = new PointOnTrait(PadKind, TraitKey, T);
		}
    }
}