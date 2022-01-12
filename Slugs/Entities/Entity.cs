using System.Collections.Generic;
using SkiaSharp;
using Slugs.Slugs;
using SegRef = Slugs.Entities.SegRef;

namespace Slugs.Entities
{
	public class Entity
    {
        public static Entity Empty = new Entity();
        public bool IsEmpty => _traits.Count == 0;

        // todo: traits should be in their own list as they can be shared by many entities. Maybe just a trait kind index, and the segRef of it is local.
        
        private List<SegRef> _traits { get; } = new List<SegRef>(); 
	    private List<Bond> _bonds { get; } = new List<Bond>(); // Interactions

	    public IEnumerable<SegRef> Traits => _traits;
	    public IEnumerable<Bond> Bonds => _bonds;

        public Entity(params SegRef[] segs)
	    {
            _traits.AddRange(segs);
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
