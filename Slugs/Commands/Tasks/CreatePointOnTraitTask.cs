using System.Drawing;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreatePointOnTraitTask : EditTask, IPointTask, ICreateTask
	{
        public PointOnTrait PointOnTrait { get; set; }

		public int TraitKeyStore { get; }
		public float TStore { get; }
        public IPoint IPoint => PointOnTrait;
        public int PointKey => PointOnTrait.Key;

        public CreatePointOnTraitTask(Trait trait, float t) : base(trait.PadKind)
		{
			TraitKeyStore = trait.Key;
			TStore = t;
		}

		public override void RunTask()
		{
			base.RunTask();
			PointOnTrait = new PointOnTrait(PadKind, TraitKeyStore, TStore);
		}
    }
}