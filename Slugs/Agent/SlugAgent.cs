using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Slugs.Input;
using Slugs.Pads;
using Slugs.Renderer;
using Slugs.Slugs;

namespace Slugs.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SlugAgent : IAgent
    {
	    public static SlugAgent ActiveAgent { get; private set; }
        private static readonly Dictionary<int, SlugPad> Pads = new Dictionary<int, SlugPad>();

	    private double _unitPull = 0;
	    public double _unitPush = 10;

	    private SlugRenderer _renderer;
	    public RenderStatus Status { get; }

	    public readonly SlugPad WorkingPad = new SlugPad(PadKind.Working);
	    public readonly SlugPad InputPad = new SlugPad(PadKind.Drawn);

	    private SKPoint DownPoint;
	    private SKPoint CurrentPoint;
	    private SKPoint SnapPoint;
	    private List<SKPoint> DragSegment = new List<SKPoint>();
	    private List<SKPoint> ClickInfoSet = new List<SKPoint>();
	    private List<SKPoint> DragPath = new List<SKPoint>();

	    public PointRef DraggingPoint = PointRef.Empty;

	    private bool IsDown => DownPoint != SKPoint.Empty;
	    public double UnitPull
	    {
		    get => _unitPull;
		    set
		    {
			    _unitPull = value;
			    SlugPad.ActiveSlug = new Slug(_unitPull, _unitPush);
		    }
	    }
	    public double UnitPush
	    {
		    get => _unitPush;
		    set
		    {
			    _unitPush = value;
			    SlugPad.ActiveSlug = new Slug(_unitPull, _unitPush);
		    }
	    }

	    public SlugAgent(SlugRenderer renderer)
	    {
		    ActiveAgent = this;
		    SlugAgent.Pads[WorkingPad.PadIndex] = WorkingPad;
		    SlugAgent.Pads[InputPad.PadIndex] = InputPad;

            _renderer = renderer;
		    SlugPad.ActiveSlug = new Slug(UnitPull, UnitPush);
		    //var pl = (new InfoSet(new SKPoint(_renderer.Width / 2.0f, 20f), new SKPoint(_renderer.Width * 3.0f / 4.0f, 20f)));
		    var pl = (new InfoSet(new[]{new SKPoint(200, 100), new SKPoint(400, 300)} ));
		    InputPad.Add(pl);
		    _renderer.Pads.Add(WorkingPad);
		    _renderer.Pads.Add(InputPad);
		    ClearMouse();
	    }

	    public SKPoint this[PointRef pointRef]
	    {
		    get
		    {
			    var pad = SlugAgent.Pads[pointRef.PadIndex];
			    var infoSet = pad.InfoSetFromIndex(pointRef.InfoSetIndex);
			    return infoSet[pointRef.PointIndex];
		    } 
		    set
		    {
			    var pad = SlugAgent.Pads[pointRef.PadIndex];
			    var infoSet = pad.InfoSetFromIndex(pointRef.InfoSetIndex);
			    infoSet[pointRef.PointIndex] = value;
		    }
	    }

	    public void Clear()
        {
        }

        public void ClearMouse()
        {
	        DownPoint = SKPoint.Empty;
	        CurrentPoint = SKPoint.Empty;
	        SnapPoint = SKPoint.Empty;
            DragSegment.Clear();
	        ClickInfoSet.Clear();
            DragPath.Clear();
	        WorkingPad.Clear();

	        DownPoint = SKPoint.Empty;
            DraggingPoint = PointRef.Empty;
        }

        public void Draw()
	    {
		    _renderer.Draw();
	    }

	    public bool MouseDown(MouseEventArgs e)
	    {
		    CurrentPoint = e.Location.ToSKPoint();
		    SetHighlight();
		    DownPoint = SnapPoint;
            var hasHighlight = SetHighlight();
		    if (hasHighlight && CurrentKey != Keys.ControlKey)
		    {
				DraggingPoint = InputPad.Highlight;
		    }
		    else
		    {
			    DragSegment.Add(SnapPoint);
			    DragPath.Add(SnapPoint);
		    }

            return true;
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
            WorkingPad.Clear();
		    CurrentPoint = e.Location.ToSKPoint();
            SetHighlight();
            SetDragging();
            SetCreating();

            return true;
        }

	    public bool MouseUp(MouseEventArgs e)
	    {
		    CurrentPoint = e.Location.ToSKPoint();
		    SetDragging();
		    SetCreating(true);

            ClearMouse();
            return true;
        }

	    private Keys CurrentKey;
	    public bool KeyDown(KeyEventArgs e)
	    {
		    CurrentKey = e.KeyCode;
		    return true;
        }

	    public bool KeyUp(KeyEventArgs e)
	    {
		    CurrentKey = Keys.None;
            return true;
        }

	    private bool SetHighlight()
	    {
		    bool hasChange = false;
		    var snap = InputPad.GetSnapPoints(CurrentPoint);
		    if (snap.Length > 0)
		    {
			    hasChange = true;
			    InputPad.Highlight = snap[0];
			    InputPad.HighlightLine = PointRef.Empty;
		    }
		    else
		    {
			    InputPad.Highlight = PointRef.Empty;
			    var snapLine = InputPad.GetSnapLine(CurrentPoint);
			    if (snapLine.IsEmpty)
			    {
				    InputPad.HighlightLine = PointRef.Empty;
			    }
			    else
			    {
				    hasChange = true;
				    InputPad.HighlightLine = snapLine;
			    }
		    }
		    SnapPoint = InputPad.Highlight.IsEmpty ? CurrentPoint : InputPad.GetHighlightPoint();

		    return hasChange;
	    }

	    private bool SetCreating(bool final = false)
	    {
		    var result = false;
		    if (DraggingPoint.IsEmpty && IsDown)
		    {
			    DragPath.Add(SnapPoint);
			    WorkingPad.Add(new InfoSet(DownPoint, SnapPoint));
			    if (final)
			    {
				    DragSegment.Add(SnapPoint);
				    ClickInfoSet.Add(SnapPoint);
				    DragPath.Add(SnapPoint);
				    InputPad.Add(new InfoSet(DragSegment));
                }
			    result = true;
		    }
		    return result;
	    }
        private bool SetDragging()
	    {
		    var result = false;
		    if (!DraggingPoint.IsEmpty)
		    {
			    InputPad.UpdatePoint(DraggingPoint, SnapPoint);
			    result = true;
		    }
		    return result;
	    }
    }
}
