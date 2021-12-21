using System.Collections;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenTK.Graphics.ES30;
using SkiaSharp;
using Slugs.Input;
using Slugs.Slugs;

namespace Slugs.Pads
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum PadKind{Working, Drawn}

    public class SlugPad : IEnumerable<(SkiaPolyline, Slug)>
    {
	    public static Slug ActiveSlug = Slug.Unit;
        private static List<Slug> Slugs = new List<Slug>(); // -1 is 'none' position, 0 is activeSlug.
        public const float SnapDistance = 10.0f;

	    public PadKind PadKind;
	    private static int _padIndexCounter = 0;
	    public readonly int PadIndex;
	    public PointRef Highlight = PointRef.Empty;
	    public PointRef HighlightLine = PointRef.Empty;

        public List<SkiaPolyline> Input = new List<SkiaPolyline>();
	    public List<SkiaPolyline> Output = new List<SkiaPolyline>();
        private List<SlugMap> Maps = new List<SlugMap>(); 

	    public SlugPad(PadKind padKind)
	    {
		    PadIndex = _padIndexCounter++;
            PadKind = padKind;
		    Clear();
	    }

	    public (SkiaPolyline, Slug) this[int index] => (PolylineFromIndex(index), SlugFromIndex(index));

	    public void Clear()
	    {
		    Input.Clear();
		    Slugs.Clear();
		    Maps.Clear();
            Slugs.Add(Slug.Zero); 
        }
	    public void Refresh()
	    {
            Output.Clear();
            foreach (var slugMap in Maps)
            {
	            if (slugMap.SlugIndex > -1)
	            {
		            var unit = SlugFromIndex(slugMap.SlugIndex);
		            var line = PolylineFromIndex(slugMap.PolyIndex);
		            var norm = unit / 10.0;
		            var multStart = line.PointAlongLine(0, norm.IsForward ? -(float)norm.Pull : (float)norm.Push);
		            var multEnd = line.PointAlongLine(0, norm.IsForward ? (float)norm.Push : -(float)norm.Pull);
		            Output.Add(new SkiaPolyline(multStart, multEnd));
	            }
            }
	    }

	    public void UpdatePoint(PointRef pointRef, SKPoint pt)
	    {
		    Input[pointRef.LineIndex][pointRef.PointIndex] = pt;
	    }

	    public SKPoint GetHighlightPoint() => Highlight.IsEmpty ? SKPoint.Empty : PolylineFromIndex(Highlight.LineIndex)[Highlight.PointIndex];
	    public SkiaPolyline GetHighlightLine() => HighlightLine.IsEmpty ? SkiaPolyline.Empty : PolylineFromIndex(HighlightLine.LineIndex);

        public PointRef[] GetSnapPoints(SKPoint input, float maxDist = SnapDistance)
	    {
		    var result = new List<PointRef>();
		    int lineIndex = 0;
		    foreach (var line in Input)
		    {
			    var ptIndex = line.GetSnapPoint(input, maxDist);
			    if (ptIndex > -1)
			    {
				    result.Add(new PointRef(PadIndex, lineIndex, ptIndex));
			    }
			    lineIndex++;

		    }
		    return result.ToArray();
	    }

	    public PointRef GetSnapLine(SKPoint point, float maxDist = SnapDistance)
	    {
		    var result = PointRef.Empty;
		    int lineIndex = 0;
		    foreach (var polyline in Input)
		    {
			    for (int i = 0; i < polyline.Points.Count - 1; i++)
			    {
				    var seg = new SkiaSegment(polyline, i, i+1);
				    var closest = seg.ProjectPointOnto(point);
				    var dist = point.SquaredDistanceTo(closest);
				    if (dist < maxDist)
				    {
                        result = new PointRef(this.PadIndex, lineIndex, i);
                        goto End;
				    }
					lineIndex++;
			    }
		    }
            End:
		    return result;
        }

        public SkiaPolyline PolylineFromIndex(int index)
	    {
		    SkiaPolyline result;
		    if (index >= 0 && index < Input.Count)
		    {
			    result = Input[index];
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
		    if(index == 0)
		    {
			    result = ActiveSlug;
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
		    Input.Add(line);
		    Slugs.Add(slug);
		    Maps.Add(new SlugMap(Input.Count - 1, Slugs.Count - 1));
		    return Slugs.Count - 1;
	    }
	    public int Add(SkiaPolyline line, int index)
	    {
		    Input.Add(line);
		    Maps.Add(new SlugMap(Input.Count - 1, index));
		    return index;
	    }
	    public int Add(SkiaPolyline line)
	    {
		    Input.Add(line);
		    Maps.Add(new SlugMap(Input.Count - 1, 0));
		    return -1;
	    }

        public IEnumerator<(SkiaPolyline, Slug)> GetEnumerator()
	    {
		    foreach (var map in Maps)
		    {
			    yield return (PolylineFromIndex(map.PolyIndex), SlugFromIndex(map.SlugIndex));
		    }
	    }
	    IEnumerator IEnumerable.GetEnumerator()
	    {
		    foreach (var map in Maps)
		    {
			    yield return (PolylineFromIndex(map.PolyIndex), SlugFromIndex(map.SlugIndex));
		    }
	    }
    }


    public readonly struct SlugMap
    {
        public static readonly SlugMap Empty = new SlugMap(-1,-1);
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
