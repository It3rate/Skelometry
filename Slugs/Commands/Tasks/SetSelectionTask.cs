using Slugs.Agents;
using Slugs.Entities;
using Slugs.Input;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISelectionTask { }

    public class SetSelectionTask : EditTask, ISelectionTask
    {
        public SelectionSet SelectionSet { get; }

        public int PointKey { get; private set; }
	    public int[] ElementKeys { get; private set; }

	    public int PreviousPointKey { get; private set; }
	    public int[] PreviousElementKeys { get; private set; }

        public SetSelectionTask(SelectionSet selSet, int pointKey, params int[] elementKeys) : base(selSet.PadKind)
        {
	        SelectionSet = selSet;
	        PointKey = pointKey;
	        ElementKeys = elementKeys;
        }

        private bool _hasRun = false;

        public override void RunTask()
        {
	        base.RunTask();

	        if (!_hasRun)
	        {
		        PreviousPointKey = SelectionSet.PointKey;
		        PreviousElementKeys = SelectionSet.ElementKeysCopy;
		        _hasRun = true;
	        }

	        SelectionSet.Clear();
            SelectionSet.SetPoint(PointKey);
			SelectionSet.SetElements(ElementKeys);
        }

        public override void UnRunTask()
        {
	        SelectionSet.Clear();
	        SelectionSet.SetPoint(PreviousPointKey);
	        SelectionSet.SetElements(PreviousElementKeys);
        }
    }
}
