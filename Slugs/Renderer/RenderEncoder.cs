using System.IO;
using System.Windows.Forms.VisualStyles;
using SkiaSharp;
using Slugs.Primitives;

namespace Slugs.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RenderEncoder : RendererBase
    {
	    private List<List<int>> Encoding = new List<List<int>>();
	    private List<string> StringList = new List<string>();

        public RenderEncoder() { }

        private RenderDecoder _decoder = new RenderDecoder(new SlugRenderer());
        public override void Draw()
        {
            Encoding.Clear();
	        base.Draw();
	        Console.WriteLine(GenerateCode());
	        _decoder.DecodeAndRender(Canvas, Encoding, StringList);
        }

        public override void DrawRoundBox(SKPoint point, SKPaint paint, float radius = 8f)
	    {
		    var drawElement = new List<int>(){ (int)DrawCommand.RoundBox };
		    EncodePoint(point, drawElement);
            EncodePen(paint, drawElement);
            drawElement.Add(FloatToInt(radius));
		    Encoding.Add(drawElement);
        }
	    public override void DrawPolyline(SKPoint[] polyline, SKPaint paint)
	    {
		    var drawElement = new List<int>() { (int)DrawCommand.Polyline };
		    drawElement.Add(polyline.Length);
		    foreach (var point in polyline)
		    {
			    EncodePoint(point, drawElement);
            }
		    EncodePen(paint, drawElement);
		    Encoding.Add(drawElement);
        }
	    public override void DrawPath(SKPoint[] polyline, SKPaint paint)
	    {
		    var drawElement = new List<int>() { (int)DrawCommand.Path };
		    drawElement.Add(polyline.Length);
		    foreach (var point in polyline)
		    {
			    EncodePoint(point, drawElement);
		    }
		    EncodePen(paint, drawElement);
		    Encoding.Add(drawElement);
        }
	    public override void DrawDirectedLine(SKSegment seg, SKPaint paint)
	    {
		    var drawElement = new List<int>() { (int)DrawCommand.DirectedLine };
		    EncodePoint(seg.StartPoint, drawElement);
		    EncodePoint(seg.EndPoint, drawElement);
            EncodePen(paint, drawElement);
		    Encoding.Add(drawElement);
        }
	    public override void DrawText(SKPoint center, string text, SKPaint paint)
	    {
            StringList.Add(text);
		    var drawElement = new List<int>() { (int)DrawCommand.Text };
		    EncodePoint(center, drawElement);
		    drawElement.Add(StringList.Count - 1);
            EncodePen(paint, drawElement);
		    Encoding.Add(drawElement);
        }
	    public override void DrawBitmap(SKBitmap bitmap)
	    {
	    }

	    private void EncodePoint(SKPoint point, List<int> drawElement)
	    {
		    drawElement.Add(FloatToInt(point.X));
		    drawElement.Add(FloatToInt(point.Y));
        }
	    private void EncodePen(SKPaint pen, List<int> drawElement)
	    {
		    var penIndex = Pens.IndexOfPen(pen);
		    drawElement.Add(penIndex);
	    }

        public override void GeneratePens()
	    {
		    Pens = new SlugPens(1);
        }

	    public int FloatToInt(float value, int decimalPlaces = 3)
	    {
		    return (int)(value * (decimalPlaces * 10));
	    }

	    public void Save(string filePath)
	    {
		    using (Stream stream = File.Open(filePath, FileMode.Create))
		    {
			    var file = new EncodedFile(Encoding, StringList);
			    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			    binaryFormatter.Serialize(stream, file);
            }
        }

		public string GenerateCode()
	    {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"public static EncodedFile file = new EncodedFile(");
            sb.AppendLine(@"    new List<List<int>>()");
            sb.AppendLine(@"    {");
            foreach (var encodedLine in Encoding)
            {
	            sb.Append(@"        new List<int>() {");
	            foreach (var value in encodedLine)
	            {
		            sb.Append(value.ToString() + ",");
	            }
	            sb.AppendLine(@"},");
            }
            sb.AppendLine(@"    },");
            sb.AppendLine(@"    new List<string>()");
            sb.AppendLine(@"    {");
            foreach (var text in StringList)
            {
	            sb.AppendLine("     \"" + text + "\",");
            }
            sb.AppendLine(@"    }");
            sb.AppendLine(@");");

            return sb.ToString();
	    }
    }

    public class EncodedFile
    {
	    private List<List<int>> Encoding = new List<List<int>>();
	    private List<string> StringList = new List<string>();

	    public EncodedFile(List<List<int>> encoding, List<string> stringList)
	    {
		    Encoding = encoding;
		    StringList = stringList;
	    }
    }

    public enum DrawCommand
    {
	    RoundBox,
	    Polyline,
        Path,
        DirectedLine,
        Text,
        Bitmap,

    }
}
