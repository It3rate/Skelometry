using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Vis.Model.Agent;
using Vis.Model.Controller;

namespace Vis.Model.UI.Modes
{
	public class ModeData
    {
	    public string Name { get; }
	    public Keys Keys { get; }
	    public UIMode UIModeChange { get; }
	    public UIState UIStateChange { get; }
	    public UIDisplay UIDisplayChange { get; }

        public bool ActiveAfterSelect { get; }
        public bool ActiveAfterCommand { get; }
        public bool ActiveAfterPress { get; }
        public Bitmap Icon { get; }
	    public string HelpBlurb { get; }
	    public Bitmap HelpImage { get; }

	    public bool IsActive { get; private set; }
	    public PadKind[] AllowedPadKinds { get; }

        public ModeData(string name, UIMode uiModeChange, UIState uiStateChange, UIDisplay uiDisplayChangeChange, Keys keys, bool activeAfterSelect = true, bool activeAfterCommand = true, bool activeAfterPress = true)
	    {
		    Name = name;
		    Keys = keys;
		    UIModeChange = uiModeChange;
		    UIStateChange = uiStateChange;
		    UIDisplayChange = uiDisplayChangeChange;
            ActiveAfterSelect = activeAfterSelect;
		    ActiveAfterCommand = activeAfterCommand;
		    ActiveAfterPress = activeAfterPress;
	    }

        public bool CanActivate(UIStatus status)
	    {
		    return true;
	    }

	    public static List<ModeData> Modes()
	    {
		    return new List<ModeData>()
		    {
			    new ModeData("Focus Pad", UIMode.None, UIState.FocusPad, UIDisplay.None, 
				    Keys.F, true, true),
			    new ModeData("View Pad", UIMode.None, UIState.ViewPad, UIDisplay.None, 
				    Keys.V, true, true),

                new ModeData("Select", UIMode.Select, UIState.None, UIDisplay.None,
	                Keys.Escape, true, true),
			    new ModeData("Line", UIMode.Line, UIState.None, UIDisplay.None, 
				    Keys.L, true, true),
                new ModeData("Circle", UIMode.Circle, UIState.None, UIDisplay.None,
                    Keys.C, true, true),
                new ModeData("Select Unit", UIMode.SelectUnit, UIState.None, UIDisplay.None, 
	                Keys.U, false, false, false),

                new ModeData("Snap To Point", UIMode.None, UIState.SnapToPoint, UIDisplay.None, 
	                Keys.S, true, true),
                new ModeData("Snap To Path", UIMode.None, UIState.SnapToPath, UIDisplay.None, 
	                Keys.D, true, true),
                new ModeData("Constrain Angle", UIMode.None, UIState.ConstrainAngle, UIDisplay.None, 
	                Keys.Control, false, false, false),
                new ModeData("Show Ticks", UIMode.None, UIState.None, UIDisplay.ShowTicks,
	                Keys.R, true, true),
                new ModeData("Show Values", UIMode.None, UIState.None, UIDisplay.ShowValues,
	                Keys.V, true, true),
                new ModeData("Show Debug Info", UIMode.None, UIState.None, UIDisplay.ShowDebugInfo,
	                Keys.OemQuestion, false, false, false),
                new ModeData("Toggle Focus Pad", UIMode.None, UIState.FocusPad, UIDisplay.None,
	                Keys.D1, true, true),
                new ModeData("Toggle View Pad", UIMode.None, UIState.ViewPad, UIDisplay.None,
	                Keys.D2, true, true),
            };
	    }
    }
}
