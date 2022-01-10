using Slugs.Slugs;

namespace Slugs.Motors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PadData
    {
	    public readonly int PadIndex;
	    private readonly Dictionary<int, PtRef> _pointMap = new Dictionary<int, PtRef>();
	    private readonly Dictionary<int, Slug> _tMap = new Dictionary<int, Slug>();
	    private readonly Dictionary<int, Entity> _motorMap = new Dictionary<int, Entity>();

	    public void Clear()
	    {
            _pointMap.Clear();
            _tMap.Clear();
            _motorMap.Clear();
	    }

	    public Slug TFromIndex(int index)
	    {
		    if (!_tMap.TryGetValue(index, out var value))
		    {
			    value = Slug.Empty;
		    }
		    return value;
	    }
	    public PtRef PtRefFromIndex(int index)
	    {
		    if (!_pointMap.TryGetValue(index, out var value))
		    {
			    value = PtRef.Empty;
		    }
		    return value;
	    }
	    public Entity MotorFromIndex(int index)
	    {
		    if (!_motorMap.TryGetValue(index, out var value))
		    {
			    value = Entity.Empty;
		    }
		    return value;
	    }
    }
}
