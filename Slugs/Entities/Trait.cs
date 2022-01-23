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

    public class Trait : SegmentBase
    {
	    public override ElementKind ElementKind => ElementKind.Trait;
	    public override IElement EmptyElement => Empty;
	    public static Trait Empty = new Trait();

	    public int KindIndex { get; }

	    private Entity _entity { get; }
        public int EntityKey => _entity.Key;

        private Trait() : base(true)
        {
            _entity = Entity.Empty;
            KindIndex = -1;
        }
        public Trait(IPoint start, IPoint end, Entity entity, int traitKindIndex) : this(start.Key, end.Key, entity, traitKindIndex) {}

        public Trait(int startKey, int endKey, Entity entity, int traitKindIndex) : base(entity.PadKind, startKey, endKey)
        {
	        _entity = entity;
		    KindIndex = traitKindIndex;
        }

        public VPoint VPointFrom(SKPoint point)
        {
	        var (t, projected) = TFromPoint(point);
            return new VPoint(_entity.PadKind, _entity.Key, Key, -1);
        }
    }
}
