using System.Runtime.CompilerServices;
using SkiaSharp;
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

        public IEnumerable<IPoint> PtRefs => Data.PtRefs;
        public IPoint PtRefAt(int key) => Data.PtRefAt(key);
        public int KeyForPtRef(IPoint pt) => Data.KeyForPtRef(pt);
        public void SetPtRef(int key, IPoint value) => Data.SetPtRef(key, value);

        public IEnumerable<Entity> Entities => Data.Entities;
        public Entity EntityAt(int key) => Data.EntityAt(key);

        public IEnumerable<Slug> Focals => Data.Focals;
        public Slug FocalAt(int key) => Data.FocalAt(key);

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

        public (int, Entity, SegRef) AddEntity(SKSegment seg, int traitKindIndex)
        {
	        var (key, entity) = Data.CreateEmptyEntity();
	        var segRef = AddTrait(key, seg, traitKindIndex);
	        return (key, entity, segRef);
        }
        public SegRef AddTrait(int key, SKSegment seg, int traitKindIndex)
        {
	        var entity = Data.GetOrCreateEntity(key);
	        var segRef = Data.CreateTerminalSegRef(seg);
	        //segRef.Start.EntityKey = key;
	        //segRef.End.EntityKey = key;
            var trait = new Trait(segRef, traitKindIndex);
            entity.EmbedTrait(trait);
	        return segRef;
        }
        //public int AddEntity(DataMap data, int index)
        //{
        //    data.PadIndex = PadIndex;
        //    data.DataMapIndex = _dataMaps.Count;
        //    _dataMaps.AddEntity(data);
        //    _slugMaps.AddEntity(new SlugRef(PadIndex, _dataMaps.Count - 1, index));
        //    return index;
        //}
        //public int AddEntity(DataMap data)
        //{
        //    data.PadIndex = PadIndex;
        //    data.DataMapIndex = _dataMaps.Count;
        //    _dataMaps.AddEntity(data);
        //    _slugMaps.AddEntity(new SlugRef(PadIndex, _dataMaps.Count - 1, 0));
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
                    //_output.AddEntity(new SKSegment(multStart, multEnd));
                }
            }
        }

        public void UpdatePoint(IPoint point, SKPoint pt)
        {
	        point.SKPoint = pt;
            //_dataMaps[point.EntityKey][point] = pt;
        }
        public void UpdatePointRef(IPoint point, IPoint value)
        {
	        Data.ReplacePointRef(point, value);
        }

        public List<IPoint> GetSnapPoints(SKPoint input, DragRef ignorePoints, float maxDist = SnapDistance)
        {
            var result = new List<IPoint>();
            foreach (var ptRef in Data.PtRefs)
            {
	            if (!ignorePoints.Contains(ptRef) && input.SquaredDistanceTo(ptRef.SKPoint) < maxDist)
	            {
                    result.Add(ptRef);
	            }
            }
            return result;
        }

        public Trait GetSnapLine(SKPoint point, float maxDist = SnapDistance)
        {
            var result = Trait.Empty;
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
