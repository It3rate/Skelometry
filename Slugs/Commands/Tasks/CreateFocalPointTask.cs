using System.Drawing;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreateFocalPointTask : EditTask, IPointTask, ICreateTask
	{
        public FocalPoint FocalPoint { get; set; }

		public int TraitKey { get; }
		public float TStore { get; }
        public IPoint IPoint => FocalPoint;
        public int PointKey => FocalPoint.Key;

        public CreateFocalPointTask(Trait trait, float t) : base(trait.PadKind)
		{
			TraitKey = trait.Key;
			TStore = t;
		}

		public override void RunTask()
		{
			base.RunTask();
			FocalPoint = new FocalPoint(PadKind, TraitKey, TStore);
		}
    }
}