using SkiaSharp;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class MoveElementTask : EditTask, IChangeTask
	{
		public int ElementKey { get; set; }
		public SKPoint Diff { get; } // always relative

		public MoveElementTask(PadKind padKind, int key, SKPoint diff) : base(padKind)
		{
			ElementKey = key;
			Diff = diff;
		}
	}
}