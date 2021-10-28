using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;

namespace Vis.Model.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SkiaPens
    {
	    public List<SKPaint> Pens = new List<SKPaint>();
	    public SkiaPens(float scale)
	    {
		    GenPens(scale);
	    }
	    public SKPaint this[int index]
	    {
		    get { return Pens[index]; }
	    }
	    private void GenPens(float scale)
	    {
		    Pens.Clear();
		    Pens.Add(GetPen(SKColors.LightGray, 8f / scale));
		    Pens.Add(GetPen(SKColors.Black, 8f / scale));
		    Pens.Add(GetPen(SKColors.DarkRed, 8f / scale));
		    Pens.Add(GetPen(SKColors.Orange, 8f / scale));
		    Pens.Add(GetPen(SKColors.DarkGreen, 8f / scale));
		    Pens.Add(GetPen(SKColors.DarkBlue, 8f / scale));
		    Pens.Add(GetPen(SKColors.DarkViolet, 16f / scale));
		    Pens.Add(GetPen(SKColors.Red, 32f / scale));
	    }
	    public SKPaint GetPen(SKColor color, float width)
	    {
		    SKPaint pen = new SKPaint()
		    {
			    Style = SKPaintStyle.Stroke,
			    Color = color,
			    StrokeWidth = width,
			    IsAntialias = true,
		    };
		    return pen;
	    }
    }
}
