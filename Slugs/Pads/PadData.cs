using System.Collections.Generic;
using SkiaSharp;
using Slugs.Entities;
using Slugs.Primitives;
using IPoint = Slugs.Entities.IPoint;

namespace Slugs.Pads
{
	public class PadData
	{
		private Pad _pad;

        public readonly PadKind PadKind;
	    //private static int _entityCounter = 1;
	    //private static int _focalCounter = 1;

        //private readonly Dictionary<int, Entity> _entityMap = new Dictionary<int, Entity>();
        //public Entity EntityAt(int key)
        //{
	       // var success = _entityMap.TryGetValue(key, out var result);
	       // return success ? result : Entity.Empty;
        //}
        //public Entity GetOrCreateEntity(int key)
        //{
	       // var success = _entityMap.TryGetValue(key, out var result);
	       // return success ? result : CreateEmptyEntity().Item2;
        //}

        private readonly Dictionary<int, Focal> _focalMap = new Dictionary<int, Focal>();
        public Focal FocalAt(int key) 
        {
	        var success = _focalMap.TryGetValue(key, out var result);
	        return success? result : Focal.Empty;
        }

        public PadData(PadKind padKind, Pad pad)
        {
            PadKind = padKind;
            _pad = pad;
        }
        public void Clear()
	    {
            _focalMap.Clear();
            //_entityMap.Clear();
	    }
        // Contains doesn't check for <0 indexes because that represents the default cached point.
        public bool IsOwnPad(VPoint p) => p.PadKind == PadKind;
        //public bool ContainsMap(VPoint p) => p.PadKind == PadKind && _entityMap.ContainsKey(p.EntityKey) && _focalMap.ContainsKey(p.FocalKey);

        //public (int, Entity) CreateEmptyEntity()
        //{
	       // var key = _entityCounter++;
	       // var entity = new Entity(PadKind);
	       // _entityMap.Add(key, entity);
	       // return (key, entity);
        //}
        //public int CreateFocal(Focal focal)
        //{
	       // var key = _focalCounter++;
	       // _focalMap.Add(key, focal);
	       // return key;
        //}
        //public bool AddBond(int entityKey, int fromSeg, int toSeg, Slug fromSlug, Slug toSlug)
        //{
	       // return false;
        //}

	    //public Entity EntityFromKey(int index)
	    //{
		   // if (!_entityMap.TryGetValue(index, out var value))
		   // {
			  //  value = Entity.Empty;
		   // }
		   // return value;
	    //}
	    //public Focal FocalFromKey(int index)
	    //{
		   // if (!_focalMap.TryGetValue(index, out var value))
		   // {
			  //  value = Focal.Empty;
		   // }
		   // return value;
	    //}

	    public IEnumerable<Focal> Focals
	    {
		    get
		    {
			    foreach (var focal in _focalMap.Values)
			    {
				    yield return focal;
			    }
		    }
	    }
	    //public IEnumerable<Entity> Entities
	    //{
		   // get
		   // {
			  //  foreach (var entity in _entityMap.Values)
			  //  {
				 //   yield return entity;
			  //  }
		   // }
	    //}

    }
}
