using SkiaSharp;
using Slugs.Primitives;

namespace Slugs.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RenderDecoder
    {
	    private RendererBase Renderer { get; }

	    public RenderDecoder(RendererBase renderer)
	    {
		    Renderer = renderer;
	    }

	    private List<string> _stringList;
	    public void DecodeAndRender(List<List<int>> encoding, List<string> stringList)
	    {
		    _stringList = stringList;
		    foreach (var drawElement in encoding)
		    {
			    switch ((DrawCommand) drawElement[0])
			    {
                    case DrawCommand.RoundBox:
                        DrawRoundBox(drawElement);
                        break;
			    }
		    }
		    _stringList = null;
	    }
	    public void DrawRoundBox(List<int> encoding)
	    {
		    var index = 1;
		    var point = DecodePoint(encoding, ref index);
		    var pen = DecodePen(encoding, ref index);
		    var radius = IntToFloat(encoding, ref index);
		    Renderer.DrawRoundBox(point, pen, radius);
	    }
	    public void DrawPolyline(List<int> encoding)
	    {
		    var index = 1;
		    var points = DecodePoints(encoding, ref index);
		    var pen = DecodePen(encoding, ref index);
		    Renderer.DrawPolyline(points, pen);
	    }
	    public void DrawPath(List<int> encoding)
	    {
		    var index = 1;
		    var points = DecodePoints(encoding, ref index);
		    var pen = DecodePen(encoding, ref index);
		    Renderer.DrawPath(points, pen);
	    }
	    public void DrawDirectedLine(List<int> encoding)
	    {
		    var index = 1;
		    var startPoint = DecodePoint(encoding, ref index);
		    var endPoint = DecodePoint(encoding, ref index);
		    var pen = DecodePen(encoding, ref index);
		    Renderer.DrawDirectedLine(new SKSegment(startPoint, endPoint), pen);
	    }
        public void DrawText(List<int> encoding)
        {
            var index = 1;
            var center = DecodePoint(encoding, ref index);
            var text = _stringList[encoding[index++]];
            var pen = DecodePen(encoding, ref index);
            Renderer.DrawText(center, text, pen);
        }

        public void DrawBitmap(List<int> encoding)
        {
        }


        private SKPoint DecodePoint(List<int> encoding, ref int index)
	    {
		    var x = IntToFloat(encoding, ref index);
		    var y = IntToFloat(encoding, ref index);
		    return new SKPoint(x , y);
	    }
	    private SKPoint[] DecodePoints(List<int> encoding, ref int index)
	    {
		    var count = encoding[index++];
		    var result = new SKPoint[count];
		    for (int i = 0; i < count; i++)
		    {
			    result[i] = DecodePoint(encoding, ref index);
		    }
		    return result;
	    }
        private SKPaint DecodePen(List<int> encoding, ref int index)
	    {
		    return Renderer.Pens[index++];
	    }
        private float IntToFloat(List<int> encoding, ref int index, int decimalPlaces = 3)
	    {
		    return (float)encoding[index++] / (10f * decimalPlaces);
	    }
    }
}
