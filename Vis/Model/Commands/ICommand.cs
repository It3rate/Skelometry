namespace Vis.Model.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    // SelectPad, Select, Add (def/inst?), RemoveSel, duplicateSel, Constrain, SetProp, Move (path, node),
    // LockNodeToPath, Add, Stretch, Reorder(account for constraints), 
    // MultiCommand, Group, Repeat, SetMode, 
    // Touch/Drag/Key, 
    public interface ICommand
    {
        int CommandID { get; }
        int StartTime { get; }
        int EndTime { get; }
        bool AddToStack { get; }
        bool AttemptMerge { get; }

	    void Execute();
	    void Unexecute();
	    void OnComplete();
    }
}
