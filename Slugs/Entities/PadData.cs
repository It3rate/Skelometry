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
	    private readonly Dictionary<int, Slug> _focalMap = new Dictionary<int, Slug>();
	    private readonly Dictionary<int, IPointRef> _pointMap = new Dictionary<int, IPointRef>();

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

        public IPointRef PtRefAt(int key) => HasPointIndex(key) ? _pointMap[key] : PtRef.Empty;
        public Entity EntityAt(int key) => HasPointIndex(key) ? _entityMap[key] : Entity.Empty;
        public Slug FocalAt(int key) => HasPointIndex(key) ? _focalMap[key] : Slug.Empty;

        public int CreatePtRef(SKPoint point)
        {
	        var ptRef = new PtRef(PadIndex, -1, -1, point);
	        var key = _pointCounter++;
            _pointMap.Add(key, ptRef);
            return key;
        }
        public int CreateEntity(params SKSegment[] segments) => CreateEntity(ToSegRefs(segments));
        public int CreateEntity(params SegRef[] segRefs)
        {
	        var key = _entityCounter++;
	        var entity = new Entity(segRefs);
	        _entityMap.Add(key, entity);
	        return key;
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

        private static SegRef[] ToSegRefs(SKSegment[] segs)
	    {
		    var result = new List<SegRef>(segs.Length);
		    foreach (var skSegment in segs)
		    {
			    var a = new PtRef(_pointCounter++, -1, -1, skSegment.StartPoint);
			    var b = new PtRef(_pointCounter++, -1, -1, skSegment.StartPoint);
			    result.Add(new SegRef(a, b));
		    }
		    return result.ToArray();
	    }
    }
}
