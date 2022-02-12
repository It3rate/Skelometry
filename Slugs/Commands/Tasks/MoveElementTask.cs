using System.Collections.Generic;
using System.Windows.Documents;
using SkiaSharp;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class MoveElementTask : EditTask, IChangeTask
	{
		public IElement Element { get; private set; }
        public int ElementKey => Element.Key;
		public SKPoint Diff { get; } // always relative
        private List<SKPoint> OriginalPoints = new List<SKPoint>();

		public MoveElementTask(PadKind padKind, IElement element, SKPoint diff) : base(padKind)
		{
			Element = element;
			Diff = diff;
		}

		public override void RunTask()
		{
			var recordPosition = OriginalPoints.Count == 0;
			foreach (var point in Element.Points)
			{
				if (recordPosition)
				{
                    OriginalPoints.Add(point.Position);
				}
				point.Position += Diff;
			}
        }

		public override void UnRunTask()
		{
			var pts = Element.Points;
			for (int i = OriginalPoints.Count - 1; i >= 0; i--)
			{
				pts[i].Position = OriginalPoints[i] - Diff;
			}
		}
    }
}