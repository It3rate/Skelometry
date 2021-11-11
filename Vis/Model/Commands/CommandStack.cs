namespace Vis.Model.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CommandStack
    {
        public List<ICommand> Commands = new List<ICommand>();

        public List<ICommand> ToAdd = new List<ICommand>();
        public List<ICommand> ToDelete = new List<ICommand>();
    }
}
