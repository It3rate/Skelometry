using System.Windows.Forms;
using Vis.Model.Agent;
using Vis.Model.Controller;
using Vis.Model.Primitives;

namespace Vis.Model.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UIStatus
    {
	    public List<IPad> Pads { get; }

        public UIMode Mode { get; set; }
	    public UIMode PreviousMode { get; set; }
        public UIState State { get; set; }
	    public Keys CurrentKey { get; set; }
	    public MouseButtons CurrentMouse { get; set; }
	    public PadKind CurrentPadKind { get; set; }

	    public int ClickSequenceIndex => ClickSequencePoints.Count;
	    public List<VisPoint> ClickSequencePoints { get; } = new List<VisPoint>();

	    public bool IsHighlightingPoint => HighlightingPoint != null;
	    public VisPoint HighlightingPoint { get; set; }

	    public bool IsHighlightingPath => HighlightingPath != null;
	    public PadAttributes<VisStroke> HighlightingPath { get; set; }
	    public PadAttributes<VisStroke> SelectedPath { get; set; }

	    public bool HasUnitPath => UnitPath != null;
	    public PadAttributes<VisStroke> UnitPath { get; set; }

        public bool IsDraggingPoint => DraggingPoint != null;
	    public VisPoint DraggingPoint { get; set; }

	    public VisPoint PositionNorm { get; } = new VisPoint(0, 0);
	    public VisPoint PreviousPositionNorm { get; } = new VisPoint(0, 0);

	    public bool IsMouseDown => CurrentMouse == MouseButtons.Left;

	    public UIStatus(params IPad[] pads)
	    {
		    Pads = pads.ToList();
	    }
    }
}
