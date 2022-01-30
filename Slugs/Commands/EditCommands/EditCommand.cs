using Slugs.Entities;

namespace Slugs.Commands.EditCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditCommand : CommandBase
    {
	    new CommandStack<EditCommand> Stack { get; set; }
        public EditCommand(Pad pad) : base(pad){}
    }
}
