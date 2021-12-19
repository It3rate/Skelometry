using System.Collections;
using System.Windows.Shapes;
using Slugs.Input;
using Slugs.Slugs;

namespace Slugs.Pads
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public struct SlugMap
    {
	    public int PolyIndex { get; }
	    public int SlugIndex { get; }

	    public SlugMap(int polyIndex, int slugIndex)
	    {
		    PolyIndex = polyIndex;
		    SlugIndex = slugIndex;
	    }
    }
    public class SlugPad : IEnumerable<(SkiaPolyline, Slug)>
    {
        public Slug PadSlug;
	    public List<SkiaPolyline> Polylines = new List<SkiaPolyline>();
	    private List<Slug> Slugs = new List<Slug>();
	    private List<SlugMap> Map = new List<SlugMap>();

	    public SlugPad()
	    {
		    PadSlug = Slug.Unit;
	    }

	    public (SkiaPolyline, Slug) this[int index] => (PolylineFromIndex(index), SlugFromIndex(index));
	    
	    public SkiaPolyline PolylineFromIndex(int index)
	    {
		    SkiaPolyline result;
		    if (index >= 0 && index < Polylines.Count)
		    {
			    result = Polylines[index];
		    }
		    else
		    {
			    result = SkiaPolyline.Empty;
		    }
		    return result;
	    }

	    public Slug SlugFromIndex(int index)
	    {
		    Slug result;
		    if(index == -1)
		    {
			    result = PadSlug;
		    }
		    else if (index >= 0 && index < Slugs.Count)
		    {
			    result = Slugs[index];
		    }
		    else
		    {
			    result = Slug.Zero;
		    }
		    return result;
	    }
        public int Add(SkiaPolyline line, Slug slug)
	    {
		    var result = Slugs.Count;
		    Map.Add(new SlugMap(Polylines.Count, result));
		    Polylines.Add(line);
		    Slugs.Add(slug);
		    return result;
	    }
	    public void Add(SkiaPolyline line, int index)
	    {
		    Polylines.Add(line);
		    Map.Add(new SlugMap(Polylines.Count, index));
	    }
	    public void Add(SkiaPolyline line)
	    {
		    Polylines.Add(line);
		    Map.Add(new SlugMap(Polylines.Count, -1));
	    }

	    public IEnumerator<(SkiaPolyline, Slug)> GetEnumerator()
	    {
		    foreach (var map in Map)
		    {
			    yield return (PolylineFromIndex(map.PolyIndex), SlugFromIndex(map.SlugIndex));
		    }
	    }
	    IEnumerator IEnumerable.GetEnumerator()
	    {
		    foreach (var map in Map)
		    {
			    yield return (PolylineFromIndex(map.PolyIndex), SlugFromIndex(map.SlugIndex));
		    }
	    }
    }
}
