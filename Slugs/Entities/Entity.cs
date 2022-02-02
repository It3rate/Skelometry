using System.Collections.Generic;
using SkiaSharp;
using Slugs.Pads;

namespace Slugs.Entities
{
	public class Entity : ElementBase
    {
	    public override ElementKind ElementKind => ElementKind.Entity;
	    public override IElement EmptyElement => Empty;
	    public static readonly Entity Empty = new Entity();
	    private Entity() : base(true) { }

        public override List<IPoint> Points => new List<IPoint> { };

        // todo: also store focal and bond lists in entities and traits (globally stored in Pads). Need to sync add and remove as needed when traits are adjusted.
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
		    focal.Trait.AddFocal(focal);
	    }
	    public void RemoveFocal(Focal focal)
	    {
		    _focalKeys.Remove(focal.Key);
		    focal.Trait.RemoveFocal(focal);
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

        public Entity(PadKind padKind, params Trait[] traits) : base(padKind)
        {
		    foreach (var trait in traits)
		    {
			    EmbedTrait(trait);
		    }
	    }
    }
}
