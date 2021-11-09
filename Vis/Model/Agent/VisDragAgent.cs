using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ML.Probabilistic.Factors;
using Vis.Model.Controller;
using Vis.Model.Primitives;

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
        private IRenderer _renderer;
        private SkiaRenderer _hoverRender;
        private VisMeasureSkills _skills;
        private IPath unit;

        public int _unitPixels = 220;

        public VisDragAgent(IRenderer renderer)
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
        }

        bool _isDown = false;
        bool _isHighlighting = false;
        private bool _isDraggingExisting = false;
        VisPoint _highlightingPoint;
        VisPoint _pivotPoint;
        VisPoint _dragPoint;
        public bool MouseDown(MouseEventArgs e)
        {
            _isDown = true;
            if (_isHighlighting)
            {
	            var path = ViewPad.PathWithNodeNear(_highlightingPoint);
	            if (path != null)
	            {
		            bool isOnStartPoint = path.StartPoint.IsNear(_highlightingPoint);
		            _pivotPoint = isOnStartPoint ? path.EndPoint : path.StartPoint;
		            //_skills.Point(this, _pivotPoint, true);
		            if (path is VisStroke stroke)
		            {
			            _isDraggingExisting = true;
			            _dragPoint = _highlightingPoint;
			            _isHighlighting = false;
		            }
                }
                else
	            {
		            _isHighlighting = false;
	            }
            }
            
            if(!_isHighlighting)
            {
	            _pivotPoint = _isDraggingExisting ? _highlightingPoint.Clone() : new VisPoint(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);
	            _skills.Point(this, _pivotPoint);
            }

            _isHighlighting = false;
            _highlightingPoint = null;
            return true;
        }

        public bool MouseMove(MouseEventArgs e)
        {
            var result = false;
            var p = new VisPoint(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);

            if (_hoverRender != null)
            {
	            var col =_hoverRender.Bitmap.GetPixel(e.X, e.Y);
	            Console.WriteLine(col);
            }

            if (_isDown)
            {
	            if (_isDraggingExisting)
	            {
		            _dragPoint.X = p.X;
		            _dragPoint.Y = p.Y;
		            _skills.Line(this, _pivotPoint, p);
                }
	            else
	            {
	                var path = _drawCircle ?
		                _skills.Circle(this, _pivotPoint, p) :
		                _skills.Line(this, _pivotPoint, p);
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
                _isHighlighting = true;
                _highlightingPoint = sp;
                result = true;
            }
            else
            {
	            _isHighlighting = false;
	            _highlightingPoint = null;
            }

            if (!result && _isHighlighting)
            {
                _isHighlighting = false;
                _highlightingPoint = null;
                result = true;
            }

	        return result;
        }

        public bool MouseUp(MouseEventArgs e)
        {
            _isDown = false;
            var endPoint = _isHighlighting ? _highlightingPoint : new VisPoint(e.X / (float)_unitPixels, e.Y / (float)_unitPixels);

            var path = _drawCircle ? 
	            _skills.Circle(this, _pivotPoint, endPoint, true) :
	            _skills.Line(this, _pivotPoint, endPoint, true);

            if (unit == null)
            {
	            unit = path;
            }
            else
            {
	            path.UnitReference = unit;
            }
            _pivotPoint = null;
            _isDraggingExisting = false;
            _dragPoint = null;
            return true;
        }

        private Keys _keyDown = Keys.None;
        private bool _drawCircle = false;
        public bool KeyDown(KeyEventArgs e)
        {
	        bool result = false;
	        _keyDown = e.KeyCode;
	        if (_keyDown == Keys.C && !_drawCircle)
	        {
		        _drawCircle = true;
		        result = true;
	        }
	        return result;
        }
        public bool KeyUp(KeyEventArgs e)
        {
	        _keyDown = Keys.None;
	        _drawCircle = false;
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
	        _renderer.PenIndex = _isDown ? 3 : 2;
        }

        private void _renderer_DrawingComplete(object sender, EventArgs e)
        {
	        _hoverRender?.DrawOnBitmap();
	        Clear();
        }



    }
}
