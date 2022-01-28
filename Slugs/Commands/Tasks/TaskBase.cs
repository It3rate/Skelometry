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

    public interface ITask
    {
        PadKind PadKind { get; }
        int TaskKey { get; }
        bool IsValid { get; }
    }

    public abstract class TaskBase : ITask
    {
	    private static int TaskCounter = 1;

        public PadKind PadKind { get; }
        public Pad Pad => Agent.Current.PadAt(PadKind);
        public SelectionSet Begin => Agent.Current.Data.Begin;
        public SelectionSet Current => Agent.Current.Data.Current;
        public SelectionSet Highlight => Agent.Current.Data.Highlight;
        public SelectionSet Selected => Agent.Current.Data.Selected;
        public SelectionSet Clipboard => Agent.Current.Data.Clipboard;

        public int TaskKey { get; }

	    public abstract bool IsValid { get; }
	    public virtual void Initialize() { }

	    protected TaskBase(PadKind padKind)
	    {
		    TaskKey = TaskCounter++;
            PadKind = padKind;
	    }
    }
}
