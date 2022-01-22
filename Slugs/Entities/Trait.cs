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

    public class Trait : IElement
    {
	    public ElementKind ElementKind => ElementKind.Trait;

        private static int _counter = 0;

        public static Trait Empty = new Trait( SegRef.Empty, Entity.Empty, -1);
        public bool IsEmpty => KindIndex == -1;

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

        public int Key { get; }
        private Entity _entity { get; }
        public int EntityKey => _entity.Key;

        public Trait(SegRef segRef, Entity entity, int traitKindIndex)
        {
	        Key = _counter++;
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
