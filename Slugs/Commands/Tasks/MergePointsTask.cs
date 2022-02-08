using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class MergePointsTask : EditTask, IPointTask, IChangeTask
	{
		public int FromKey { get; }
		public int ToKey { get; }
    
		public int PointKey { get; }

		private FocalPoint Point { get; set; }
		public IPoint IPoint => Point;

		public MergePointsTask(PadKind padKind, int fromKey, int toKey) : base(padKind)
		{
			FromKey = fromKey;
			ToKey = toKey;
		}

		public override void RunTask()
		{
			base.RunTask();
			Pad.MergePoints(FromKey, ToKey);
		}
	}
}