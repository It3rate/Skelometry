using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreateVPointTask : EditTask, IPointTask, ICreateTask
	{
		public int PointKey { get; }
		public int SegmentKey { get; }
		public float T { get; }
		private PointOnTrait Point { get; set; }
		public IPoint IPoint => Point;

		public CreateVPointTask(PadKind padKind, int segmentkey, float t) : base(padKind)
		{
			SegmentKey = segmentkey;
			T = t;
			// create vpoint, assign key
		}
	}
}