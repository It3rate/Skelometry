using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreateRefPointTask : EditTask, IPointTask, ICreateTask
	{
		public int TargetKey { get; }
		public int PointKey => Point.Key;
		private RefPoint Point { get; set; }
		public IPoint IPoint => Point;
		public CreateRefPointTask(PadKind padKind, int targetKey) : base(padKind)
		{
			TargetKey = targetKey;
			// create ref, assign key
		}
		public override void RunTask()
		{
			Point = Pad.CreateRefPoint(TargetKey);
		}

		public override void UnRunTask()
		{
			//Location = Point.Position; // get location in case it has been updated
			Pad.RemoveElement(PointKey);
			// todo: decrement key counter on undo
		}
	}
}