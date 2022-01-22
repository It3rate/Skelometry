using SkiaSharp;
using Slugs.Primitives;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct TraitKind{}

    public class Trait : ElementBase
    {
	    public new ElementKind ElementKind => ElementKind.Trait;
	    public new IElement EmptyElement => Empty;
	    public static Trait Empty = new Trait();

	    public SegRef SegRef { get; }
	    public int KindIndex { get; }

        // Trait default properties
        public IPoint StartRef => SegRef.StartRef;
        public IPoint EndRef  => SegRef.EndRef;
        
        public SKSegment Segment => SegRef.Segment;
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

        private Entity _entity { get; }
        public int EntityKey => _entity.Key;

        private Trait() : base(true)
        {
            _entity = Entity.Empty;
            SegRef = SegRef.Empty;
            KindIndex = -1;
        }
        public Trait(SegRef segRef, Entity entity, int traitKindIndex) : base(entity.PadKind)
        {
	        _entity = entity;
		    SegRef = segRef;
		    KindIndex = traitKindIndex;
	    }

        public SKPoint PointAlongLine(float t) =>  SegRef.PointAlongLine(t);
        public SKPoint ProjectPointOnto(SKPoint p) => SegRef.ProjectPointOnto(p);
        public (float, SKPoint) TFromPoint(SKPoint point) => SegRef.TFromPoint(point);

        public VPoint VPointFrom(SKPoint point)
        {
	        var (t, projected) = TFromPoint(point);
            return new VPoint(_entity.PadKind, _entity.Key, Key, -1);
        }
    }
}
