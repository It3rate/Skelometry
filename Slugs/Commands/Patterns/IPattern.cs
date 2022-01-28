using System.Windows.Forms;
using Slugs.Agents;
using Slugs.Entities;
using Slugs.Input;

namespace Slugs.Commands.Patterns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // recipes, data locations and constraints for possible commands.
    public interface IPattern
    {
        Pad SourcePad { get; }
        string Name { get; }

        bool IsValid { get; }
        Keys Keys { get; }

        ICommand CreateCommand();
        void AddToCommand(ICommand command);
    }

}
