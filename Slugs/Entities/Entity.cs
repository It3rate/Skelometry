using SkiaSharp;
using Slugs.Input;
using Slugs.Slugs;

namespace Slugs.Motors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Entity
    {
        public static Entity Empty = new Entity();
        public bool IsEmpty => Traits.Count == 0;

	    private List<SegRef> Traits { get; } = new List<SegRef>();
	    private List<Bond> Interactions { get; } = new List<Bond>();

	    public SKPoint GetPointAt(Slug t)
	    {
		    SKPoint result;
		    if (Traits.Count > 0)
		    {
			    result = Traits[0].PointAlongLine(t.Natural);
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
		    if (Traits.Count > 0)
		    {
			    if (t.End == 0)
			    {
				    Traits[0].StartPoint = value; // todo: make point set return true if success
				    result = true;
			    }
                else if (t.End == 1)
			    {
				    Traits[1].StartPoint = value;
				    result = true;
                }
		    }
		    return result;
	    }
    }
}
