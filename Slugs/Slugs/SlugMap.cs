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
	    public int DataMapIndex { get; }
	    public int SlugIndex { get; }

	    public SlugMap(int dataMapIndex, int slugIndex)
	    {
		    DataMapIndex = dataMapIndex;
		    SlugIndex = slugIndex;
	    }
	    public bool IsEmpty => DataMapIndex == -1 && SlugIndex == -1;
    }

}
