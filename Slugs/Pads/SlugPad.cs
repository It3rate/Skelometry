using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenTK.Graphics.ES30;
using SkiaSharp;
using Slugs.Extensions;
using Slugs.Input;
using Slugs.Motors;
using Slugs.Slugs;
using SegRef = Slugs.Input.SegRef;

namespace Slugs.Pads
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum PadKind{Working, Drawn}

    public class SlugPad : IEnumerable<(DataMap, Slug)>
    {
	    private static int _padIndexCounter = 0;
	    public readonly int PadIndex;

        public readonly PadData Data = new PadData();

	    public static Slug ActiveSlug = Slug.Unit;
        private static readonly List<Slug> Slugs = new List<Slug>(); // -1 is 'none' position, 0 is activeSlug.
        public const float SnapDistance = 10.0f;

	    public PadKind PadKind;
	    public List<IPointRef> HighlightPoints = new List<IPointRef>();
	    public bool HasHighlightPoint => HighlightPoints.Count > 0;
	    public IPointRef FirstHighlightPoint => HasHighlightPoint ? HighlightPoints[0] : PointRef.Empty;
        public SegRef HighlightLine = SegRef.Empty;
        public bool HasHighlightLine => HighlightLine != SegRef.Empty;

        private readonly List<DataMap> _dataMaps = new List<DataMap>();
        private readonly List<SlugRef> _slugMaps = new List<SlugRef>();
        private readonly List<SKSegment> _output = new List<SKSegment>();

        public IEnumerable<DataMap> Input
        {
	        get
	        {
		        foreach (var pointRef in _dataMaps)
		        {
			        yield return pointRef;
		        }
	        }
        }

        public IEnumerable<SKSegment> Output => _output;

        public SlugPad(PadKind padKind)
	    {
		    PadIndex = _padIndexCounter++;
            PadKind = padKind;
            Clear();
	    }

	    public (DataMap, Slug) this[int index] => (InputFromIndex(index), SlugFromIndex(index));

        public int Add(DataMap data, Slug slug)
	    {
		    data.PadIndex = PadIndex;
		    data.DataMapIndex = _dataMaps.Count;
		    _dataMaps.Add(data);
		    Slugs.Add(slug);
		    _slugMaps.Add(new SlugRef(PadIndex, _dataMaps.Count - 1, Slugs.Count - 1));
		    return Slugs.Count - 1;
	    }
	    public int Add(DataMap data, int index)
	    {
		    data.PadIndex = PadIndex;
            data.DataMapIndex = _dataMaps.Count;
            _dataMaps.Add(data);
            _slugMaps.Add(new SlugRef(PadIndex, _dataMaps.Count - 1, index));
		    return index;
	    }
	    public int Add(DataMap data)
	    {
		    data.PadIndex = PadIndex;
		    data.DataMapIndex = _dataMaps.Count;
            _dataMaps.Add(data);
            _slugMaps.Add(new SlugRef(PadIndex, _dataMaps.Count - 1, 0));
		    return -1;
	    }
        public void Clear()
        {
	        _dataMaps.Clear();
            Slugs.Clear();
		    _slugMaps.Clear();

            Slugs.Add(Slug.Zero); 
        }
	    public void Refresh()
	    {
            _output.Clear();
            foreach (var slugMap in _slugMaps)
            {
	            if (slugMap.SlugIndex > -1)
	            {
		            var unit = SlugFromIndex(slugMap.SlugIndex);
		            var line = InputFromIndex(slugMap.DataMapIndex);
		            var norm = unit / 10.0;
		            var multStart = line.PointAlongLine(0, 1, norm.IsForward ? -(float)norm.Pull : (float)norm.Push);
		            var multEnd = line.PointAlongLine(0, 1, norm.IsForward ? (float)norm.Push : -(float)norm.Pull);
		            _output.Add(new SKSegment(multStart, multEnd));
	            }
            }
	    }

	    public void UpdatePoint(IPointRef pointRef, SKPoint pt)
	    {
		    _dataMaps[pointRef.EntityIndex][pointRef] = pt;
	    }
        public void UpdatePointRef(IPointRef pointRef, IPointRef value)
	    {
		    _dataMaps[pointRef.EntityIndex][pointRef.FocalIndex] = value;
	    }

	    public SKPoint GetHighlightPoint() => HighlightPoints.Count > 0 ? HighlightPoints[0].SKPoint : SKPoint.Empty;
	    public SKSegment GetHighlightLine() => HighlightLine.SKSegment;

        public List<IPointRef> GetSnapPoints(SKPoint input, DragRef ignorePoints, float maxDist = SnapDistance)
	    {
		    var result = new List<IPointRef>();
		    int dataIndex = 0;
		    foreach (var dataMap in _dataMaps)
		    {
			    var ptIndex = dataMap.GetSnapPoint(input, maxDist);
			    if (ptIndex > -1 && !ignorePoints.Contains(dataMap[ptIndex]) && !result.Contains(dataMap[ptIndex]))
			    {
				    result.Add(dataMap[ptIndex]);//new IPointRef(PadIndex, dataIndex, ptIndex));
			    }
			    dataIndex++;
		    }
		    return result;
	    }

	    public SegRef GetSnapLine(SKPoint point, float maxDist = SnapDistance)
	    {
		    var result = SegRef.Empty;
		    int lineIndex = 0;
		    foreach (var dataMap in _dataMaps)
		    {
			    for (int i = 0; i < dataMap.Count - 1; i++)
			    {
				    var seg = new SegRef(dataMap[i], dataMap[i+1]);
				    var closest = seg.ProjectPointOnto(point);
				    var dist = point.SquaredDistanceTo(closest);
				    if (dist < maxDist)
				    {
                        result = seg;
                        goto End;
				    }
					lineIndex++;
			    }
		    }
            End:
		    return result;
        }

        public DataMap InputFromIndex(int index)
        {
	        DataMap result;
            if (index >= 0 && index < _dataMaps.Count)
            {
                result = _dataMaps[index];
            }
            else
            {
	            result = DataMap.Empty;
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

        public IEnumerator<(DataMap, Slug)> GetEnumerator()
	    {
		    foreach (var map in _slugMaps)
		    {
			    yield return (InputFromIndex(map.DataMapIndex), SlugFromIndex(map.SlugIndex));
		    }
	    }
	    IEnumerator IEnumerable.GetEnumerator()
	    {
		    foreach (var map in _slugMaps)
		    {
			    yield return (InputFromIndex(map.DataMapIndex), SlugFromIndex(map.SlugIndex));
		    }
	    }
    }

}
