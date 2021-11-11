using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Vis.Model.Controller;

namespace Vis.Model.UI.Modes
{
	public class ModeData
    {
	    public string Name { get; }
        public Keys Keys { get; }
        public bool StaysActiveAfterSelect { get; }
        public bool StaysActiveAfterCommand { get; }
        public bool IsModeEnhancement { get; }
        public bool IsToggle { get; }
        public Bitmap Icon { get; }
	    public string HelpBlurb { get; }
	    public Bitmap HelpImage { get; }

	    public ModeData(string name, Keys keys, bool staysActiveAfterSelect, bool staysActiveAfterCommand, bool isModeEnhancement, bool isToggle)
	    {
		    Name = name;
		    Keys = keys;
		    StaysActiveAfterSelect = staysActiveAfterSelect;
		    StaysActiveAfterCommand = staysActiveAfterCommand;
		    IsModeEnhancement = isModeEnhancement;
		    IsToggle = isToggle;
	    }

        public bool IsActiveWith(UIStatus status)
	    {
		    return true;
	    }

	    public static List<ModeData> Modes()
	    {
		    return new List<ModeData>()
		    {
			    new ModeData("Focus Pad", Keys.NumPad1, true, true, true, false),
			    new ModeData("View Pad", Keys.NumPad2, true, true, true, false),

                new ModeData("Select", Keys.Escape, true, true, false, false),
			    new ModeData("Line", Keys.L, true, true, false, false),
                new ModeData("Circle", Keys.C, true, true, false, false),
                new ModeData("Select Unit", Keys.U, false, false, false, false),

                new ModeData("Snap To Point", Keys.S, true, true, true, true),
                new ModeData("Show Units", Keys.I, true, true, true, true),
                new ModeData("Constrain Angle", Keys.Control, false, false, true, false),
                new ModeData("Show Hover Bitmap", Keys.H, false, false, true, false),
            };
	    }
    }
}
