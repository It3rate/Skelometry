﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.ML.Probabilistic.Factors;
using OpenTK.Input;
using Vis.Model.Controller;
using Vis.Model.Primitives;
using Vis.Model.Render;
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
	    public VisPad WorkingPad { get; private set; }
        public VisPad FocusPad { get; private set; }
        public VisPad ViewPad { get; private set; }
        private SkiaRenderer _renderer;
        private SkiaRenderer _hoverRender;
        private VisMeasureSkills _skills;

        private List<ModeData> ModeMap = ModeData.Modes();
	    public UIStatus Status { get; }

        public int _unitPixels = 220;

        public VisDragAgent(SkiaRenderer renderer)
        {
            _renderer = renderer;
            _renderer.UnitPixels = _unitPixels;
            _renderer.DrawingComplete += _renderer_DrawingComplete;

            WorkingPad = new VisPad(typeof(VisPoint), _renderer.Width, _renderer.Height, PadKind.Working, false);
            FocusPad = new VisPad(typeof(VisPoint), _renderer.Width, _renderer.Height, PadKind.Focus);
            ViewPad = new VisPad(typeof(VisStroke), _renderer.Width, _renderer.Height, PadKind.View);
            Status = new UIStatus(FocusPad, ViewPad, WorkingPad);
            _renderer.Status = Status;

            _skills = new VisMeasureSkills();

            _hoverRender = new SkiaRenderer();
            _hoverRender.Status = new UIStatus(FocusPad,ViewPad);
            _hoverRender.GenerateBitmap(_renderer.Width, _renderer.Height);
            _hoverRender.DrawTicks = false;
            _hoverRender.Pens.IsHoverMap = true;

            _renderer.Bitmap = _hoverRender.Bitmap;

            Status.Mode = UIMode.Line;
            //Status.PreviousMode = UIModeChange.Line;
        }

        private bool SetMouseData(MouseEventArgs e)
        {
	        var result = false;
			Status.StartDraw();
	        Status.CurrentMouse = e.Button;
            Status.PreviousPositionNorm.UpdateWith(Status.PreviousPositionNorm);
	        Status.PositionNorm.UpdateWith(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);

	        if (_hoverRender != null && e.Y >= 0  && e.Y < _hoverRender.Bitmap.Height && e.X >= 0 && e.X < _hoverRender.Bitmap.Width)
	        {
		        var col = _hoverRender.Bitmap.GetPixel(e.X, e.Y);

		        if (_hoverRender.Pens.IndexOfColor.TryGetValue((uint)col, out var index))
		        {
			        var pad = _hoverRender.Status.Pads[0];
			        if (index >= 0 && index < pad.Paths.Count && pad.Paths[index] is PadAttributes padAttrs)
			        {
				        Status.HighlightingPath = padAttrs;
				        result = true;
			        }
		        }
	        }

	        if (!result)
	        {
		        Status.HighlightingPath = null;
	        }
	        return result;
		}

		private void SetHighlighting()
		{
			// check if we are over an existing point
			var similarPt = ViewPad.GetSimilar(Status.PositionNorm);
			if (similarPt == null)
			{
				similarPt = FocusPad.GetSimilar(Status.PositionNorm);
            }

			if (similarPt is VisPoint sp)
			{
				var rp = new RenderPoint(sp, 4, 2f);
				_skills.Point(this, rp);
				Status.HighlightingPoint = sp;
			}
			else if (Status.IsHighlightingPoint)
			{
				Status.HighlightingPoint = null;
			}
		}

		public bool MouseDown(MouseEventArgs e)
        {
            SetMouseData(e);
            Status.PositionMouseDown = Status.PositionNorm.ClonePoint();
            switch (Status.Mode)
            {
	            case UIMode.Select:
					if (Status.IsHighlightingPoint)
					{
						var (nearPath, nearPt) = ViewPad.PathWithNodeNear(Status.HighlightingPoint);
						if (nearPath != null)
						{
							bool isStartPoint = nearPath.StartPoint == nearPt;
							Status.ClickSequencePoints.Add(isStartPoint ? nearPath.EndPoint : nearPath.StartPoint);
							if (nearPath is VisStroke stroke)
							{
								Status.DraggingNode = isStartPoint ? stroke.StartNode : stroke.EndNode;
								Status.DraggingPoint = nearPt;
								Status.HighlightedPathToSelected();
                            }
						}
					}
					else if (Status.IsHighlightingPath)
		            {
						Status.HighlightedPathToSelected();
		            }
                    break;

	            case UIMode.SelectUnit:
		            break;

	            case UIMode.Line:
	            case UIMode.Circle:
	            case UIMode.ParallelLines:
                    var pt = Status.IsHighlightingPoint ? Status.HighlightingPoint.ClonePoint() : Status.PositionNorm.ClonePoint();
					Status.ClickSequencePoints.Add(pt);
					_skills.Point(this, pt);
					break;
            }

            Status.HighlightingPoint = null;

			SetHighlighting();
			return true; // always redraw on mouse down
        }

        public bool MouseMove(MouseEventArgs e)
        {
            var result = SetMouseData(e);
			SetHighlighting();
			switch (Status.Mode)
			{
				case UIMode.Select:
					if (Status.IsDraggingPoint)
					{
						Status.DraggingPoint.UpdateWith(Status.PositionNorm);
						_skills.Line(this, Status.ClickSequencePoints[0], Status.PositionNorm);
						result = true;
					}
                    else if (Status.PositionMouseDown != null && Status.HasSelectedPath)
					{
						var poly = Status.SelectedPath.Element.GetPolyline();
						var dif = Status.PositionNorm.Subtract(Status.PositionMouseDown);
						poly.AddOffset(dif);
						_skills.Polyline(this, poly);
						result = true;
					}
					break;
				case UIMode.SelectUnit:
					break;
				case UIMode.Line:
				case UIMode.Circle:
				case UIMode.ParallelLines:
					if (Status.IsMouseDown)
					{
						var endPoint = Status.PositionNorm;
						if (Status.IsHighlightingPoint)
						{
							endPoint = Status.HighlightingPoint;
						}
						else if (Status.IsHighlightingPath)
						{
							endPoint = Status.HighlightingPath.Element.ProjectPointOnto(Status.PositionNorm);
						}

						var paths = _skills.AddElements(Status.Mode, this, Status.ClickSequencePoints[0], endPoint);
						if (Status.Mode != UIMode.ParallelLines)
						{
							foreach (var path in paths)
							{
								path.UnitReference = (IPath)Status.UnitPath?.Element;
	                        }
						}
						result = true;
					}
					break;
            }
   
            return result | Status.NeedsUpdate;
        }

        public bool MouseUp(MouseEventArgs e)
        {
	        SetMouseData(e);
            // if dragging point from node, update node to show new value and same reference. Means moving will drag along node path.
            switch (Status.Mode)
            {
                case UIMode.SelectUnit:
		            if (Status.IsHighlightingPath)
		            {
						Status.HighlightedPathToUnit();
                        foreach (var padAttr in ViewPad.Paths)
			            {
				            if(padAttr.Element is VisStroke stroke)
				            {
					            stroke.UnitReference = (IPath)Status.UnitPath.Element;
				            }
			            }
		            }
		            break;
                case UIMode.Select:
                    if (Status.IsDraggingPoint)
					{
						var nodeRef = Status.DraggingNode.Reference;
						bool isDraggingStartPoint = Status.ClickSequencePoints[0].IsNear(nodeRef.EndPoint);
						var pt = isDraggingStartPoint ? nodeRef.StartPoint : nodeRef.EndPoint;
						var targPt = Status.IsHighlightingPoint ? Status.HighlightingPoint : Status.PositionNorm;
						pt.UpdateWith(targPt);
						ViewPad.RecalculateAll();
                        //Status.SelectedPath.Element.Recalculate();
                    }
                    else if (Status.PositionMouseDown != null && Status.HasSelectedPath)
                    {
	                    var dif = Status.PositionNorm.Subtract(Status.PositionMouseDown);
	                    var stroke = Status.SelectedPath.Element;
	                    stroke.AddOffset(dif.X, dif.Y);
	                    ViewPad.RecalculateAll();
                    }
                    break;
                case UIMode.Line:
                case UIMode.Circle:
                case UIMode.ParallelLines:
                    var endPoint = Status.PositionNorm;
	                if (Status.IsHighlightingPoint)
	                {
		                endPoint = Status.HighlightingPoint;
	                }
	                else if (Status.IsHighlightingPath)
	                {
		                endPoint = Status.HighlightingPath.Element.ProjectPointOnto(Status.PositionNorm);
	                }

	                var startPoint = Status.ClickSequencePoints[0];
	                var dist = startPoint.SquaredDistanceTo(endPoint);
	                if (dist > startPoint.NearThreshold)
	                {
		                var paths = _skills.AddElements(Status.Mode, this, Status.ClickSequencePoints[0], endPoint, true);
		                if (Status.UnitPath == null && paths.Length == 1)
		                {
			                var attr = ViewPad.GetPadAttributesFor(paths[0]);
			                if (attr != null)
			                {
				                Status.UnitPath = attr;
				                attr.ElementLinkage = ElementLinkage.IsUnit;
				                Status.SelectedPath = attr;
                            }
                        }

		                if(Status.Mode != UIMode.ParallelLines) // add this flag to mode data
		                {
			                foreach (var path in paths)
			                {
				                path.UnitReference = (IPath)Status.UnitPath?.Element;
			                }
		                }
	                }
                    break;
            }
            Status.PositionMouseDown = null;
            Status.ClickSequencePoints.Clear();
            Status.DraggingPoint = null;
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
			        if (Status.Mode != modeData.UIModeChange && modeData.UIModeChange != UIMode.None)
			        {
				        Status.PreviousMode = Status.Mode;  
				        Status.Mode = modeData.UIModeChange;
				        result = true;
			        }

			        if (modeData.UIStateChange != UIState.None)
			        {
				        if (modeData.ActiveAfterPress)
				        {
					        Status.State ^= modeData.UIStateChange;
				        }
				        else if (Status.State != modeData.UIStateChange)
				        {
					        Status.State |= modeData.UIStateChange;
				        }
				        result = true;
			        }

			        if (modeData.UIDisplayChange != UIDisplay.None)
			        {
				        if (modeData.ActiveAfterPress)
				        {
					        Status.Display ^= modeData.UIDisplayChange;
				        }
				        else if (Status.Display != modeData.UIDisplayChange)
				        {
					        Status.Display |= modeData.UIDisplayChange;
				        }
				        result = true;
			        }
                    break;
		        }
	        }

            FocusPad.PadStyle = (Status.Display & UIDisplay.ShowFocusPad) != 0 ? PadStyle.Normal : PadStyle.Hidden;
            ViewPad.PadStyle = (Status.Display & UIDisplay.ShowViewPad) != 0 ? PadStyle.Normal : PadStyle.Hidden;
            _renderer.ShowBitmap = (Status.Display & UIDisplay.ShowDebugInfo) != 0;
            return result;
        }
        public bool KeyUp(KeyEventArgs e)
        {
	        foreach (var modeData in ModeMap)
	        {
		        if (Status.Mode == modeData.UIModeChange && !modeData.ActiveAfterPress)
		        {
			        Status.Mode = Status.PreviousMode;
			        Status.PreviousMode = UIMode.None;
		        }

		        if (modeData.UIStateChange != UIState.None && !modeData.ActiveAfterPress)
		        {
			        Status.State &= ~modeData.UIStateChange;
		        }

		        if (Status.Display != UIDisplay.None && !modeData.ActiveAfterPress)
		        {
			        Status.Display &= ~modeData.UIDisplayChange;
		        }
            }

	        FocusPad.PadStyle = (Status.Display & UIDisplay.ShowFocusPad) != 0 ? PadStyle.Normal : PadStyle.Hidden;
	        ViewPad.PadStyle = (Status.Display & UIDisplay.ShowViewPad) != 0 ? PadStyle.Normal : PadStyle.Hidden;
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
