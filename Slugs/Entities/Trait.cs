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
	    public static readonly Trait Empty = new Trait();
        private Trait() : base(true) { EntityKey = Entity.Empty.Key; KindIndex = -1;}

	    public int KindIndex { get; }

	    private Entity _entity => Pad.EntityAt(EntityKey); // trait doesn't have entity as multiple entities can hold the same trait
        public int EntityKey { get; set; }

        public Trait(Entity entity, IPoint start, IPoint end, int traitKindIndex) : this(entity, start.Key, end.Key, traitKindIndex) {}

        public Trait(Entity entity, int startKey, int endKey, int traitKindIndex) : base(entity.PadKind, startKey, endKey)
        {
	        EntityKey = entity.Key;
		    KindIndex = traitKindIndex;
        }

        public VPoint VPointFrom(SKPoint point)
        {
	        var (t, projected) = TFromPoint(point);
            return new VPoint(_entity.PadKind, _entity.Key, Key, -1);
        }
        public static bool operator ==(Trait left, Trait right) => 
	        left.Key == right.Key && left.EntityKey == right.EntityKey && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
        public static bool operator !=(Trait left, Trait right) => 
	        left.Key != right.Key || left.EntityKey != right.EntityKey || left.StartKey != right.StartKey || left.EndKey != right.EndKey;
        public override bool Equals(object obj) => obj is Trait value && this == value;
        public override int GetHashCode() => Key * 17 + EntityKey * 23 + StartKey * 29 + EndKey * 31;
    }
}
