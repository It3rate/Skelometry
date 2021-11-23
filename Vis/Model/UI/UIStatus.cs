using System.Windows.Forms;
using Vis.Model.Agent;
using Vis.Model.Controller;
using Vis.Model.Primitives;
using Vis.Model.Render;

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
        public UIDisplay Display { get; set; }
        public Keys CurrentKey { get; set; }
	    public MouseButtons CurrentMouse { get; set; }
	    public PadKind CurrentPadKind { get; set; }

		private bool _needsUpdate;
		public bool NeedsUpdate => _needsUpdate;

		public VisPoint PositionNorm { get; } = new VisPoint(0, 0);
        public VisPoint PreviousPositionNorm { get; } = new VisPoint(0, 0);

		public int ClickSequenceIndex => ClickSequencePoints.Count;
		public List<VisPoint> ClickSequencePoints { get; } = new List<VisPoint>();
		public List<VisNode> ClickNodes { get; } = new List<VisNode>();

		public bool HasValidClickNodes()
		{
			bool result = false;
			foreach (var clickNode in ClickNodes)
			{
				if (!clickNode.IsEmpty)
				{
					result = true;
					break;
				}
			}
			return result;
		}

        private VisPoint _highlightingPoint;
	    public bool IsHighlightingPoint => _highlightingPoint != null;
		public VisPoint HighlightingPoint { get => _highlightingPoint; set => _highlightingPoint = value; }

		private ElementRecord _highlightingPath;
		public bool IsHighlightingPath => _highlightingPath != null;
		public ElementRecord HighlightingPath
		{
			get => _highlightingPath;
			set
			{
				if(_highlightingPath != null)
                {
					_highlightingPath.ElementStyle &= ~ElementStyle.Highlighting;
                }
				_highlightingPath = value;
				if (_highlightingPath != null)// && _highlightingPath.ElementState != ElementState.Selected)
				{
					_highlightingPath.ElementStyle |= ElementStyle.Highlighting;
				}
				_needsUpdate = true;
			}
		}

		private ElementRecord _selectedPath;
		public bool HasSelectedPath => _selectedPath != null;
		public ElementRecord SelectedPath
		{
			get => _selectedPath;
			set
			{
				if (_selectedPath != null)
				{
					_selectedPath.ElementState = ElementState.None;
				}
				_selectedPath = value;
				if (_selectedPath != null)
				{
					_selectedPath.ElementState |= ElementState.Selected;
				}
				_needsUpdate = true;
			}
		}

		private ElementRecord _unitPath;
		public bool HasUnitPath => _unitPath != null;
		public ElementRecord UnitPath
		{
			get => _unitPath;
			set
			{
				if (_unitPath != null)
				{
					_unitPath.ElementLinkage = ElementLinkage.HasUnit;
				}
				_unitPath = value;
				if (_unitPath != null)
				{
					_unitPath.ElementLinkage = ElementLinkage.IsUnit;
				}
				_needsUpdate = true;
			}
		}

		public bool IsDraggingPoint => DraggingPoint != null;
	    public VisPoint DraggingPoint { get; set; }
		public VisNode DraggingNode { get; set; }


	    public bool IsMouseDown => CurrentMouse == MouseButtons.Left;
	    public VisPoint PositionMouseDown { get; set; }

        public UIStatus(params IPad[] pads)
	    {
		    Pads = pads.ToList();
		    State = UIState.FocusPad | UIState.ViewPad;
		    Display = UIDisplay.ShowFocusPad | UIDisplay.ShowViewPad;
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
			var unitPath = HighlightingPath;
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
