using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ML.Probabilistic.Factors;
using OpenTK.Input;
using Vis.Model.Controller;
using Vis.Model.Primitives;
using Vis.Model.UI.Modes;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Vis.Model.Agent
{
    // view: toggle prominent focus or view, multiple pads.
    // hover: select none, node, edge. From focus or view.
    //  If edge is selected, option to move or separate joints, select sections.
    //  If shape is selected, option to move joints and all connections at once.
    // click:
    //  Select hovered attributes. Multiple clicks cycle through options.
    //  Perhaps joints could have a circular flyout of joints/attributes if there are more than one connection.
    // right click: Option to change joint/node/line type
    // drag:
    //  move reference node/edge, all connections update and move with it
    //  move joint: Joint slides along edges, changing the type of joint as needed. Joins update.
    //  disconnect/reconnect node, based on selection and keys
    // up: set end point of new line
    //  reconnect or disconnect dragging node
    //  create new node/edge on selected pad
    //  finalize move, add, delete or change

    // do not delete and recreate nodes, esp as dragging
    // put nodes and edges in indexed tables?
    // state machine for creation by mouse
    // commands for creation, git?
    // attribute has unit, correlation level (potential multiple correlations if not 100%)
    //  correlation is a joint, is exactly the binding of two elements in a 2D relation
    // Resolve if elements are inside a given area, or touch point within tolerance

    public class VisDragAgent : IAgent
    {
	    public VisPad<VisPoint> WorkingPad { get; private set; }
        public VisPad<VisPoint> FocusPad { get; private set; }
        public VisPad<VisStroke> ViewPad { get; private set; }
        private SkiaRenderer _renderer;
        private SkiaRenderer _hoverRender;
        private VisMeasureSkills _skills;
        private IPath unit;

        private List<ModeData> Modes = ModeData.Modes();
	    private UIStatus Status { get; } = new UIStatus();

        public int _unitPixels = 220;

        public VisDragAgent(SkiaRenderer renderer)
        {
            _renderer = renderer;
            _renderer.UnitPixels = _unitPixels;
            _renderer.DrawingComplete += _renderer_DrawingComplete;

            _skills = new VisMeasureSkills();
            WorkingPad = new VisPad<VisPoint>(_renderer.Width, _renderer.Height, PadKind.Working, false);
            FocusPad = new VisPad<VisPoint>(_renderer.Width, _renderer.Height, PadKind.Focus);
            ViewPad = new VisPad<VisStroke>(_renderer.Width, _renderer.Height, PadKind.View);

            _renderer.Pads = new List<IPad>(){ WorkingPad, FocusPad, ViewPad };

            _hoverRender = new SkiaRenderer();
            _hoverRender.GenerateBitmap(_renderer.Width, _renderer.Height);
            _hoverRender.Pads = new List<IPad>(){ ViewPad };
            _hoverRender.DrawTicks = false;
            _hoverRender.Pens.IsHoverMap = true;

            _renderer.Bitmap = _hoverRender.Bitmap;
        }

        private bool SetMouseData(MouseEventArgs e)
        {
	        var result = false;
	        Status.CurrentMouse = e.Button;
            Status.PreviousPositionNorm.UpdateWith(Status.PreviousPositionNorm);
	        Status.PositionNorm.UpdateWith(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);

	        if (_hoverRender != null)
	        {
		        var col = _hoverRender.Bitmap.GetPixel(e.X, e.Y);

		        if (_hoverRender.Pens.IndexOfColor.TryGetValue((uint)col, out var index))
		        {
			        if (index >= 0 && index < ViewPad.Paths.Count)
			        {
				        if (Status.IsHighlightingPath)
				        {
					        Status.HighlightingPath.DisplayState = DisplayState.None;
				        }
				        Status.HighlightingPath = ViewPad.Paths[index];
				        Status.HighlightingPath.DisplayState = DisplayState.Selected;
				        Status.IsHighlightingPath = true;
				        result = true;
			        }
		        }
	        }
	        return result;
        }

        public bool MouseDown(MouseEventArgs e)
        {
            SetMouseData(e);
            switch (Status.UIMode)
            {
	            case UIMode.Select:
		            break;
	            case UIMode.Line:
		            break;
	            case UIMode.Circle:
		            break;
	            case UIMode.SelectUnit:
		            break;
            }
            if (Status.IsHighlightingPoint)
            {
	            var (path, pt) = ViewPad.PathWithNodeNear(Status.HighlightingPoint);
	            if (path != null)
	            {
		            Status.ClickSequenceIndex = 1;
                    Status.ClickSequencePoints.Add((path.StartPoint == pt) ? path.EndPoint : path.StartPoint);
		            if (path is VisStroke stroke)
		            {
			            Status.IsDraggingPoint = true;
			            Status.DraggingPoint = pt;//Status.HighlightingPoint;
			            Status.IsHighlightingPoint = false;
		            }
                }
                else
	            {
		            Status.IsHighlightingPoint = false;
	            }
            }
            
            if(!Status.IsHighlightingPoint)
            {
                var pt = Status.IsDraggingPoint && (Status.HighlightingPoint != null) ? 
		            Status.HighlightingPoint.ClonePoint() :
		            Status.PositionNorm.ClonePoint();
	            Status.ClickSequenceIndex = 1;
	            Status.ClickSequencePoints.Add(pt);
	            _skills.Point(this, pt);
            }

            Status.IsHighlightingPoint = false;
            Status.HighlightingPoint = null;

            return true; // always redraw on mouse down
        }

        public bool MouseMove(MouseEventArgs e)
        {
            var result = SetMouseData(e);
            var p = Status.PositionNorm; //new VisPoint(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);

            if (Status.IsMouseDown)
            {
	            if (Status.IsDraggingPoint)
	            {
		            Status.DraggingPoint.UpdateWith(p);
		            _skills.Line(this, Status.ClickSequencePoints[0], p);
                }
	            else
	            {
	                var path = (Status.UIMode == UIMode.Circle) ?
		                _skills.Circle(this, Status.ClickSequencePoints[0], p) :
		                _skills.Line(this, Status.ClickSequencePoints[0], p);
					path.UnitReference = unit;
                }
                result = true;
            }

            // check if we are over an existing point
            var similarPt = ViewPad.GetSimilar(p);
            if(similarPt is VisPoint sp)
            {
                var rp = new RenderPoint(sp, 4, 2f);
                _skills.Point(this, rp);
                Status.IsHighlightingPoint = true;
                Status.HighlightingPoint = sp;
                result = true;
            }
            else
            {
	            Status.IsHighlightingPoint = false;
	            Status.HighlightingPoint = null;
            }

            if (!result && Status.IsHighlightingPoint)
            {
                Status.IsHighlightingPoint = false;
                Status.HighlightingPoint = null;
                result = true;
            }

	        return result;
        }

        public bool MouseUp(MouseEventArgs e)
        {
	        SetMouseData(e);
            // if dragging point from node, update node to show new value and same reference. Means moving will drag along node path.
            if (Status.UIMode == UIMode.SelectUnit)
            {
	            if (Status.IsHighlightingPath)
	            {
		            unit = Status.HighlightingPath.Element;
		            foreach (var padAttr in ViewPad.Paths)
		            {
			            if(padAttr.Element is VisStroke stroke)
			            {
				            stroke.UnitReference = unit;
			            }
		            }
	            }
            }
            else
            {
		        var endPoint = Status.IsHighlightingPoint ? Status.HighlightingPoint : Status.PositionNorm;// new VisPoint(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);
		        var path = (Status.UIMode == UIMode.Circle) ? 
		            _skills.Circle(this, Status.ClickSequencePoints[0], endPoint, true) :
		            _skills.Line(this, Status.ClickSequencePoints[0], endPoint, true);

	            if (unit == null)
	            {
		            unit = path;
	            }
	            else
	            {
		            path.UnitReference = unit;
	            }
            }


            Status.ClickSequenceIndex = 0;
            Status.ClickSequencePoints.Clear();
            Status.IsDraggingPoint = false;
            Status.DraggingPoint = null;
            //Status.IsHighlightingPath = false;
            //Status.HighlightingPath = null;
            return true;
        }

        private Keys _keyDown = Keys.None;
        public bool KeyDown(KeyEventArgs e)
        {
	        bool result = false;
	        Status.CurrentKey = e.KeyCode;
	        _keyDown = e.KeyCode;
	        
	        if (_keyDown == Keys.L && Status.UIMode != UIMode.Line)
	        {
		        Status.UIMode = UIMode.Line;
		        result = true;
	        }
	        else if (_keyDown == Keys.C && Status.UIMode != UIMode.Circle)
	        {
		        Status.UIMode = UIMode.Circle;
                result = true;
	        }
            else if (_keyDown == Keys.H)
	        {
		        _renderer.ShowBitmap = true;
		        Status.UIDebugState = UIDebugState.ShowHoverBitmap;
                result = true;
	        }
	        else if (_keyDown == Keys.Escape)
	        {
		        Status.UIMode = UIMode.Select;
	        }
	        else if (_keyDown == Keys.U)
	        {
		        Status.UIMode = UIMode.SelectUnit;
	        }
            return result;
        }
        public bool KeyUp(KeyEventArgs e)
        {
	        Status.CurrentKey = Keys.None;
	        Status.UIDebugState = UIDebugState.None;
	        Status.UIState = UIState.None;
			_keyDown = Keys.None;
	        _renderer.ShowBitmap = false;
            return true;
        }

        public void Clear()
        {
            WorkingPad.Clear();
            //FocusPad.Clear();
            //ViewPad.Clear();
        }

        public void Draw()
        {
	        _renderer.PenIndex = Status.IsMouseDown ? 3 : 2;
        }

        private void _renderer_DrawingComplete(object sender, EventArgs e)
        {
	        _hoverRender?.DrawOnBitmap();
	        Clear();
        }



    }
}
