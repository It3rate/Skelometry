using SkiaSharp;
using Slugs.Extensions;
using Slugs.Slugs;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct TraitKind{}

    public class Trait
    {
        public static Trait Empty = new Trait(SegRef.Empty, -1);
        public bool IsEmpty => KindIndex == -1;

	    public SegRef SegRef { get; }
	    public int KindIndex { get; }

        // Trait default properties
        public IPoint Start
        {
	        get => SegRef.Start;
	        set => throw new NotImplementedException();
        }
        public IPoint End
        {
	        get => SegRef.End;
	        set => throw new NotImplementedException();
        }
        public SKPoint StartPoint
        {
	        get => SegRef.StartPoint;
	        set => SegRef.StartPoint = value;
        }
        public SKPoint EndPoint
        {
	        get => SegRef.EndPoint;
	        set => SegRef.EndPoint = value;
        }
        public SKSegment Segment => SegRef.Segment;

        public Trait(SegRef segRef, int traitKindIndex)
	    {
		    SegRef = segRef;
		    KindIndex = traitKindIndex;
	    }

        public SKPoint PointAlongLine(float t) =>  SegRef.PointAlongLine(t);
        public SKPoint ProjectPointOnto(SKPoint p) => SegRef.ProjectPointOnto(p);

    }
}
