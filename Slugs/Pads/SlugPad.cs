﻿using System.Collections;
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

    public class SlugPad : IEnumerable<(DataMap, Slug)>
    {
	    private static int _padIndexCounter = 0;
	    public readonly int PadIndex;

	    public static Slug ActiveSlug = Slug.Unit;
        private static readonly List<Slug> Slugs = new List<Slug>(); // -1 is 'none' position, 0 is activeSlug.
        public const float SnapDistance = 10.0f;

	    public PadKind PadKind;
	    public PointRef HighlightPoint = PointRef.Empty;
	    public PointRef HighlightLine = PointRef.Empty;

        private readonly List<DataMap> _dataMaps = new List<DataMap>();
        private readonly List<SlugMap> _slugMaps = new List<SlugMap>();
        //private readonly List<DataPoints> _dataMaps = new List<DataPoints>(); // todo: move points and map into single file that updates either.
        //private readonly List<PointRef[]> _dataMapsMap = new List<PointRef[]>();
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

        private readonly DataMap _defaults;

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
		    _slugMaps.Add(new SlugMap(_dataMaps.Count - 1, Slugs.Count - 1));
		    return Slugs.Count - 1;
	    }
	    public int Add(DataMap data, int index)
	    {
		    data.PadIndex = PadIndex;
            data.DataMapIndex = _dataMaps.Count;
            _dataMaps.Add(data);
            _slugMaps.Add(new SlugMap(_dataMaps.Count - 1, index));
		    return index;
	    }
	    public int Add(DataMap data)
	    {
		    data.PadIndex = PadIndex;
		    data.DataMapIndex = _dataMaps.Count;
            _dataMaps.Add(data);
            _slugMaps.Add(new SlugMap(_dataMaps.Count - 1, 0));
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

	    public void UpdatePoint(PointRef pointRef, SKPoint pt)
	    {
		    _dataMaps[pointRef.DataMapIndex][pointRef] = pt;
	    }
	    public void UpdatePointRef(PointRef pointRef, PointRef value)
	    {
		    _dataMaps[pointRef.DataMapIndex][pointRef.PointIndex] = value;
	    }

        public SKPoint GetHighlightPoint() => HighlightPoint.IsEmpty ? SKPoint.Empty : InputFromIndex(HighlightPoint.DataMapIndex)[HighlightPoint];

	    public SKSegment GetHighlightLine()
	    {
		    SKSegment result = SKSegment.Empty;
		    var dataMap = InputFromIndex(HighlightPoint.DataMapIndex);
		    if (!dataMap.IsEmpty)
		    {
                result = new SKSegment(dataMap.PointAt(0), dataMap.PointAt(1));
            }
		    return result;
	    } 

        public PointRef[] GetSnapPoints(SKPoint input, float maxDist = SnapDistance)
	    {
		    var result = new List<PointRef>();
		    int dataIndex = 0;
		    foreach (var dataMap in _dataMaps)
		    {
			    var ptIndex = dataMap.GetSnapPoint(input, maxDist);
			    if (ptIndex > -1)
			    {
				    result.Add(new PointRef(PadIndex, dataIndex, ptIndex));
			    }
			    dataIndex++;

		    }
		    return result.ToArray();
	    }

	    public PointRef GetSnapLine(SKPoint point, float maxDist = SnapDistance)
	    {
		    var result = PointRef.Empty;
		    int lineIndex = 0;
		    foreach (var dataMap in _dataMaps)
		    {
			    for (int i = 0; i < dataMap.Count - 1; i++)
			    {
				    var seg = new SegmentRef(dataMap[i]);
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
