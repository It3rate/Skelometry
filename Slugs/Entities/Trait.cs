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

        public IPoint StartPoint => Pad.PointAt(StartKey); // todo: make this base class for segment elements.
        public IPoint EndPoint => Pad.PointAt(EndKey);
        public override SKPoint StartPosition => StartPoint.Position;
	    public override SKPoint EndPosition => EndPoint.Position;

        public override List<IPoint> Points => IsEmpty ? new List<IPoint> { } : new List<IPoint> { StartPoint, EndPoint };

	    public int KindIndex { get; }
	    private Entity _entity => Pad.EntityAt(EntityKey); // trait doesn't have entity as multiple entities can hold the same trait
	    public int EntityKey { get; set; }



        public Trait(Entity entity, IPoint start, IPoint end, int traitKindIndex) : this(entity, start.Key, end.Key, traitKindIndex) {}
        public Trait(Entity entity, int startKey, int endKey, int traitKindIndex) : base(entity.PadKind)
        {
            EntityKey = entity.Key;
		    KindIndex = traitKindIndex;
		    SetStartKey(startKey);
		    SetEndKey(endKey);
        }

        protected override void SetStartKey(int key)
        {
	        if (Pad.ElementAt(key) is IPoint)
	        {
		        base.SetStartKey(key);
	        }
	        else
	        {
		        throw new ArgumentException("Trait points must be IPoint.");
	        }
        }
        protected override void SetEndKey(int key)
        {
	        if (Pad.ElementAt(key) is IPoint)
	        {
		        base.SetEndKey(key);
	        }
	        else
	        {
		        throw new ArgumentException("Trait points must be IPoint.");
	        }
        }

        private readonly HashSet<int> _focalKeys = new HashSet<int>();
        public IEnumerable<Focal> Focals
        {
	        get
	        {
		        foreach (var key in _focalKeys)
		        {
			        yield return Pad.FocalAt(key);
		        }
	        }
        }
        public void AddFocal(Focal focal)
        {
	        _focalKeys.Add(focal.Key);
	        _entity.AddFocal(focal);
        }
        public void RemoveFocal(Focal focal)
        {
	        _focalKeys.Remove(focal.Key);
	        _entity.RemoveFocal(focal);
        }

        public FocalPoint FocalPointFrom(SKPoint point)
        {
	        var (t, projected) = TFromPoint(point);
            return new FocalPoint(_entity.PadKind, Key, -1);
        }
        public static bool operator ==(Trait left, Trait right) => 
	        left.Key == right.Key && left.EntityKey == right.EntityKey && left.StartKey == right.StartKey && left.EndKey == right.EndKey;
        public static bool operator !=(Trait left, Trait right) => 
	        left.Key != right.Key || left.EntityKey != right.EntityKey || left.StartKey != right.StartKey || left.EndKey != right.EndKey;
        public override bool Equals(object obj) => obj is Trait value && this == value;
        public override int GetHashCode() => Key * 17 + EntityKey * 23 + StartKey * 29 + EndKey * 31;
    }
}
