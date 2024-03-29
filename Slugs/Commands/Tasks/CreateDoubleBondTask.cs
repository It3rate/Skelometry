﻿using Slugs.Entities;
using Slugs.Primitives;

namespace Slugs.Commands.Tasks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class CreateDoubleBondTask : EditTask, ICreateTask
	{
		private Focal StartFocal { get; }
		private Focal EndFocal { get; set; }

		public int StartFocalKey => StartFocal.Key;
		public int StartTraitKey => StartFocal.TraitKey;
		public Trait StartTrait => StartFocal.Trait;

		public int EndFocalKey => EndFocal.Key;
		public int EndTraitKey => EndFocal.TraitKey;
		public Trait EndTrait => EndFocal.Trait;

		public ElementKind SegmentKind => ElementKind.DoubleBond;
		public DoubleBond AddedDoubleBond { get; private set; } = DoubleBond.Empty;

		// bonds need an entity to be stored in?
		public CreateDoubleBondTask(Focal startFocal, Focal endFocal) : base(startFocal.PadKind)
		{
			StartFocal = startFocal;
			EndFocal = endFocal;
		}

		public override void RunTask()
		{
			base.RunTask();
			if (AddedDoubleBond.IsEmpty)
			{
				AddedDoubleBond = new DoubleBond(StartFocal, EndFocal);
            }
			else
			{
				Pad.AddElement(AddedDoubleBond);
			}
			StartFocal.Entity.AddDoubleBond(AddedDoubleBond);
		}

		public override void UnRunTask()
		{
			base.UnRunTask();
			StartFocal.Entity.RemoveDoubleBond(AddedDoubleBond);
            Pad.RemoveElement(AddedDoubleBond.Key);
		}

        public void SetEndFocal(Focal endFocal)
		{
			EndFocal = endFocal;
			AddedDoubleBond.EndFocal = endFocal;
		}
	}
}
