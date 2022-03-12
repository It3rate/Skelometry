using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SkiaSharp;
using Slugs.Primitives;

namespace Slugs.Renderer
{
	public class RenderEncoder : RendererBase
    {
        public bool DecodeAndRender { get; set; }

        private List<List<int>> Encoding = new List<List<int>>();
	    private List<string> StringList = new List<string>();

	    private RenderDecoder _decoder = new RenderDecoder(new SlugRenderer());

	    public RenderEncoder(bool decodeAndRender) : base()
	    {
		    DecodeAndRender = decodeAndRender;
	    }

        public override void Draw()
        {
	        Encoding.Clear();
	        StringList.Clear();
            base.Draw();
	        if (DecodeAndRender)
	        {
		        //Console.WriteLine(GenerateCode());
		        _decoder.DecodeAndRender(Canvas, Encoding, StringList);
            }
        }

        public override void DrawRoundBox(SKPoint point, SKPaint paint, float radius = 8f)
	    {
		    var drawElement = new List<int> { (int)DrawCommand.RoundBox };
		    EncodePoint(point, drawElement);
            EncodePen(paint, drawElement);
            drawElement.Add(FloatToInt(radius));
		    Encoding.Add(drawElement);
        }
	    public override void DrawPolyline(SKPoint[] polyline, SKPaint paint)
	    {
		    var drawElement = new List<int> { (int)DrawCommand.Polyline };
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
		    var drawElement = new List<int> { (int)DrawCommand.Path };
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
		    var drawElement = new List<int> { (int)DrawCommand.DirectedLine };
		    EncodePoint(seg.StartPoint, drawElement);
		    EncodePoint(seg.EndPoint, drawElement);
            EncodePen(paint, drawElement);
		    Encoding.Add(drawElement);
        }
	    public override void DrawText(SKPoint center, string text, SKPaint paint)
	    {
            StringList.Add(text);
		    var drawElement = new List<int> { (int)DrawCommand.Text };
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
		    Pens = new SlugPens();
        }

	    public int FloatToInt(float value, int decimalPlaces = 3)
	    {
		    return (int)(value * (decimalPlaces * 10));
	    }

        public EncodedFile EncodedFile => new EncodedFile(Encoding, StringList);

        public void Save(string filePath)
	    {
		    using (Stream stream = File.Open(filePath, FileMode.Create))
		    {
			    var binaryFormatter = new BinaryFormatter();
			    binaryFormatter.Serialize(stream, EncodedFile);
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
		            sb.Append(value + ",");
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
	    private readonly List<List<int>> Encoding;
	    private readonly List<string> StringList;

	    public EncodedFile(List<List<int>> encoding, List<string> stringList)
	    {
		    Encoding = new List<List<int>>(encoding.Count);
		    foreach (var enc in encoding)
		    {
			    var el = new List<int>();
			    foreach (var val in enc)
			    {
                    el.Add(val);
			    }
                Encoding.Add(el);
		    }

            StringList = new List<string>(stringList.Count);
		    foreach (var s in stringList)
		    {
				StringList.Add(s);   
		    }
	    }

	    public static bool operator ==(EncodedFile a, EncodedFile b)
	    {
            // need to manually check encoding due to nested lists, and also makes debugging easier.
            var result = a.Encoding.Count == b.Encoding.Count && a.StringList.SequenceEqual(b.StringList);
            if (result)
		    {
			    for (var i = 0; i < a.Encoding.Count; i++)
			    {
				    if (a.Encoding[i].Count != b.Encoding[i].Count)
				    {
					    result = false;
					    goto EndTest;
				    }
				    else
				    {
					    for (var j = 0; j < a.Encoding[i].Count; j++)
					    {
						    if (a.Encoding[i][j] != b.Encoding[i][j])
						    {
							    result = false;
							    goto EndTest;
						    }
					    }
				    }
			    }
		    }
		    EndTest:
		    return result;
	    }

        public static bool operator !=(EncodedFile a, EncodedFile b) => (a is null) || (b is null) || !(a == b);
	    public override bool Equals(object a) => a is EncodedFile ef && ef == this;
        public override int GetHashCode() => Encoding.GetHashCode() + 17 * StringList.GetHashCode();
	    
    }

    public enum DrawCommand
    {
        None = 0,
        RoundBox = 1,
        Polyline = 2,
        Path = 3,
        DirectedLine = 4,
        Text = 5,
        Bitmap = 6,
    }
}
