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
    }

    public abstract class TaskBase : ITask
    {
	    public PadKind PadKind { get; }
	    public int TaskKey { get; }
    }
}
