using System.Collections.Generic;
using SkiaSharp;
using Slugs.Slugs;
using SegRef = Slugs.Entities.SegRef;

namespace Slugs.Entities
{
	public class Entity
    {
        public static Entity Empty = new Entity(-1);
        public bool IsEmpty => (Key == -1);

        private int Key { get; }

        // todo: traits should be in their own list as they can be shared by many entities. Maybe just a trait kind index, and the segRef of it is local.

        private static int _traitCounter;
        private Dictionary<int, Trait> _traits { get; } = new Dictionary<int, Trait>(); 
        public bool HasTraits => _traits.Count > 0;
	    public IEnumerable<Trait> Traits => _traits.Values;
        public Trait TraitAt(int key) => _traits[key];
        public void EmbedTrait(Trait trait)
        {
	        var traitKey = _traitCounter++;
	        //trait.StartKey.EntityKey = Key;
	        //trait.StartKey.TraitKey = traitKey;
	        //trait.EndKey.EntityKey = Key;
         //   trait.EndKey.TraitKey = traitKey;
            _traits.Add(traitKey, trait);
        }
        public void LinkTrait(Trait trait)
        {
	        var traitKey = _traitCounter++;
	        _traits.Add(traitKey, trait);
        }

        private List<Bond> _bonds { get; } = new List<Bond>(); // Interactions
	    public IEnumerable<Bond> Bonds => _bonds;
        public Bond BondAt(int key) => _bonds[key];

        public Entity(int entityKey, params Trait[] segs)
        {
	        Key = entityKey;
		    foreach (var segRef in segs)
		    {
			    EmbedTrait(segRef);
		    }
	    }


        public SKPoint GetPointAt(Slug t)
	    {
		    SKPoint result;
		    if (_traits.Count > 0)
		    {
			    result = _traits[0].PointAlongLine(t.Natural);
		    }
		    else
		    {
                result = SKPoint.Empty;
		    }
		    return result;
	    }
	    public bool SetPointAt(Slug t, SKPoint value)
	    {
		    bool result = false;
		    if (_traits.Count > 0)
		    {
			    if (t.End == 0)
			    {
				    _traits[0].StartPoint = value; // todo: make point set return true if success
				    result = true;
			    }
                else if (t.End == 1)
			    {
				    _traits[1].StartPoint = value;
				    result = true;
                }
		    }
		    return result;
	    }
    }
}
