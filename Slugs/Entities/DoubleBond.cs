using System.Net;
using System.Numerics;
using System.Windows.Forms.ComponentModel.Com2Interop;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Constraints;
using Slugs.Primitives;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DoubleBond : ElementBase, IAreaElement //, ISlugElement // the double bond is a fixed ratio between two focals already.
    {
	    public override ElementKind ElementKind => ElementKind.DoubleBond;
	    public override IElement EmptyElement => Empty;
	    public static readonly DoubleBond Empty = new DoubleBond();
	    private DoubleBond() : base(true) { }// Empty ctor

	    public int StartKey { get; private set; }
	    public int EndKey { get; private set; }
	    public Slug Ratio { get; set; }
	    public Complex ComplexRatio { get; set; }

        public Focal StartFocal
	    {
		    get => Pad.FocalAt(StartKey);
		    set => SetStartKey(value.Key);
	    }
	    public Focal EndFocal
	    {
		    get => Pad.FocalAt(EndKey);
		    set => SetEndKey(value.Key);
        }
	    public Focal OtherFocal(Focal orgFocal) => (orgFocal.Key == StartKey) ? EndFocal : (orgFocal.Key == EndKey) ? StartFocal : Focal.Empty;
	    public SKSegment StartSegment => StartFocal.Segment;
	    public SKSegment EndPosition => EndFocal.Segment;

	    public override bool HasArea => true;

	    public override List<IPoint> Points => new List<IPoint>() { StartFocal.StartPoint, StartFocal.EndPoint, EndFocal.StartPoint, EndFocal.EndPoint };
	    public override float DistanceToPoint(SKPoint point)
	    {
		    var startDist = StartFocal.DistanceToPoint(point);
		    var endDist = EndFocal.DistanceToPoint(point);
		    return startDist < endDist ? startDist : endDist;
	    }

	    public DoubleBond(Focal startFocal, Focal endFocal) : base(startFocal.PadKind)
	    {
		    SetStartKey(startFocal.Key);
		    SetEndKey(endFocal.Key);
            CalculateRatio();
	    }

        protected void SetStartKey(int key)
	    {
		    if (!Pad.FocalAt(key).IsEmpty)
		    {
			    StartKey = key;
                CalculateRatio();
		    }
		    else
		    {
			    throw new ArgumentException("DoubleBond focals must be type Focal.");
		    }
	    }
	    protected void SetEndKey(int key)
	    {
		    if (!Pad.FocalAt(key).IsEmpty)
		    {
			    EndKey = key;
			    CalculateRatio();
            }
		    else
		    {
			    throw new ArgumentException("DoubleBond focals must be type Focal.");
		    }
	    }
	    public Focal ContainsFocalPoint(FocalPoint point)
	    {
		    var result = Focal.Empty;
		    if (StartFocal.StartKey == point.Key || StartFocal.EndKey == point.Key)
		    {
			    result = StartFocal;
		    }
		    else if (EndFocal.StartKey == point.Key || EndFocal.EndKey == point.Key)
		    {
			    result = EndFocal;
		    }
            return result;
	    }
        public bool ContainsPosition(SKPoint point)
	    {
		    throw new NotImplementedException();
	    }

	    public void CalculateRatio()
	    {
		    var c0 = new Complex(0.9, 0.1);
		    var c1 = new Complex(1, 0);
		    var cr = c0 / c1;
		    var r0 = c1 * cr;
		    var r1 = c0 / cr;
		    var c0b = new Complex(0.7, 0.1);
		    //var crb = c0b / c1;
      //      var r0b = c1 * cr;
		    var r1b = c0b / cr;
		    var len = r1.Magnitude;
		    var lenb = r1b.Magnitude;
		    if (!StartFocal.IsEmpty && !EndFocal.IsEmpty)
		    {
	            Ratio = EndFocal.LocalSlug / StartFocal.LocalSlug;
			    var real = EndFocal.Length / StartFocal.Length;
			    var img = EndFocal.LocalSlug.Start / StartFocal.LocalSlug.Start;
			    //ComplexRatio = new Complex(real, img);
			    ComplexRatio = EndFocal.Complex / StartFocal.Complex;
			    Ratio = new Slug(img, real);
		    }

        }
	    public void ApplyRatioToEnd()
	    {
		    //var real = StartFocal.LengthT * -ComplexRatio.Real;
		    //var img = StartFocal.StartT * ComplexRatio.Imaginary;
		    //EndFocal.LocalSlug = new Slug(img, real);
            var val = StartFocal.Slug.Complex * ComplexRatio;
            EndFocal.Complex = new Complex(val.Real,      val.Imaginary);
	    }
	    public void ApplyRatioToStart()
	    {
		    StartFocal.Complex = EndFocal.Slug.Complex / ComplexRatio;
            //StartFocal.Slug = EndFocal.Slug / Ratio;
            //      var real = EndFocal.LocalSlug.DirectedLength() / ComplexRatio.Real;
            //var img = StartFocal.LocalSlug.Start * ComplexRatio.Imaginary;
            //      StartFocal.LocalSlug = new Slug(.5f, real);
        }

        public override SKPath Path
	    {
		    get
		    {
			    var path = new SKPath();
			    path.AddPoly(new SKPoint[] { StartFocal.StartPosition, StartFocal.EndPosition, EndFocal.EndPosition, EndFocal.StartPosition }, true);
			    return path;
		    }
	    }

        public static bool operator ==(DoubleBond left, DoubleBond right) =>
		    left.Key == right.Key && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
	    public static bool operator !=(DoubleBond left, DoubleBond right) =>
		    left.Key != right.Key || left.StartKey != right.StartKey || left.EndKey != right.EndKey;
	    public override bool Equals(object obj) => obj is DoubleBond value && this == value;
	    public override int GetHashCode() => 13 * Key + 17 * Key + 27 * StartKey + 29 * EndKey;
    }
}
