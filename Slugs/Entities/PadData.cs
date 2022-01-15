using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using Slugs.Agent;
using Slugs.Extensions;
using Slugs.Input;
using Slugs.Slugs;

namespace Slugs.Entities
{
	public class PadData
	{
		private EntityPad _pad;

        public readonly int PadIndex;
	    private static int _entityCounter = 1;
	    private static int _pointCounter = 1;
	    private static int _focalCounter = 1;

        private readonly Dictionary<int, Entity> _entityMap = new Dictionary<int, Entity>();
        public Entity EntityAt(int key) => _entityMap.ContainsKey(key) ? _entityMap[key] : Entity.Empty;
        public Entity GetOrCreateEntity(int key) => _entityMap.ContainsKey(key) ? _entityMap[key] : CreateEmptyEntity().Item2;

        private readonly Dictionary<int, Slug> _focalMap = new Dictionary<int, Slug>();
        public Slug FocalAt(int key) => HasPointIndex(key) ? _focalMap[key] : Slug.Empty;

	    private readonly Dictionary<int, IPointRef> _pointMap = new Dictionary<int, IPointRef>();
	    public IPointRef PtRefAt(int key) => HasPointIndex(key) ? _pointMap[key] : PtRef.Empty;
	    public IPointRef SetPtRef(int key, IPointRef value) => _pointMap[key] = value;

        public int KeyForPtRef(IPointRef ptRef)
        {
	        var result = -1;
	        foreach (var kvp in _pointMap)
	        {
		        if (ptRef == kvp.Value)
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
        public bool IsOwnPad(PtRef p) => p.PadIndex == PadIndex;
        public bool ContainsMap(PtRef p) => p.PadIndex == PadIndex && _entityMap.ContainsKey(p.EntityKey) && _focalMap.ContainsKey(p.FocalKey);
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

        public bool ReplacePointRef(IPointRef source, IPointRef value)
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

	    public IPointRef PtRefFromIndex(int index)
	    {
		    if (!_pointMap.TryGetValue(index, out var value))
		    {
			    value = PtRef.Empty;
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

	    public IEnumerable<IPointRef> PtRefs
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

        public (int, PtRef) CreateTerminalPoint(SKPoint pt)
	    {
		    var key = _pointCounter++;
		    var ptRef = new PtRef(PadIndex, -1, -1, -1, pt);
		    _pointMap.Add(key, ptRef);
		    return (key, ptRef);
	    }
	    public SegRef CreateTerminalSegRef(SKSegment skSegment)
	    {
		    var (aKey, aVal) = CreateTerminalPoint(skSegment.StartPoint);
		    var (bKey, bVal) = CreateTerminalPoint(skSegment.EndPoint);
		    return new SegRef(aVal, bVal);
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
