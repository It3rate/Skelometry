using System.Collections.Generic;
using SkiaSharp;
using Slugs.Pads;

namespace Slugs.Entities
{
	public class Entity : ElementBase
    {
	    public override ElementKind ElementKind => ElementKind.Entity;
	    public override IElement EmptyElement => Empty;
	    public static Entity Empty = new Entity();

        // todo: traits should be in their own list as they can be shared by many entities. Maybe just a trait kind index, and the segmentBase of it is local.

        public override IPoint[] Points => new IPoint[] {};

        #region Traits
        private readonly HashSet<int> _traitKeys = new HashSet<int>();
	    public IEnumerable<Trait> Traits
        {
		    get
		    {
			    foreach (var key in _traitKeys)
			    {
				    yield return Pad.TraitAt(key);
			    }
		    }
	    }
	    public void EmbedTrait(Trait trait) => _traitKeys.Add(trait.Key);
	    public void EmbedTrait(int key) => _traitKeys.Add(key);
	    public void RemoveTrait(Trait trait) => _traitKeys.Remove(trait.Key);
	    public void RemoveTrait(int key) => _traitKeys.Remove(key);
	    public bool ContainsTrait(Trait trait) => _traitKeys.Contains(trait.Key);
	    public bool ContainsTrait(int traitKey) => _traitKeys.Contains(traitKey);

        #endregion

        #region Bonds

        private List<Bond> _bonds { get; } = new List<Bond>(); // Interactions
	    public IEnumerable<Bond> Bonds => _bonds;
	    public Bond BondAt(int key) => _bonds[key];

        #endregion

	    private Entity() : base(PadKind.None) { }
        public Entity(PadKind padKind, params Trait[] traits) : base(padKind)
        {
		    foreach (var trait in traits)
		    {
			    EmbedTrait(trait);
		    }
	    }
    }
}
