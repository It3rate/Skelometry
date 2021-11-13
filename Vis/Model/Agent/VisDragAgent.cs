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
using Vis.Model.UI;
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

        private List<ModeData> ModeMap = ModeData.Modes();
	    private UIStatus Status { get; }

        public int _unitPixels = 220;

        public VisDragAgent(SkiaRenderer renderer)
        {
            _renderer = renderer;
            _renderer.UnitPixels = _unitPixels;
            _renderer.DrawingComplete += _renderer_DrawingComplete;

            WorkingPad = new VisPad<VisPoint>(_renderer.Width, _renderer.Height, PadKind.Working, false);
            FocusPad = new VisPad<VisPoint>(_renderer.Width, _renderer.Height, PadKind.Focus);
            ViewPad = new VisPad<VisStroke>(_renderer.Width, _renderer.Height, PadKind.View);
            Status = new UIStatus(WorkingPad, FocusPad, ViewPad);
            _renderer.Status = Status;

            _skills = new VisMeasureSkills();

            _hoverRender = new SkiaRenderer();
            _hoverRender.Status = new UIStatus(ViewPad);
            _hoverRender.GenerateBitmap(_renderer.Width, _renderer.Height);
            _hoverRender.DrawTicks = false;
            _hoverRender.Pens.IsHoverMap = true;

            _renderer.Bitmap = _hoverRender.Bitmap;

            Status.Mode = UIMode.Line;
            //Status.PreviousMode = UIMode.Line;
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
				        var newPath = ViewPad.Paths[index];
				        if (Status.HighlightingPath != null && Status.HighlightingPath != newPath)
				        {
					        Status.HighlightingPath.DisplayState = DisplayState.None;
				        }

				        if (newPath.DisplayState != DisplayState.Selected)
				        {
					        Status.HighlightingPath = newPath;
				        }
				        result = true;
			        }
		        }
	        }
	        return result;
        }

        public bool MouseDown(MouseEventArgs e)
        {
            SetMouseData(e);
            switch (Status.Mode)
            {
	            case UIMode.Select:
		            break;
	            case UIMode.SelectUnit:
		            break;
	            case UIMode.Line:
	            case UIMode.Circle:
		            bool isHighlightingPoint = Status.IsHighlightingPoint;
		            if (isHighlightingPoint)
		            {
			            var (path, pt) = ViewPad.PathWithNodeNear(Status.HighlightingPoint);
			            if (path != null)
			            {
				            Status.ClickSequenceIndex = 1;
				            Status.ClickSequencePoints.Add((path.StartPoint == pt) ? path.EndPoint : path.StartPoint);
				            if (path is VisStroke stroke)
				            {
					            Status.DraggingPoint = pt;//Status.HighlightingPoint;
					            isHighlightingPoint = false;
				            }
			            }
			            else
			            {
				            isHighlightingPoint = false;
                        }
		            }

		            if (!isHighlightingPoint)
		            {
			            var pt = Status.IsDraggingPoint && Status.IsHighlightingPoint ? Status.HighlightingPoint.ClonePoint() : Status.PositionNorm.ClonePoint();
			            Status.ClickSequenceIndex = 1;
			            Status.ClickSequencePoints.Add(pt);
			            _skills.Point(this, pt);
		            }
                    break;
            }

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
	            else if(Status.Mode == UIMode.Line || Status.Mode == UIMode.Circle)
	            {
	                var path = (Status.Mode == UIMode.Circle) ?
		                _skills.Circle(this, Status.ClickSequencePoints[0], p) :
		                _skills.Line(this, Status.ClickSequencePoints[0], p);
					path.UnitReference = unit;
                }
                result = true;
            }

            if (Status.IsHighlightingPath && Status.HighlightingPath.DisplayState != DisplayState.Selected)
            {
				Status.HighlightingPath.DisplayState = DisplayState.Hovering;
            }

            // check if we are over an existing point
            var similarPt = ViewPad.GetSimilar(p);
            if(similarPt is VisPoint sp)
            {
                var rp = new RenderPoint(sp, 4, 2f);
                _skills.Point(this, rp);
                Status.HighlightingPoint = sp;
                result = true;
            }
            else
            {
	            Status.HighlightingPoint = null;
            }

            if (!result && Status.IsHighlightingPoint)
            {
                Status.HighlightingPoint = null;
                result = true;
            }

	        return result;
        }

        public bool MouseUp(MouseEventArgs e)
        {
	        SetMouseData(e);
            // if dragging point from node, update node to show new value and same reference. Means moving will drag along node path.
            if (Status.Mode == UIMode.Select)
            {
	            if (Status.SelectedPath != Status.HighlightingPath)
	            {
		            if (Status.SelectedPath != null)
		            {
			            Status.SelectedPath.DisplayState = DisplayState.None;
		            }
		            Status.SelectedPath = Status.HighlightingPath;
		            Status.HighlightingPath = null;
                    Status.SelectedPath.DisplayState = DisplayState.Selected;
	            }
            }
            else if (Status.Mode == UIMode.SelectUnit)
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
		        var path = (Status.Mode == UIMode.Circle) ? 
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
            Status.DraggingPoint = null;
            //Status.IsHighlightingPath = false;
            //Status.HighlightingPath = null;
            return true;
        }

        public bool KeyDown(KeyEventArgs e)
        {
	        bool result = false;
	        Status.CurrentKey = e.KeyCode;
            foreach (var modeData in ModeMap)
	        {
		        if (modeData.Keys == Status.CurrentKey)
		        {
			        if (Status.Mode != modeData.ModeChange && modeData.ModeChange != UIMode.None)
			        {
				        Status.PreviousMode = Status.Mode;  
				        Status.Mode = modeData.ModeChange;
				        result = true;
			        }

			        if (Status.State != modeData.StateChange && modeData.StateChange != UIState.None)
			        {
				        if (modeData.ActiveAfterPress)
				        {
					        Status.State ^= modeData.StateChange;
                        }
				        else
				        {
					        Status.State |= modeData.StateChange;
                        } 
				        result = true;
			        }
			        break;
		        }
	        }

	        _renderer.ShowBitmap = (Status.State & UIState.ShowDebugInfo) > 0;
            return result;
        }
        public bool KeyUp(KeyEventArgs e)
        {
	        foreach (var modeData in ModeMap)
	        {
		        if (modeData.StateChange != UIState.None && !modeData.ActiveAfterPress)
		        {
			        Status.State &= ~modeData.StateChange;
		        }

		        if (Status.Mode == modeData.ModeChange && !modeData.ActiveAfterPress)
		        {
			        Status.Mode = Status.PreviousMode;
					Status.PreviousMode = UIMode.None;
		        }
            }

	        Status.CurrentKey = Keys.None;
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
