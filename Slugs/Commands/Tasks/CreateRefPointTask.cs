﻿using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
	public class CreateRefPointTask : EditTask, IPointTask, ICreateTask
	{
		public int TargetKey { get; }
		private RefPoint Point { get; set; } = RefPoint.Empty;
		public int PointKey => Point.Key;
		public IPoint IPoint => Point;
		public CreateRefPointTask(PadKind padKind, int targetKey) : base(padKind)
		{
			TargetKey = targetKey;
			// create ref, assign key
		}

		public override void RunTask()
		{
			base.RunTask();
			if (Point.IsEmpty)
			{
				Point = Pad.CreateRefPoint(TargetKey);
            }
			else
			{
				Pad.AddElement(Point);
			}

		}
		public override void UnRunTask()
		{
			base.UnRunTask();
			Pad.RemoveElement(PointKey);
		}
    }
}