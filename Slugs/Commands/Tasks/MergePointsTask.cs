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
        private IPoint OriginalPoint { get; set; } = TerminalPoint.Empty;

		public MergePointsTask(PadKind padKind, int fromKey, int toKey) : base(padKind)
		{
			FromKey = fromKey;
			ToKey = toKey;
		}

		public override void RunTask()
		{
			base.RunTask();
			if (OriginalPoint.IsEmpty)
			{
				OriginalPoint = Pad.PointAt(FromKey);
			}
			Pad.MergePoints(FromKey, ToKey);
		}

		public override void UnRunTask()
		{
            Pad.SetPointAt(FromKey, OriginalPoint);
		}
    }
}