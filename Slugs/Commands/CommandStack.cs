﻿using Slugs.Agents;
using Slugs.Commands.EditCommands;

namespace Slugs.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICommandStack
    {
	    Agent Agent { get; }
	    bool CanUndo { get; }
        bool CanRedo { get; }
        int RedoSize { get; }
        bool CanRepeat { get; }

        ICommand Do(ICommand command);
        ICommand PreviousCommand();
        bool Undo();
        void UndoAll();
        void UndoToIndex(int index);
        bool Redo();
        void RedoAll();
        void RedoToIndex(int index);
        void Clear();
    }
    public class CommandStack<TCommand> : ICommandStack where TCommand : ICommand
    {
	    public Agent Agent { get; }
    
	    private readonly List<TCommand> _stack = new List<TCommand>(4096);
	    private int _stackIndex = 0; // the pointer to the next insertion index

	    private readonly List<TCommand> _toAdd = new List<TCommand>();
	    private readonly List<TCommand> _toRemove = new List<TCommand>();

	    public bool CanUndo => _stackIndex > -0;
	    public bool CanRedo => RedoSize > 0;
	    public int RedoSize => _stack.Count - (_stackIndex + 1);

	    public CommandStack(Agent agent)
	    {
		    Agent = agent;
	    }

	    public ICommand Do(ICommand command)
	    {
            // should always pick up correct type of command override for Do
		    throw new ArgumentException("command must match generic type of command stack");
	    }

        public TCommand Do(TCommand command)
	    {
		    command.Stack = (ICommandStack)this;
		    if (command.IsRetainedCommand)
		    {
			    RemoveRedoCommands();
			    // try merging with previous command
			    AddAndExecuteCommand(command);
		    }
		    return command;
	    }

	    public ICommand PreviousCommand() => CanUndo ? (ICommand)_stack[_stackIndex - 1] : null;

        public bool Undo()
	    {
		    var result = false;
		    if (CanUndo)
		    {
			    _stackIndex--;
			    _stack[_stackIndex].Unexecute();
			    result = true;
		    }

		    return result;
	    }

	    public void UndoAll()
	    {
		    while (CanUndo)
		    {
			    Undo();
		    }
	    }

	    public void UndoToIndex(int index)
	    {
		    while (_stackIndex >= index)
		    {
			    Undo();
		    }
	    }

	    public bool Redo()
	    {
		    var result = false;
		    if (CanRedo)
		    {
			    _stack[_stackIndex].Execute();
			    _stackIndex++;
			    result = true;
		    }

		    return result;
	    }

	    public void RedoAll()
	    {
		    while (CanRedo)
		    {
			    Redo();
		    }
	    }

	    public void RedoToIndex(int index)
	    {
		    while (_stackIndex < index)
		    {
			    Redo();
		    }
	    }

	    public void Clear()
	    {
		    _stack.Clear();
		    _toAdd.Clear();
		    _toRemove.Clear();
		    _stackIndex = 0;
	    }

	    public bool CanRepeat => true;

	    public List<TCommand> GetRepeatable()
	    {
		    return null;
	    }

	    private void AddAndExecuteCommand(TCommand command)
	    {
		    _stack.Add(command);
		    _stackIndex++;
		    command.Execute();
	    }

	    private void RemoveRedoCommands()
	    {
		    if (CanRedo)
		    {
			    _stack.RemoveRange(_stackIndex + 1, RedoSize);
		    }
	    }

	    // saving
	    // temporal command clock, elements and updates.
    }
}