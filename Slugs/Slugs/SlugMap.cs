namespace Slugs.Slugs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public readonly struct SlugMap
    {
        // pointRef0, pointRef1, Linkages (
	    public static readonly SlugMap Empty = new SlugMap(-1, -1);
	    public int PolyIndex { get; }
	    public int SlugIndex { get; }

	    public SlugMap(int polyIndex, int slugIndex)
	    {
		    PolyIndex = polyIndex;
		    SlugIndex = slugIndex;
	    }
	    public bool IsEmpty => PolyIndex == -1 && SlugIndex == -1;
    }

}
