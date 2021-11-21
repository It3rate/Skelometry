using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;
using Vis.Model.Agent;
using Vis.Model.Render;

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
	    private Dictionary<ElementType, SKPaint> UIPens;

        public float Scale { get; }
        public float DefaultWidth { get; }
        public bool IsHoverMap { get; set; }

        public SKPaint HoverPen { get; private set; }
        public SKPaint SelectedPen { get; private set; }
        public SKPaint UnitPen { get; private set; }
        public SKPaint DarkPen { get; private set; }
        public SKPaint GrayPen { get; private set; }
        public SKPaint WorkingPen { get; private set; }

        public SkiaPens(float scale, float defaultWidth = 8f)
	    {
		    Scale = scale;
		    DefaultWidth = defaultWidth;
		    GenPens();
	    }

	    public SKPaint[] GetPensForElement(PadAttributes attributes)
	    {
		    SKPaint[] result = new SKPaint[2];
		    
		    if (attributes.ElementStyle == ElementStyle.Highlighting)
		    {
			    result[0] = HoverPen;
		    }
		    
		    if (IsHoverMap)
		    {
			    result[1] = GetPenByOrder(attributes.Index, 3f, false);
		    }
		    else if (attributes.PadKind == PadKind.Working)
		    {
			    result[1] = WorkingPen;
		    }
		    else if (attributes.PadKind == PadKind.Focus)
		    {
			    result[1] = GrayPen;
		    }
            else if (attributes.ElementState == ElementState.Selected)
		    {
			    result[1] = SelectedPen;
		    }
		    else if (attributes.ElementLinkage == ElementLinkage.IsUnit)
		    {
			    result[1] = UnitPen;
		    }
            else
		    { 
			    //result = GetPenForIndex(attributes.Index);
			    result[1] = DarkPen;
            }

		    return result;
	    }

	    public SKPaint GetPenForUIType(ElementType uiType)
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
		    HoverPen = GetPen(SKColors.LightCyan, DefaultWidth * 4f);
		    SelectedPen = GetPen(SKColors.LightGreen, DefaultWidth * 1.5f);
		    UnitPen = GetPen(SKColors.SteelBlue, DefaultWidth * 1.5f);
		    GrayPen = GetPen(SKColors.LightGray, DefaultWidth * .75f);
		    DarkPen = GetPen(SKColors.Black, DefaultWidth);
		    WorkingPen = GetPen(SKColors.DarkGray, DefaultWidth);

            Pens.Clear();
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

            UIPens = new Dictionary<ElementType, SKPaint>()
		    {
			    {ElementType.None, GetPen(SKColors.Empty, DefaultWidth)},
			    {ElementType.Node, GetPen(SKColors.DarkBlue, DefaultWidth)},
			    {ElementType.Joint, GetPen(SKColors.DarkGreen, DefaultWidth)},
			    {ElementType.Edge, GetPen(SKColors.Black, DefaultWidth)},
                {ElementType.HighlightSpot, GetPen(SKColors.LightSteelBlue, DefaultWidth * 2f)},
                {ElementType.HighlightPath, GetPen(SKColors.DarkViolet, DefaultWidth * 2f)},
                {ElementType.MeasureTick, GetPen(SKColors.DarkSlateGray, DefaultWidth)},
            };

	    }

        public Dictionary<uint, int> IndexOfColor { get; } = new Dictionary<uint, int>();
	    public SKPaint GetPenByOrder(int index, float widthScale = 1, bool antiAlias = true)
	    {
		    //uint col = (uint)((index + 3) | 0xFF000000);
		    uint col = (uint)((index + 3) * 0x110D05) | 0xFF000000;
            if (IndexOfColor.ContainsKey(col))
            {
	            IndexOfColor[col] = index;
            }
            else
            {
	            IndexOfColor.Add(col, index);
            }

            var color = new SKColor(col);
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

