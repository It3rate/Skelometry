using SkiaSharp;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreateTerminalPointTask : EditTask, IPointTask, ICreateTask
	{
		public SKPoint Location { get; private set; }
		public TerminalPoint Point { get; set; } = TerminalPoint.Empty;
		public IPoint IPoint => Point;
		public int PointKey => Point?.Key ?? TerminalPoint.EmptyKeyValue;

		public CreateTerminalPointTask(PadKind padKind, SKPoint point) : base(padKind)
		{
			Location = point;
			// create point, assign key
		}

		public override void RunTask()
		{
			if (Point.IsEmpty)
			{
				Point = Pad.CreateTerminalPoint(Location);
            }
			else
			{
				Pad.AddElement(Point);
			}
		}

		public override void UnRunTask()
		{
			Pad.RemoveElement(PointKey);
		}
	}
}