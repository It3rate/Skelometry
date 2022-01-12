﻿using SkiaSharp;
using Slugs.Agent;
using Slugs.Extensions;
using Slugs.Input;
using Slugs.Pads;
using Slugs.Slugs;
using SegRef = Slugs.Entities.SegRef;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EntityPad
    {
        private static int _padIndexCounter = 0;
        public readonly int PadIndex;

        public PadKind PadKind;

        private IAgent _agent;
        public readonly PadData Data;

        public static Slug ActiveSlug = Slug.Unit;
        private static readonly List<Slug> Slugs = new List<Slug>(); // -1 is 'none' position, 0 is activeSlug.
        public const float SnapDistance = 10.0f;

        public IEnumerable<IPointRef> PtRefs => Data.PtRefs;
        public IEnumerable<Entity> Entities => Data.Entities;
        public IEnumerable<Slug> Focals => Data.Focals;

        private readonly List<SKSegment> _output = new List<SKSegment>();
        public IEnumerable<SKSegment> Output => _output;

        public EntityPad(PadKind padKind, IAgent agent)
        {
            PadKind = padKind;
            _agent = agent;
            PadIndex = _padIndexCounter++;
            Data = new PadData(PadIndex, this);
            Clear();
        }

        public int Add(SKPoint start, SKPoint end, Slug slug)
        {
	        var entityKey = Data.CreateEntity(new SKSegment(start, end));
            return entityKey;
        }
        //public int Add(DataMap data, int index)
        //{
        //    data.PadIndex = PadIndex;
        //    data.DataMapIndex = _dataMaps.Count;
        //    _dataMaps.Add(data);
        //    _slugMaps.Add(new SlugRef(PadIndex, _dataMaps.Count - 1, index));
        //    return index;
        //}
        //public int Add(DataMap data)
        //{
        //    data.PadIndex = PadIndex;
        //    data.DataMapIndex = _dataMaps.Count;
        //    _dataMaps.Add(data);
        //    _slugMaps.Add(new SlugRef(PadIndex, _dataMaps.Count - 1, 0));
        //    return -1;
        //}
        public void Clear()
        {
            Data.Clear();
        }
        public void Refresh()
        {
            _output.Clear();
            foreach (var entity in Entities)
            {
	            foreach (var bond in entity.Bonds)
	            {
                    //var unit = SlugFromIndex(entity.SlugIndex);
                    //var line = InputFromIndex(entity.DataMapIndex);
                    //var norm = unit / 10.0;
                    //var multStart = line.PointAlongLine(0, 1, norm.IsForward ? -(float)norm.Pull : (float)norm.Push);
                    //var multEnd = line.PointAlongLine(0, 1, norm.IsForward ? (float)norm.Push : -(float)norm.Pull);
                    //_output.Add(new SKSegment(multStart, multEnd));
                }
            }
        }

        public void UpdatePoint(IPointRef pointRef, SKPoint pt)
        {
	        pointRef.SKPoint = pt;
            //_dataMaps[pointRef.EntityKey][pointRef] = pt;
        }
        public void UpdatePointRef(IPointRef pointRef, IPointRef value)
        {
	        Data.ReplacePointRef(pointRef, value);
        }

        public List<IPointRef> GetSnapPoints(SKPoint input, DragRef ignorePoints, float maxDist = SnapDistance)
        {
            var result = new List<IPointRef>();
            foreach (var ptRef in Data.PtRefs)
            {
	            if (!ignorePoints.Contains(ptRef) && input.SquaredDistanceTo(ptRef.SKPoint) < maxDist)
	            {
                    result.Add(ptRef);
	            }
            }
            return result;
        }

        public SegRef GetSnapLine(SKPoint point, float maxDist = SnapDistance)
        {
            var result = SegRef.Empty;
            int lineIndex = 0;
            foreach (var entity in Data.Entities)
            {
	            foreach (var trait in entity.Traits)
	            {
                    var closest = trait.ProjectPointOnto(point);
                    var dist = point.SquaredDistanceTo(closest);
                    if (dist < maxDist)
                    {
                        result = trait;
                        goto End;
                    }
                    lineIndex++;
                }
            }
			End:
            return result;
        }

        //public DataMap InputFromIndex(int index)
        //{
        //    DataMap result;
        //    if (index >= 0 && index < _dataMaps.Count)
        //    {
        //        result = _dataMaps[index];
        //    }
        //    else
        //    {
        //        result = DataMap.Empty;
        //    }
        //    return result;
        //}

        public Slug SlugFromIndex(int index)
        {
            Slug result;
            if (index == 0)
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

        //public IEnumerator<(DataMap, Slug)> GetEnumerator()
        //{
        //    foreach (var map in _slugMaps)
        //    {
        //        yield return (InputFromIndex(map.DataMapIndex), SlugFromIndex(map.SlugIndex));
        //    }
        //}
        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    foreach (var map in _slugMaps)
        //    {
        //        yield return (InputFromIndex(map.DataMapIndex), SlugFromIndex(map.SlugIndex));
        //    }
        //}
    }
}
