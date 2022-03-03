using SkiaSharp;
using Slugs.Entities;
using Slugs.Primitives;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum LengthLock
    {
	    None,
	    Equal,
	    Ratio,
    }
    public enum DirectionLock
    {
	    None,
	    Parallel,
	    Perpendicular,
    }
    public class EqualConstraint : TwoElementConstraintBase
    {
	    public Trait StartTrait => (Trait)StartElement;
	    public Trait EndTrait => (Trait)EndElement;
        public LengthLock LengthLock { get; }
        public DirectionLock DirectionLock { get; }
        private float Ratio { get; }


        public EqualConstraint(Trait startElement, Trait endElement, LengthLock lengthLock, DirectionLock directionLock) : base(startElement, endElement)
        {
	        LengthLock = lengthLock;
	        DirectionLock = directionLock;
	        Ratio = EndTrait.Length / StartTrait.Length;
	        if (LengthLock == LengthLock.None && DirectionLock == DirectionLock.None)
	        {
                throw new ArgumentException("Equal constraint does nothing.");
	        }
        }

        public override void OnStartChanged(Dictionary<int, SKPoint> adjustedElements)
        {
	        AddPoints(StartTrait, adjustedElements);
	        AdjustLengthAndDirection(StartTrait, EndTrait, Ratio, adjustedElements);
        }
	    public override void OnEndChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    AddPoints(EndTrait, adjustedElements);
		    AdjustLengthAndDirection(EndTrait, StartTrait, 1f / Ratio, adjustedElements);
        }

        private void AdjustLengthAndDirection(Trait changed, Trait target, float ratio, Dictionary<int, SKPoint> adjustedElements)
        {
	        var sp = target.StartPoint;
	        var ep = target.EndPoint;
	        var canMoveStart = !sp.IsLocked && !adjustedElements.ContainsKey(sp.Key);
	        var canMoveEnd = !ep.IsLocked && !adjustedElements.ContainsKey(ep.Key);
	        var dirSeg = DirectionLock == DirectionLock.None ? target.Segment : changed.Segment;
	        var len = LengthLock == LengthLock.None ? target.Length : changed.Length;
	        if (canMoveStart && canMoveEnd) 
	        {
		        var curSegMid = target.Segment.Midpoint;
		        var dif = NormDiff(dirSeg.Midpoint, dirSeg.StartPoint, len / 2f, ratio);

		        sp.Position = curSegMid - dif;
		        ep.Position = curSegMid + dif;
		        ep.Pad.UpdateConstraints(target, adjustedElements);
            }
            else if (canMoveStart)
            {
	            var curEnd = target.Segment.EndPoint;
	            var dif = NormDiff(dirSeg.EndPoint, dirSeg.StartPoint, len, ratio);

                sp.Position = curEnd - dif;
                sp.Pad.UpdateConstraints(sp, adjustedElements);
            }
            else if (canMoveEnd)
            {
	            var curStart = target.Segment.StartPoint;
	            var dif = NormDiff(dirSeg.EndPoint, dirSeg.StartPoint, len, ratio);

                ep.Position = curStart + dif;
	            ep.Pad.UpdateConstraints(ep, adjustedElements);
            }
        }
        private SKPoint NormDiff(SKPoint end, SKPoint start, float len, float ratio)
        {
	        var diff = end - start;
	        var finalLen = LengthLock == LengthLock.Ratio ? len * ratio : len;
	        var result = diff.Normalize().Multiply(finalLen);
            // todo: keep direction of perp line
            return DirectionLock == DirectionLock.Perpendicular ? result.RotateOnOrigin(-90) : result;
        }
    }
}
