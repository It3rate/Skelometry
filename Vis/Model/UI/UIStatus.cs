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
    using Vis.Model.Connections;

    public class UIStatus
    {
	    public List<IPad> Pads { get; }

        public UIMode Mode { get; set; }
	    public UIMode PreviousMode { get; set; }
        public UIState State { get; set; }
	    public Keys CurrentKey { get; set; }
	    public MouseButtons CurrentMouse { get; set; }
	    public PadKind CurrentPadKind { get; set; }

		private bool _needsUpdate;
		public bool NeedsUpdate => _needsUpdate;

		public VisPoint PositionNorm { get; } = new VisPoint(0, 0);
		public VisPoint PreviousPositionNorm { get; } = new VisPoint(0, 0);

		public int ClickSequenceIndex => ClickSequencePoints.Count;
	    public List<VisPoint> ClickSequencePoints { get; } = new List<VisPoint>();

		private VisPoint _highlightingPoint;
	    public bool IsHighlightingPoint => _highlightingPoint != null;
		public VisPoint HighlightingPoint { get => _highlightingPoint; set => _highlightingPoint = value; }

		private PadAttributes<VisStroke> _highlightingPath;
		public bool IsHighlightingPath => _highlightingPath != null;
		public PadAttributes<VisStroke> HighlightingPath
		{
			get => _highlightingPath;
			set
			{
				if(_highlightingPath != null && _highlightingPath.DisplayState != DisplayState.Selected)
                {
					_highlightingPath.DisplayState = DisplayState.None;
                }
				_highlightingPath = value;
				if (_highlightingPath != null && _highlightingPath.DisplayState != DisplayState.Selected)
				{
					_highlightingPath.DisplayState = DisplayState.Highlighting;
				}
				_needsUpdate = true;
			}
		}

		private PadAttributes<VisStroke> _selectedPath;
		public bool HasSelectedPath => _selectedPath != null;
		public PadAttributes<VisStroke> SelectedPath
		{
			get => _selectedPath;
			set
			{
				if (_selectedPath != null)
				{
					_selectedPath.DisplayState = DisplayState.None;
				}
				_selectedPath = value;
				if (_selectedPath != null)
				{
					_selectedPath.DisplayState = DisplayState.Selected;
				}
				_needsUpdate = true;
			}
		}

		private PadAttributes<VisStroke> _unitPath;
		public bool HasUnitPath => _unitPath != null;
		public PadAttributes<VisStroke> UnitPath
		{
			get => _unitPath;
			set
			{
				if (_unitPath != null)
				{
					_unitPath.CorrelationState = CorrelationState.HasUnit;
				}
				_unitPath = value;
				if (_unitPath != null)
				{
					_unitPath.CorrelationState = CorrelationState.IsUnit;
				}
				_needsUpdate = true;
			}
		}

		public bool IsDraggingPoint => DraggingPoint != null;
	    public VisPoint DraggingPoint { get; set; }
		public VisNode DraggingNode { get; set; }


	    public bool IsMouseDown => CurrentMouse == MouseButtons.Left;

	    public UIStatus(params IPad[] pads)
	    {
		    Pads = pads.ToList();
	    }

		public void HighlightedPathToSelected()
		{
			var hp = HighlightingPath;
			HighlightingPath = null;
			SelectedPath = hp;
			_needsUpdate = true;
		}
		public void HighlightedPathToUnit()
		{
			var unitPath = UnitPath;
			HighlightingPath = null;
			UnitPath = unitPath;
			_needsUpdate = true;
		}
		public void StartDraw()
		{
			_needsUpdate = false;
		}
	}
}
