using System.Collections.Generic;
using SkiaSharp;
using Slugs.Entities;
using Slugs.Extensions;
using Slugs.Slugs;
using IPoint = Slugs.Entities.IPoint;

namespace Slugs.Pads
{
	public class PadData
	{
		private EntityPad _pad;

        public readonly int PadIndex;
	    private static int _entityCounter = 1;
	    private static int _focalCounter = 1;

        private readonly Dictionary<int, Entity> _entityMap = new Dictionary<int, Entity>();
        public Entity EntityAt(int key) => _entityMap.ContainsKey(key) ? _entityMap[key] : Entity.Empty;
        public Entity GetOrCreateEntity(int key) => _entityMap.ContainsKey(key) ? _entityMap[key] : CreateEmptyEntity().Item2;

        private readonly Dictionary<int, Slug> _focalMap = new Dictionary<int, Slug>();
        public Slug FocalAt(int key) => HasPointIndex(key) ? _focalMap[key] : Slug.Empty;

	    private readonly Dictionary<int, IPoint> _pointMap = new Dictionary<int, IPoint>();
	    public IPoint PtRefAt(int key) => HasPointIndex(key) ? _pointMap[key] : VPoint.Empty;
	    public void SetPtRef(int key, IPoint value)
	    {
		    value.Kind = PointKind.Dirty;
		    _pointMap[key] = value;
	    }

	    public int KeyForPtRef(IPoint pt)
        {
	        var result = -1;
	        foreach (var kvp in _pointMap)
	        {
		        if (pt == kvp.Value)
		        {
			        result = kvp.Key;
			        break;
		        }
	        }
	        return result;
        }

        public PadData(int padIndex, EntityPad pad)
        {
            PadIndex = padIndex;
            _pad = pad;
        }
        public void Clear()
	    {
            _pointMap.Clear();
            _focalMap.Clear();
            _entityMap.Clear();
	    }
        // Contains doesn't check for <0 indexes because that represents the default cached point.
        public bool IsOwnPad(VPoint p) => p.PadIndex == PadIndex;
        public bool ContainsMap(VPoint p) => p.PadIndex == PadIndex && _entityMap.ContainsKey(p.EntityKey) && _focalMap.ContainsKey(p.FocalKey);
        public bool HasPointIndex(int key) => _pointMap.ContainsKey(key);

        //public (int, Entity, SegRef[]) CreateEntity(params SKSegment[] segments) => CreateEntity(ToSegRefs(segments));
        //public (int, Entity, SegRef[]) CreateEntity(params SegRef[] segRefs)
        //{
	       // var key = _entityCounter++;
	       // var entity = new Entity(segRefs);
	       // _entityMap.Add(key, entity);
	       // return (key, entity, segRefs);
        //}
        public (int, Entity) CreateEmptyEntity()
        {
	        var key = _entityCounter++;
	        var entity = new Entity(key);
	        _entityMap.Add(key, entity);
	        return (key, entity);
        }
        public int CreateFocal(Slug focal)
        {
	        var key = _focalCounter++;
	        _focalMap.Add(key, focal);
	        return key;
        }
        public bool AddBond(int entityKey, int fromSeg, int toSeg, Slug fromSlug, Slug toSlug)
        {
	        return false;
        }


        public SKPoint PointAt(int index) => PtRefFromIndex(index).SKPoint;

        public bool ReplacePointRef(IPoint source, IPoint value)
        {
	        var result = false;
	        foreach (var pair in _pointMap)
	        {
		        if (pair.Value == source)
		        {
			        _pointMap[pair.Key] = value;
					result = true;
					break;
		        }
	        }
            return result;
        }

	    public IPoint PtRefFromIndex(int index)
	    {
		    if (!_pointMap.TryGetValue(index, out var value))
		    {
			    value = VPoint.Empty;
		    }
		    return value;
	    }
	    public Entity EntityFromIndex(int index)
	    {
		    if (!_entityMap.TryGetValue(index, out var value))
		    {
			    value = Entity.Empty;
		    }
		    return value;
	    }
	    public Slug FocalFromIndex(int index)
	    {
		    if (!_focalMap.TryGetValue(index, out var value))
		    {
			    value = Slug.Empty;
		    }
		    return value;
	    }

	    public IEnumerable<IPoint> PtRefs
	    {
		    get
		    {
			    foreach (var pointRef in _pointMap.Values)
			    {
				    yield return pointRef;
			    }
		    }
	    }
	    public IEnumerable<Slug> Focals
	    {
		    get
		    {
			    foreach (var slug in _focalMap.Values)
			    {
				    yield return slug;
			    }
		    }
	    }
	    public IEnumerable<Entity> Entities
	    {
		    get
		    {
			    foreach (var entity in _entityMap.Values)
			    {
				    yield return entity;
			    }
		    }
	    }

        public Point CreateTerminalPoint(SKPoint pt)
	    {
		    var ptRef = new Point(PadIndex, pt);
		    _pointMap.Add(ptRef.Key, ptRef);
		    return ptRef;
	    }
	    public SegRef CreateTerminalSegRef(SKSegment skSegment)
	    {
		    var a = CreateTerminalPoint(skSegment.StartPoint);
		    var b = CreateTerminalPoint(skSegment.EndPoint);
		    return new SegRef(a, b);
	    }
	    public SegRef[] CreateTerminalSegRefs(params SKSegment[] segs)
	    {
		    var result = new List<SegRef>(segs.Length);
		    foreach (var skSegment in segs)
		    {
			    result.Add(CreateTerminalSegRef(skSegment));
		    }
		    return result.ToArray();
	    }
    }
}
