using System.Runtime.CompilerServices;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Input;
using Slugs.Pads;
using Slugs.Primitives;
using SegRef = Slugs.Primitives.SegRef;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Pad
    {
        public readonly PadKind PadKind;

        private Agent _agent;
        public readonly PadData Data;

        public static Slug ActiveSlug = Slug.Unit;
        private static readonly List<Slug> Slugs = new List<Slug>(); // -1 is 'none' position, 0 is activeSlug.
        public const float SnapDistance = 10.0f;

        public IEnumerable<Entity> Entities => Data.Entities;
        public Entity EntityAt(int key) => Data.EntityAt(key);

        public IEnumerable<Focal> Focals => Data.Focals;
        public Focal FocalAt(int key) => Data.FocalAt(key);

        private readonly List<SKSegment> _output = new List<SKSegment>();
        public IEnumerable<SKSegment> Output => _output;

        public Pad(PadKind padKind, Agent agent)
        {
            PadKind = padKind;
            _agent = agent;
            Data = new PadData(PadKind, this);
            Clear();
        }

        public (int, Entity, Trait) AddEntity(SKSegment seg, int traitKindIndex)
        {
	        var (key, entity) = Data.CreateEmptyEntity();
	        var trait = AddTrait(key, seg, traitKindIndex);
	        return (key, entity, trait);
        }
        public Trait AddTrait(int entityKey, SKSegment seg, int traitKindIndex)
        {
	        var entity = Data.GetOrCreateEntity(entityKey);
	        var segRef = _agent.CreateTerminalSegRef(PadKind, seg);
            var trait = new Trait(segRef, entity, traitKindIndex);
            entity.EmbedTrait(trait);
	        return trait;
        }
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
	        _agent.SetPointAt(point.Key, value);
        }

        public List<IPoint> GetSnapPoints(SKPoint input, DragRef ignorePoints, float maxDist = SnapDistance)
        {
            var result = new List<IPoint>();
            foreach (var ptRef in _agent.Points) // use entities and traits rather than points?
            {
	            if (ptRef.PadKind == PadKind && !ignorePoints.Contains(ptRef) && input.SquaredDistanceTo(ptRef.SKPoint) < maxDist)
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

        //public IEnumerator<(DataMap, Range)> GetEnumerator()
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
