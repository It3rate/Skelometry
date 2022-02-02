using SkiaSharp;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreateTerminalPointTask : EditTask, IPointTask, ICreateTask
	{
		public SKPoint Location { get; private set; }
		public int PointKey => Point?.Key ?? TerminalPoint.EmptyKeyValue;
		public TerminalPoint Point { get; set; }
		public IPoint IPoint => Point;

		public CreateTerminalPointTask(PadKind padKind, SKPoint point) : base(padKind)
		{
			Location = point;
			// create point, assign key
		}

		public override void RunTask()
		{
			Point = Pad.CreateTerminalPoint(Location);
		}

		public override void UnRunTask()
		{
			//Location = Point.Position; // get location in case it has been updated
			Pad.RemoveElement(Point.Key);
			// todo: decrement key counter on undo
		}
	}
}