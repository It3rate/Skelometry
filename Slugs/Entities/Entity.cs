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

        private readonly HashSet<int> _focalKeys = new HashSet<int>();
	    private readonly HashSet<int> _doubleBondKeys = new HashSet<int>();

        public Entity(PadKind padKind) : base(padKind)
        {
        }

		#region Focals
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
		#region DoubleBonds
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
#endregion

        public override float DistanceToPoint(SKPoint point)
        {
	        return float.MaxValue; 
        }
    }
}
