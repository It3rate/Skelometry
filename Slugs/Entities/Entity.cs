using System.Collections.Generic;
using SkiaSharp;
using Slugs.Pads;
using SegRef = Slugs.Primitives.SegRef;

namespace Slugs.Entities
{
	public class Entity : IElement
    {
        public static Entity Empty = new Entity(PadKind.None, -1);
        public bool IsEmpty => (Key == -1);

        public PadKind PadKind { get; }
        public int Key { get; }

        // todo: traits should be in their own list as they can be shared by many entities. Maybe just a trait kind index, and the segRef of it is local.

		#region Traits

	    private Dictionary<int, Trait> _traits { get; } = new Dictionary<int, Trait>(); 
	    public bool HasTraits => _traits.Count > 0;
	    public IEnumerable<Trait> Traits => _traits.Values;
	    public Trait TraitAt(int key)
	    {
		    var success = _traits.TryGetValue(key, out var result);
		    return success ? result : Trait.Empty;
	    }
	    public void EmbedTrait(Trait trait)
	    {
		    _traits.Add(trait.Key, trait);
	    }
	    public void LinkTrait(Trait trait)
	    {
		    _traits.Add(trait.Key, trait);
	    }

#endregion

		#region Bonds

	    private List<Bond> _bonds { get; } = new List<Bond>(); // Interactions
	    public IEnumerable<Bond> Bonds => _bonds;
	    public Bond BondAt(int key) => _bonds[key];

#endregion

        public Entity(PadKind padKind, int entityKey, params Trait[] segs)
        {
	        PadKind = padKind;
	        Key = entityKey;
		    foreach (var segRef in segs)
		    {
			    EmbedTrait(segRef);
		    }
	    }
    }
}
