using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;
using Vis.Model.Agent;

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
        public bool IsHoverMap { get; set; }

        public SkiaPens(float scale, float defaultWidth = 8f)
	    {
		    Scale = scale;
		    DefaultWidth = defaultWidth;
		    GenPens();
	    }

	    public SKPaint GetPenForElement(PadAttributes attributes)
	    {
		    SKPaint result;
		    if (attributes.PadKind == PadKind.Focus)
		    {
			    result = Pens[0];
		    }
		    else if (IsHoverMap)
		    {
			    result = GetPenByOrder(attributes.Index, 4f, false);
		    }
		    else
		    {
			   result = GetPenForIndex(attributes.Index);
            }
		    
                //result.Color.WithAlpha()
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
		    Pens.Add(GetPen(SKColors.DarkOrange, DefaultWidth));
		    Pens.Add(GetPen(SKColors.DarkGoldenrod, DefaultWidth));
		    Pens.Add(GetPen(SKColors.DarkOliveGreen, DefaultWidth));
		    Pens.Add(GetPen(SKColors.DarkGreen, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkCyan, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkBlue, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkOrchid, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkMagenta, DefaultWidth));
            Pens.Add(GetPen(SKColors.Red, DefaultWidth));
            Pens.Add(GetPen(SKColors.Orange, DefaultWidth));
            Pens.Add(GetPen(SKColors.Yellow, DefaultWidth));
            Pens.Add(GetPen(SKColors.Chartreuse, DefaultWidth));
            Pens.Add(GetPen(SKColors.Green, DefaultWidth));
            Pens.Add(GetPen(SKColors.Cyan, DefaultWidth));
            Pens.Add(GetPen(SKColors.Blue, DefaultWidth));
            Pens.Add(GetPen(SKColors.Orchid, DefaultWidth));
            Pens.Add(GetPen(SKColors.Magenta, DefaultWidth));

            UIPens = new Dictionary<UIType, SKPaint>()
		    {
			    {UIType.None, GetPen(SKColors.Empty, DefaultWidth)},
			    {UIType.Node, GetPen(SKColors.DarkBlue, DefaultWidth)},
			    {UIType.Joint, GetPen(SKColors.DarkGreen, DefaultWidth)},
			    {UIType.Edge, GetPen(SKColors.Black, DefaultWidth)},
                {UIType.HighlightSpot, GetPen(SKColors.MediumSlateBlue, DefaultWidth * 2f)},
                {UIType.HighlightPath, GetPen(SKColors.DarkViolet, DefaultWidth * 2f)},
                {UIType.MeasureTick, GetPen(SKColors.DarkSlateGray, DefaultWidth)},
            };

	    }

	    public SKPaint GetPenByOrder(int index, float widthScale = 1, bool antiAlias = true)
	    {
		    uint col = (uint) ((index + 3) * 0x110D05);
		    var color = new SKColor((byte) (col & 0xFF), (byte) ((col >> 8) & 0xFF), (byte) ((col >> 16) & 0xFF));
		    return GetPen(color, DefaultWidth * widthScale, antiAlias);
	    }

	    public SKPaint GetPen(SKColor color, float width, bool antiAlias = true)
	    {
		    SKPaint pen = new SKPaint()
		    {
			    Style = SKPaintStyle.Stroke,
			    Color = color,
			    StrokeWidth = width / Scale,
			    IsAntialias = antiAlias,
                StrokeCap = SKStrokeCap.Round,
		    };
		    return pen;
	    }
    }
}

