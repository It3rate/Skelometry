using System.Collections.Generic;
using System.Linq;
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

        public override SKPath Path => new SKPath();

        // todo: also store focal and singleBond lists in entities and traits (globally stored in Pads). Need to sync add and remove as needed when traits are adjusted.
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
	    }
	    public void RemoveFocal(Focal focal)
	    {
		    _focalKeys.Remove(focal.Key);
	    }
	    private readonly HashSet<int> _doubleBondKeys = new HashSet<int>();
	    public IEnumerable<DoubleBond> DoubleBonds
	    {
		    get
		    {
			    foreach (var key in _doubleBondKeys)
			    {
				    yield return Pad.DoubleBondAt(key);
			    }
		    }
	    }
        public void AddDoubleBond(DoubleBond bond)
	    {
		    _doubleBondKeys.Add(bond.Key);
	    }
	    public void RemoveDoubleBond(DoubleBond bond)
	    {
		    _doubleBondKeys.Remove(bond.Key);
	    }
        public void EmbedTrait(Trait trait) => _traitKeys.Add(trait.Key);
	    public void EmbedTrait(int key) => _traitKeys.Add(key);
	    public void RemoveTrait(Trait trait) => _traitKeys.Remove(trait.Key);
	    public void RemoveTrait(int key) => _traitKeys.Remove(key);
	    public bool ContainsTrait(Trait trait) => _traitKeys.Contains(trait.Key);
	    public bool ContainsTrait(int traitKey) => _traitKeys.Contains(traitKey);

	    public Focal NearestFocal(SKPoint point, params int[] excludeKeys)
	    {
            var result = Focal.Empty;
            var dist = float.MaxValue;
            foreach (var focal in Focals)
            {
	            if (!(excludeKeys.Contains(focal.Key) || excludeKeys.Contains(focal.TraitKey)))
	            {
		            var fDist = focal.DistanceToPoint(point);
		            if (fDist < dist)
		            {
			            result = focal;
			            dist = fDist;
		            }
	            }
            }
            return result;
	    }
        #endregion


	    public Entity(PadKind padKind, params Trait[] traits) : base(padKind)
	    {
		    foreach (var trait in traits)
		    {
			    EmbedTrait(trait);
		    }
	    }
        public override float DistanceToPoint(SKPoint point)
        {
	        return float.MaxValue; // todo: Calculate distance to closest entity element.
        }
    }
}
