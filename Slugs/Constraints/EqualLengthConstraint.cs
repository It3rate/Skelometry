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

    public class EqualLengthConstraint : TwoElementConstraintBase
    {
	    public Trait StartFocal => (Trait)StartElement;
	    public Trait EndFocal => (Trait)EndElement;
	    public bool AlsoAdjustDirection { get; set; } = false;

	    public EqualLengthConstraint(Trait startElement, Trait endElement) : base(startElement, endElement) { }


	    public override void OnStartChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    if (AlsoAdjustDirection)
		    {
			    AdjustLengthAndDirection(StartFocal, EndFocal, adjustedElements);
            }
		    else
		    {
			    AdjustLength(StartFocal, EndFocal, adjustedElements);
            }
        }
	    public override void OnEndChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    if (AlsoAdjustDirection)
		    {
			    AdjustLengthAndDirection(EndFocal, StartFocal, adjustedElements);
            }
		    else
		    {
			    AdjustLength(EndFocal, StartFocal, adjustedElements);
            }
        }
        private void AdjustLengthAndDirection(Trait changed, Trait target, Dictionary<int, SKPoint> adjustedElements)
        {
	        var sp = target.StartPoint;
	        var ep = target.EndPoint;
	        var canMoveStart = !sp.IsLocked && !adjustedElements.ContainsKey(sp.Key);
	        var canMoveEnd = !ep.IsLocked && !adjustedElements.ContainsKey(ep.Key);
	        var oppSeg = changed.Segment;
	        if (canMoveStart && canMoveEnd)
	        {
		        var curSegMid = target.Segment.Midpoint;
		        var dif = oppSeg.Midpoint - oppSeg.StartPoint;

		        sp.Position = curSegMid - dif;
		        ep.Position = curSegMid + dif;

		        ep.Pad.UpdateConstraints(target, adjustedElements);
	        }
            else if (canMoveStart)
            {
	            var curEnd = target.Segment.EndPoint;
	            var dif = oppSeg.EndPoint - oppSeg.StartPoint;
                sp.Position = curEnd - dif;
                sp.Pad.UpdateConstraints(sp, adjustedElements);
            }
            else if (canMoveEnd)
            {
	            var curStart = target.Segment.StartPoint;
	            var dif = oppSeg.EndPoint - oppSeg.StartPoint;
	            ep.Position = curStart + dif;
	            ep.Pad.UpdateConstraints(ep, adjustedElements);
            }
        }
        private void AdjustLength(Trait changed, Trait target, Dictionary<int, SKPoint> adjustedElements)
        {
	        var changedLen = changed.Length;
	        var sp = target.StartPoint;
	        var ep = target.EndPoint;
	        var canMoveStart = !sp.IsLocked && !adjustedElements.ContainsKey(sp.Key);
	        var canMoveEnd = !ep.IsLocked && !adjustedElements.ContainsKey(ep.Key);
            var seg = new SKSegment(sp.Position, ep.Position);
	        var lenRatio = changedLen / target.Length;
	        //var dif = ep.Position - sp.Position;
	        //var angle = (float)Math.Atan2(dif.Y, dif.X);

	        if (canMoveStart && canMoveEnd)
	        {
		        sp.Position = seg.PointAlongLine(0.5f - lenRatio / 2f);
		        ep.Position = seg.PointAlongLine(0.5f + lenRatio / 2f);
		        sp.Pad.UpdateConstraints(target, adjustedElements);
	        }
	        else if (canMoveStart)
	        {
		        ep.Position = seg.PointAlongLine(lenRatio);
		        ep.Pad.UpdateConstraints(ep, adjustedElements);
            }
	        else if (canMoveEnd)
	        {
		        sp.Position = seg.PointAlongLine(1f - lenRatio);
		        sp.Pad.UpdateConstraints(sp, adjustedElements);
            }
        }
    }
}
