using Slugs.Agents;
using Slugs.Commands.Tasks;
using Slugs.Entities;

namespace Slugs.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICommand
    {
        Pad Pad { get; }
        ICommandStack Stack { get; set; }

        int CommandKey { get; }
        List<ITask> Tasks { get; }
        bool IsContinuous { get; }
        bool IsRetainedCommand { get; }

        IElement ElementAt(int key);

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
	    public Pad Pad { get; }

	    public ICommandStack Stack { get; set; }

        public IElement ElementAt(int key) => Pad.ElementAt(key);

        // A command is a series of tasks which can run and be modified as they are added if the command is on the stack.
        // They can have a duration, and can be 'scrubbed'.
        public List<ITask> Tasks { get; } = new List<ITask>();
        public bool IsContinuous { get; }
        public bool IsRetainedCommand { get; } = true;

        public DateTime StartTime { get; }
	    public DateTime EndTime { get; }
	    public TimeSpan Duration { get; }
	    public bool IsActive { get; }

	    public event EventHandler OnExecute;
	    public event EventHandler OnUnexecute;
	    public event EventHandler OnCompleted;

	    public CommandBase(Pad pad)
	    {
		    Pad = pad;
	    }

	    public virtual void Execute()
	    {
            // remember selection state
            // stamp times
            // run tasks
            // select new element
            //foreach (var task in Tasks)
            //{
	           // task.RunTask();
            //}
	    }
        public virtual void Unexecute()
        {
	        foreach (var task in Tasks)
	        {
		        task.UnRunTask();
	        }
        }

        public virtual void Completed()
	    {
	    }

        public virtual ICommand Duplicate()
        {
	        return null;
        }
        public virtual bool TryMergeWith(ICommand command)
        {
	        return false;
        }
    }
}
