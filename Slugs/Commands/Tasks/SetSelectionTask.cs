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
	    public int PointKey { get; private set; }
	    public int[] ElementKeys { get; private set; }

	    public int PreviousPointKey { get; private set; }
	    public int[] PreviousElementKeys { get; private set; }

        public SetSelectionTask(PadKind padKind, int pointKey, params int[] elementKeys) : base(padKind)
        {
	        PointKey = pointKey;
	        ElementKeys = elementKeys;
        }

        public override void RunTask()
        {
	        base.RunTask();

	        var selectSet = Agent.Current.Data.Selected;
	        PreviousPointKey = selectSet.PointKey;
	        PreviousElementKeys = selectSet.ElementKeysCopy;

            selectSet.Clear();
            if (PointKey != ElementBase.EmptyKeyValue)// && ElementKeys.Length == 0)
	        {
                selectSet.SetPoint(PointKey);
	        }

	        if(ElementKeys.Length > 0)
	        {
	            selectSet.SetElements(ElementKeys);
	        }
        }
    }
}
