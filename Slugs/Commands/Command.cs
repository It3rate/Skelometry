using Slugs.Commands.Tasks;

namespace Slugs.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICommand
    {
        int CommandKey { get; }
        List<ITask> Tasks { get; }
        bool IsContinuous { get; }

        DateTime StartTime { get; }
        DateTime EndTime { get; }
        TimeSpan Duration { get; }
        bool IsActive { get; }

        void Execute();
        event EventHandler OnExecute;

        void Unexecute();
	    event EventHandler OnUnexecute;

        void Completed();
        event EventHandler OnCompleted;

	    ICommand Duplicate();
	    bool TryMergeWith(ICommand command);
	    // temporal elements: start time, end time etc
    }

    public abstract class CommandBase : ICommand
    {
	    public int CommandKey { get; }
	    public List<ITask> Tasks { get; } = new List<ITask>();
	    public bool IsContinuous { get; }

	    public DateTime StartTime { get; }
	    public DateTime EndTime { get; }
	    public TimeSpan Duration { get; }
	    public bool IsActive { get; }

	    public event EventHandler OnExecute;
	    public event EventHandler OnUnexecute;
	    public event EventHandler OnCompleted;

	    public void Execute()
	    {
            // remember selection state
            // stamp times
            // run tasks
            // select new element
            throw new NotImplementedException();
	    }
	    public void Unexecute()
	    {
		    throw new NotImplementedException();
	    }

	    public void Completed()
	    {
		    throw new NotImplementedException();
	    }

	    public ICommand Duplicate()
	    {
		    throw new NotImplementedException();
	    }
	    public bool TryMergeWith(ICommand command)
	    {
		    throw new NotImplementedException();
	    }
    }
}
