namespace Slugs.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CommandStack
    {
	    private readonly List<ICommand> _stack = new List<ICommand>();
	    public int Count() { return _stack.Count; }

	    public bool CanUndo => true;
	    public bool CanRedo => true;
        public void DoCommand(ICommand command) {  }
	    public bool Undo() { return true; }
	    public bool Redo() { return true; }
	    public ICommand Peek() { return null; }

	    public bool CanRepeat => true;
        public List<ICommand> GetRepeatable() { return null; }

        // saving
        // temporal command clock, elements and updates.
    }
}
