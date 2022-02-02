using System;
using System.Collections.Generic;
using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.EditCommands
{
	public interface ICommand
    {
        Pad Pad { get; }
        PadKind PadKind { get; }
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
        void Update(SKPoint point);
        void Unexecute();
        void Completed();

        void AddTask(ITask task); 
        void AddTasks(params ITask[] tasks);
        void AddTaskAndRun(ITask task);
        void AddTasksAndRun(params ITask[] tasks);
        void RunToEnd();

        // event EventHandler OnExecute;
        // event EventHandler OnUpdate;
        // event EventHandler OnUnexecute;
        // event EventHandler OnCompleted;

        ICommand Duplicate();
	    bool TryMergeWith(ICommand command);
	    // temporal elements: start time, end time etc
    }

    public abstract class CommandBase : ICommand
    {
	    public int CommandKey { get; }
	    public Pad Pad { get; }
	    public PadKind PadKind => Pad.PadKind;

	    public ICommandStack Stack { get; set; }

        public IElement ElementAt(int key) => Pad.ElementAt(key);

        // A command is a series of tasks which can run and be modified as they are added if the command is on the stack.
        // They can have a duration, and can be 'scrubbed'.
        public List<ITask> Tasks { get; } = new List<ITask>();
        protected int _taskIndex = 0;
        public bool IsContinuous { get; }
        public bool IsRetainedCommand { get; } = true;

        public DateTime StartTime { get; }
	    public DateTime EndTime { get; }
	    public TimeSpan Duration { get; }
	    public bool IsActive { get; }

	    //public event EventHandler OnExecute;
	    //public event EventHandler OnUpdate;
	    //public event EventHandler OnUnexecute;
	    //public event EventHandler OnCompleted;

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
	    public virtual void Update(SKPoint point)
	    {
	    }
        public virtual void Unexecute()
        {
	        while(_taskIndex > 0)
	        {
		        _taskIndex--;
		        Tasks[_taskIndex].UnRunTask();
	        }
        }

        public virtual void Completed()
	    {
	    }

        public void AddTask(ITask task)
        {
	        Tasks.Add(task);
        }
        public void AddTasks(params ITask[] tasks)
        {
	        foreach (var task in tasks)
	        {
		        Tasks.Add(task);
	        }
        }
        public void AddTaskAndRun(ITask task)
        {
	        Tasks.Add(task);
            RunToEnd();
        }
        public void AddTasksAndRun(params ITask[] tasks)
        {
	        foreach (var task in tasks)
	        {
		        Tasks.Add(task);
	        }
	        RunToEnd();
        }
        public void RunToEnd()
        {
	        while (_taskIndex < Tasks.Count)
	        {
		        Tasks[_taskIndex].RunTask();
		        _taskIndex++;
	        }
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
