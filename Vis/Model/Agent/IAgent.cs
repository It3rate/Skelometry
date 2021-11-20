using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Primitives;
using Vis.Model.UI;

namespace Vis.Model.Agent
{
    public interface IAgent
    {
        VisPad WorkingPad { get; }
        VisPad FocusPad { get; }
        VisPad ViewPad { get; }
        UIStatus Status { get; }

        void Clear();
        void Draw();
    }
}
