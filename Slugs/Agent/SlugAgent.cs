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

	    public readonly SlugPad WorkingPad = new SlugPad();
	    public readonly SlugPad InputPad = new SlugPad();
        public readonly SlugPad OutputPad = new SlugPad();

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
		        OutputPad.PadSlug = new Slug(_unitPull, _unitPush);
	        }
        }
        public double UnitPush
        {
	        get => _unitPush;
	        set
	        {
		        _unitPush = value;
		        OutputPad.PadSlug = new Slug(_unitPull, _unitPush);
	        }
        }

        public SlugAgent(SlugRenderer renderer)
        {
	        _renderer = renderer;
	        OutputPad.PadSlug = new Slug(UnitPull, UnitPush);
	        InputPad.Polylines.Add((new SkiaPolyline(new SKPoint(_renderer.Width / 2.0f, 20f), new SKPoint(_renderer.Width * 3.0f / 4.0f, 20f))));
	        _renderer.Pads.Add(WorkingPad);
	        _renderer.Pads.Add(InputPad);
	        _renderer.Pads.Add(OutputPad);
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
	        WorkingPad.Polylines.Clear();
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
            WorkingPad.Polylines.Clear();
		    CurrentPoint = e.Location.ToSKPoint();
		    if (IsDown)
		    {
			    DragPath.Add(CurrentPoint);
                WorkingPad.Polylines.Add(new SkiaPolyline(DownPoint, CurrentPoint));
		    }
            return true;
        }

	    public bool MouseUp(MouseEventArgs e)
	    {
		    CurrentPoint = e.Location.ToSKPoint();
		    DragSegment.Add(e.Location.ToSKPoint());
            ClickPolyline.Add(e.Location.ToSKPoint());
		    DragPath.Add(CurrentPoint);
            InputPad.Polylines.Add(new SkiaPolyline(DragSegment));
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
