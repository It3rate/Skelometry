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
		private Pad _pad;

        public readonly int PadIndex;
	    private static int _entityCounter = 1;
	    private static int _focalCounter = 1;

        private readonly Dictionary<int, Entity> _entityMap = new Dictionary<int, Entity>();
        public Entity EntityAt(int key)
        {
	        var success = _entityMap.TryGetValue(key, out Entity result);
	        return success ? result : Entity.Empty;
        }
        public Entity GetOrCreateEntity(int key)
        {
	        var success = _entityMap.TryGetValue(key, out Entity result);
	        return success ? result : CreateEmptyEntity().Item2;
        }

        private readonly Dictionary<int, Slug> _focalMap = new Dictionary<int, Slug>();
        public Slug FocalAt(int key) 
        {
	        var success = _focalMap.TryGetValue(key, out Slug result);
	        return success? result : Slug.Empty;
        }

        public PadData(int padIndex, Pad pad)
        {
            PadIndex = padIndex;
            _pad = pad;
        }
        public void Clear()
	    {
            _focalMap.Clear();
            _entityMap.Clear();
	    }
        // Contains doesn't check for <0 indexes because that represents the default cached point.
        public bool IsOwnPad(VPoint p) => p.PadIndex == PadIndex;
        public bool ContainsMap(VPoint p) => p.PadIndex == PadIndex && _entityMap.ContainsKey(p.EntityKey) && _focalMap.ContainsKey(p.FocalKey);

        public (int, Entity) CreateEmptyEntity()
        {
	        var key = _entityCounter++;
	        var entity = new Entity(PadIndex, key);
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

    }
}
