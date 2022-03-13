using SkiaSharp;
using Slugs.Primitives;
using System.Numerics;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;

    public class DoubleBond : ElementBase, IAreaElement, ISlugElement // the double bond is a fixed ratio between two focals already.
    {
	    public override ElementKind ElementKind => ElementKind.DoubleBond;
	    public override IElement EmptyElement => Empty;
	    public static readonly DoubleBond Empty = new DoubleBond();
	    private DoubleBond() : base(true) { }// Empty ctor

	    public int StartKey { get; private set; }
	    public int EndKey { get; private set; }
	    public override List<int> AllKeys => new List<int>() { StartKey, EndKey, StartFocal.StartKey, StartFocal.EndKey, EndFocal.StartKey, EndFocal.EndKey };

        public Slug LocalRatio { get; set; }
	    public float StartT
	    {
		    get => (float)LocalRatio.Imaginary;
		    set => LocalRatio = new Slug(value, LocalRatio.Real);
	    }
	    public float EndT
	    {
		    get => (float)LocalRatio.Real;
		    set => LocalRatio = new Slug(LocalRatio.Imaginary, value);
	    }
	    public float TRatio => (float)(EndFocal.LocalSlug.DirectedLength() / StartFocal.LocalSlug.DirectedLength());
        //public float TRatio => (float)LocalRatio.DirectedLength();

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
	    public bool HasFocal(int key) => StartKey == key || EndKey == key;
	    public Focal OtherFocal(Focal orgFocal) => (orgFocal.Key == StartKey) ? EndFocal : (orgFocal.Key == EndKey) ? StartFocal : Focal.Empty;
	    public SKSegment StartSegment => StartFocal.Segment;
	    public SKSegment EndSegment => EndFocal.Segment;
	    public override SKPoint Center => new SKSegment(StartFocal.MidPosition, EndFocal.MidPosition).Midpoint;

        public override bool HasArea => true;

	    public override List<IPoint> Points => new List<IPoint>() { StartFocal.StartFocalPoint, StartFocal.EndFocalPoint, EndFocal.StartFocalPoint, EndFocal.EndFocalPoint };

	    public DoubleBond(Focal startFocal, Focal endFocal) : base(startFocal.PadKind)
	    {
		    SetStartKey(startFocal.Key);
		    SetEndKey(endFocal.Key);
            CalculateRatio();
	    }
	    public override IElement Duplicate(bool addElement = true)
	    {
		    var dup = new DoubleBond(StartFocal, EndFocal);
		    if (addElement)
		    {
			    Pad.AddElement(dup);
		    }
		    return dup;
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
	    public override float DistanceToPoint(SKPoint point)
	    {
		    var startDist = StartFocal.DistanceToPoint(point);
		    var endDist = EndFocal.DistanceToPoint(point);
		    return startDist < endDist ? startDist : endDist;
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
        // todo: deal with area things on double bonds.
        public bool ContainsPosition(SKPoint point)
	    {
		    throw new NotImplementedException();
	    }

	    public void CalculateRatio()
	    {
		    if (!StartFocal.IsEmpty && !EndFocal.IsEmpty)
		    {
			    LocalRatio = EndFocal.LocalRatio / StartFocal.LocalRatio;
		    }
	    }

	    public void ApplyRatioRecursively(int fromFocalKey, HashSet<int> appliedBondKeys)
	    {
		    if (!appliedBondKeys.Contains(Key))
		    {
                int newFocalKey = -1;
                if (fromFocalKey == StartKey)
                {
	                newFocalKey = ApplyRatioToEnd();
					appliedBondKeys.Add(Key);
                }
                else if (fromFocalKey == EndKey)
                {
	                newFocalKey = ApplyRatioToStart();
	                appliedBondKeys.Add(Key);
                }

                if (newFocalKey >= 0)
                {
	                var connections = Pad.ConnectedDoubleBonds(this, appliedBondKeys);
	                foreach (var db in connections)
	                {
		                db.ApplyRatioRecursively(newFocalKey, appliedBondKeys);
	                }
                }
		    }
	    }
        public int ApplyRatioToEnd()
	    {
            EndFocal.LocalRatio = StartFocal.LocalRatio * LocalRatio;
            return EndKey;
	    }
	    public int ApplyRatioToStart()
	    {
		    StartFocal.LocalRatio = EndFocal.LocalRatio / LocalRatio;
		    return StartKey;
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

        public bool IsPointInside(SKPoint point)
        {
	        var result = false;
	        var vs = new [] {StartFocal.StartPosition, StartFocal.EndPosition, EndFocal.EndPosition, EndFocal.StartPosition};
	        for (int i = 0, j = vs.Length - 1; i < vs.Length; j = i++)
	        {
		        var xi = vs[i].X;
		        var yi = vs[i].Y;
		        var xj = vs[j].X;
		        var yj = vs[j].Y;

		        var intersect = ((yi > point.Y) != (yj > point.Y)) && (point.X < (xj - xi) * (point.Y - yi) / (yj - yi) + xi);
		        if (intersect) result = !result;
	        }
	        return result;
        }

        public static bool operator ==(DoubleBond left, DoubleBond right) =>
		    left.Key == right.Key && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
	    public static bool operator !=(DoubleBond left, DoubleBond right) =>
		    left.Key != right.Key || left.StartKey != right.StartKey || left.EndKey != right.EndKey;
	    public override bool Equals(object obj) => obj is DoubleBond value && this == value;
	    public override int GetHashCode() => 13 * Key + 17 * Key + 27 * StartKey + 29 * EndKey;
    }
}
