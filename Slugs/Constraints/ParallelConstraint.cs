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

    public class ParallelConstraint : TwoElementConstraintBase
    {
	    public Trait StartTrait => (Trait)StartElement;
	    public Trait EndTrait => (Trait)EndElement;

	    public ParallelConstraint(Trait startElement, Trait endElement) : base(startElement, endElement) { }

	    public override void OnStartChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    var sp = EndTrait.StartPoint;
		    var ep = EndTrait.EndPoint;
		    var canMoveStart = !sp.IsLocked && !adjustedElements.ContainsKey(sp.Key);
		    var canMoveEnd = !ep.IsLocked && !adjustedElements.ContainsKey(ep.Key);
		    var len = EndTrait.Length;
		    if (canMoveStart && canMoveEnd)
		    {
			    var segEnd = StartTrait.ProjectPointOnto(EndTrait.Center, false);
                var perpSeg = new SKSegment(EndTrait.Center, segEnd);
                sp.Position = perpSeg.OffsetAlongLine(0, -len / 2f);
                ep.Position = perpSeg.OffsetAlongLine(0, len / 2f);

                ep.Pad.UpdateConstraints(EndTrait, adjustedElements);
                //adjustedElements.Add(ep.Key, SKPoint.Empty);
                //sp.Pad.UpdateConstraints(sp, adjustedElements);
                //adjustedElements.Remove(ep.Key);
                //ep.Pad.UpdateConstraints(ep, adjustedElements);
            }
		    else if (canMoveStart)
		    {
                var segEnd = StartTrait.ProjectPointOnto(EndTrait.EndPosition, false);
                var perpSeg = new SKSegment(EndTrait.EndPosition, segEnd);
                sp.Position = perpSeg.OffsetAlongLine(0, -len);
                sp.Pad.UpdateConstraints(sp, adjustedElements);
            }
		    else if(canMoveEnd)
		    {
			    var segStart = StartTrait.ProjectPointOnto(EndTrait.StartPosition, false);
			    var perpSeg = new SKSegment(EndTrait.StartPosition, segStart);
			    ep.Position = perpSeg.OffsetAlongLine(0, len);
			    ep.Pad.UpdateConstraints(ep, adjustedElements);
            }
        }
	    public override void OnEndChanged(Dictionary<int, SKPoint> adjustedElements)
	    {
		    var sp = StartTrait.StartPoint;
		    var ep = StartTrait.EndPoint;
		    var canMoveStart = !sp.IsLocked && !adjustedElements.ContainsKey(sp.Key);
		    var canMoveEnd = !ep.IsLocked && !adjustedElements.ContainsKey(ep.Key);
		    var len = StartTrait.Length;
		    if (canMoveStart && canMoveEnd)
		    {
			    var segEnd = EndTrait.ProjectPointOnto(StartTrait.Center, false);
			    var perpSeg = new SKSegment(StartTrait.Center, segEnd);
			    sp.Position = perpSeg.OffsetAlongLine(0, -len / 2f);
			    ep.Position = perpSeg.OffsetAlongLine(0, len / 2f);

			    ep.Pad.UpdateConstraints(StartTrait, adjustedElements);

                //adjustedElements.Add(ep.Key, SKPoint.Empty);
                //sp.Pad.UpdateConstraints(sp, adjustedElements);
                //adjustedElements.Remove(ep.Key);
                //ep.Pad.UpdateConstraints(ep, adjustedElements);
            }
		    else if (canMoveStart)
		    {
			    var segEnd = EndTrait.ProjectPointOnto(StartTrait.EndPosition, false);
			    var perpSeg = new SKSegment(StartTrait.EndPosition, segEnd);
			    sp.Position = perpSeg.OffsetAlongLine(0, -len);
			    sp.Pad.UpdateConstraints(sp, adjustedElements);
            }
		    else if (canMoveEnd)
		    {
			    var segStart = EndTrait.ProjectPointOnto(StartTrait.StartPosition, false);
			    var perpSeg = new SKSegment(StartTrait.StartPosition, segStart);
			    ep.Position = perpSeg.OffsetAlongLine(0, len);
			    ep.Pad.UpdateConstraints(ep, adjustedElements);
            }


        }
    }
}
