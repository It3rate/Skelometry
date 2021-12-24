using System.Collections;
using System.Runtime.CompilerServices;
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

    public class SlugPad : IEnumerable<(InfoSet, Slug)>
    {
	    private static int _padIndexCounter = 0;
	    public readonly int PadIndex;

	    public static Slug ActiveSlug = Slug.Unit;
        private static List<Slug> Slugs = new List<Slug>(); // -1 is 'none' position, 0 is activeSlug.
        public const float SnapDistance = 10.0f;

	    public PadKind PadKind;
	    public PointRef Highlight = PointRef.Empty;
	    public PointRef HighlightLine = PointRef.Empty;

        private readonly List<SlugMap> _maps = new List<SlugMap>(); 
        private readonly List<InfoSet> _input = new List<InfoSet>();
        private readonly List<SKSegment> _output = new List<SKSegment>();
        public IEnumerable<InfoSet> Input => _input;
        public IEnumerable<SKSegment> Output => _output;

        public SlugPad(PadKind padKind)
	    {
		    PadIndex = _padIndexCounter++;
            PadKind = padKind;
		    Clear();
	    }

	    public (InfoSet, Slug) this[int index] => (InfoSetFromIndex(index), SlugFromIndex(index));

        public int Add(InfoSet line, Slug slug)
	    {
		    line.PadIndex = PadIndex;
		    line.InfoSetIndex = _input.Count;
		    _input.Add(line);
		    Slugs.Add(slug);
		    _maps.Add(new SlugMap(_input.Count - 1, Slugs.Count - 1));
		    return Slugs.Count - 1;
	    }
	    public int Add(InfoSet line, int index)
	    {
		    line.PadIndex = PadIndex;
            line.InfoSetIndex = _input.Count;
            _input.Add(line);
            _maps.Add(new SlugMap(_input.Count - 1, index));
		    return index;
	    }
	    public int Add(InfoSet line)
	    {
		    line.PadIndex = PadIndex;
		    line.InfoSetIndex = _input.Count;
            _input.Add(line);
		    _maps.Add(new SlugMap(_input.Count - 1, 0));
		    return -1;
	    }
        public void Clear()
	    {
		    _input.Clear();
		    Slugs.Clear();
		    _maps.Clear();
            Slugs.Add(Slug.Zero); 
        }
	    public void Refresh()
	    {
            _output.Clear();
            foreach (var slugMap in _maps)
            {
	            if (slugMap.SlugIndex > -1)
	            {
		            var unit = SlugFromIndex(slugMap.SlugIndex);
		            var line = InfoSetFromIndex(slugMap.PolyIndex);
		            var norm = unit / 10.0;
		            var multStart = line.PointAlongLine(0, 1, norm.IsForward ? -(float)norm.Pull : (float)norm.Push);
		            var multEnd = line.PointAlongLine(0, 1, norm.IsForward ? (float)norm.Push : -(float)norm.Pull);
		            _output.Add(new SKSegment(multStart, multEnd));
	            }
            }
	    }

	    public void UpdatePoint(PointRef pointRef, SKPoint pt)
	    {
		    _input[pointRef.InfoSetIndex][pointRef.PointIndex] = pt;
	    }

	    public SKPoint GetHighlightPoint() => Highlight.IsEmpty ? SKPoint.Empty : InfoSetFromIndex(Highlight.InfoSetIndex)[Highlight.PointIndex];

	    public SKSegment GetHighlightLine()
	    {
		    SKSegment result = SKSegment.Empty;
		    var infoSet = InfoSetFromIndex(Highlight.InfoSetIndex);
		    if (!infoSet.IsEmpty)
		    {
                result = new SKSegment(infoSet[0], infoSet[1]);
            }
		    return result;
	    } 

        public PointRef[] GetSnapPoints(SKPoint input, float maxDist = SnapDistance)
	    {
		    var result = new List<PointRef>();
		    int lineIndex = 0;
		    foreach (var line in _input)
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
		    foreach (var infoSet in _input)
		    {
			    for (int i = 0; i < infoSet.Count - 1; i++)
			    {
				    var seg = new SegmentRef(infoSet.PointRefAt(i));
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

        public InfoSet InfoSetFromIndex(int index)
        {
            InfoSet result;
            if (index >= 0 && index < _input.Count)
            {
                result = _input[index];
            }
            else
            {
                result = InfoSet.Empty;
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

        public IEnumerator<(InfoSet, Slug)> GetEnumerator()
	    {
		    foreach (var map in _maps)
		    {
			    yield return (InfoSetFromIndex(map.PolyIndex), SlugFromIndex(map.SlugIndex));
		    }
	    }
	    IEnumerator IEnumerable.GetEnumerator()
	    {
		    foreach (var map in _maps)
		    {
			    yield return (InfoSetFromIndex(map.PolyIndex), SlugFromIndex(map.SlugIndex));
		    }
	    }
    }

}
