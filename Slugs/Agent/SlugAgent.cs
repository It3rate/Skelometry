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
	    private double _unitPull = 0;
	    public double _unitPush = 10;

        private SlugRenderer _renderer;
	    public RenderStatus Status { get; }

	    public readonly SlugPad WorkingPad = new SlugPad(PadKind.Working);
	    public readonly SlugPad InputPad = new SlugPad(PadKind.Drawn);

        private SKPoint DownPoint;
        private SKPoint CurrentPoint;
        private List<SKPoint> DragSegment = new List<SKPoint>();
        private List<SKPoint> ClickPolyline = new List<SKPoint>();
        private List<SKPoint> DragPath = new List<SKPoint>();

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
	        _renderer = renderer;
	        SlugPad.ActiveSlug = new Slug(UnitPull, UnitPush);
	        var pl =(new SkiaPolyline(new SKPoint(_renderer.Width / 2.0f, 20f), new SKPoint(_renderer.Width * 3.0f / 4.0f, 20f)));
	        InputPad.Add(pl);
	        _renderer.Pads.Add(WorkingPad);
	        _renderer.Pads.Add(InputPad);
            ClearMouse();
        }

        public void Clear()
        {
        }

        public void ClearMouse()
        {
	        DownPoint = SKPoint.Empty;
	        CurrentPoint = SKPoint.Empty;
	        DragSegment.Clear();
	        ClickPolyline.Clear();
            DragPath.Clear();
	        WorkingPad.Clear();
        }

        public void Draw()
	    {
		    _renderer.Draw();
	    }

	    public bool MouseDown(MouseEventArgs e)
	    {
		    DownPoint = e.Location.ToSKPoint();
		    CurrentPoint = e.Location.ToSKPoint(); 
		    DragSegment.Add(DownPoint);
		    DragPath.Add(CurrentPoint);

            return true;
	    }

	    public bool MouseMove(MouseEventArgs e)
	    {
            WorkingPad.Clear();
		    CurrentPoint = e.Location.ToSKPoint();
		    if (IsDown)
		    {
			    DragPath.Add(CurrentPoint);
                WorkingPad.Add(new SkiaPolyline(DownPoint, CurrentPoint));
		    }

		    var snap = InputPad.GetSnapPoints(CurrentPoint);
		    if (snap.Length > 0)
		    {
			    InputPad.Highlight = snap[0];
		    }
		    else
		    {
			    InputPad.Highlight = SKPoint.Empty;
		    }
            return true;
        }

	    public bool MouseUp(MouseEventArgs e)
	    {
		    CurrentPoint = e.Location.ToSKPoint();
		    DragSegment.Add(e.Location.ToSKPoint());
            ClickPolyline.Add(e.Location.ToSKPoint());
		    DragPath.Add(CurrentPoint);
            InputPad.Add(new SkiaPolyline(DragSegment));
		    DownPoint = SKPoint.Empty;

            ClearMouse();
            return true;
        }

	    public bool KeyDown(KeyEventArgs e)
	    {
		    return true;
        }

	    public bool KeyUp(KeyEventArgs e)
	    {
		    return true;
        }
    }
}
