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
	    private Dictionary<UIType, SKPaint> UIPens;

        public float Scale { get; }
	    public float DefaultWidth { get; }

	    public SkiaPens(float scale, float defaultWidth = 8f)
	    {
		    Scale = scale;
		    DefaultWidth = defaultWidth;
		    GenPens();
	    }

	    public SKPaint GetPenForElement(PadAttributes attributes)
	    {
		    SKPaint result = GetPenForIndex(attributes.Index);
		    return result;
	    }

	    public SKPaint GetPenForUIType(UIType uiType)
	    {
		    SKPaint result = UIPens[uiType];
		    return result;
	    }

	    public SKPaint this[int i] => GetPenForIndex(i);

	    private SKPaint GetPenForIndex(int index)
	    {
		    SKPaint result;
		    index = index < 0 ? 0 : index;
		    if (index < Pens.Count)
		    {
			    result = Pens[index];
		    }
		    else
		    {
			    result = GetPenByOrder(index - Pens.Count);
		    }

		    return result;
	    }

	    private void GenPens()
	    {
		    Pens.Clear();
		    Pens.Add(GetPen(SKColors.LightGray, DefaultWidth));
		    Pens.Add(GetPen(SKColors.Black, DefaultWidth));
		    Pens.Add(GetPen(SKColors.DarkRed, DefaultWidth));
		    Pens.Add(GetPen(SKColors.Orange, DefaultWidth));
		    Pens.Add(GetPen(SKColors.DarkGreen, DefaultWidth));
		    Pens.Add(GetPen(SKColors.DarkBlue, DefaultWidth));
		    Pens.Add(GetPen(SKColors.DarkViolet, DefaultWidth * 2f));
		    Pens.Add(GetPen(SKColors.Red, DefaultWidth * 4f));

		    UIPens = new Dictionary<UIType, SKPaint>()
		    {
			    {UIType.None, GetPen(SKColors.Empty, DefaultWidth)},
			    {UIType.Node, GetPen(SKColors.DarkBlue, DefaultWidth)},
			    {UIType.Joint, GetPen(SKColors.DarkGreen, DefaultWidth)},
			    {UIType.Edge, GetPen(SKColors.Black, DefaultWidth)},
                {UIType.HighlightSpot, GetPen(SKColors.Red, DefaultWidth * 2f)},
                {UIType.HighlightPath, GetPen(SKColors.DarkViolet, DefaultWidth * 2f)},
                {UIType.Measure, GetPen(SKColors.Gray, DefaultWidth)},
            };

	    }

	    public SKPaint GetPenByOrder(int index)
	    {
		    uint col = (uint) ((index + 3) * 0x110D05);
		    var color = new SKColor((byte) (col & 0xFF), (byte) ((col >> 8) & 0xFF), (byte) ((col >> 16) & 0xFF));
		    return GetPen(color, DefaultWidth);
	    }

	    public SKPaint GetPen(SKColor color, float width)
	    {
		    SKPaint pen = new SKPaint()
		    {
			    Style = SKPaintStyle.Stroke,
			    Color = color,
			    StrokeWidth = width / Scale,
			    IsAntialias = true,
		    };
		    return pen;
	    }
    }
}

