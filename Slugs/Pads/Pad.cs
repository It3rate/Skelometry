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
        private readonly Dictionary<int, IElement> _elements = new Dictionary<int, IElement>();
        private readonly HashSet<int> _entityKeys = new HashSet<int>();

        public static Slug ActiveSlug = Slug.Unit;
        private static readonly List<Slug> Slugs = new List<Slug>(); // -1 is 'none' position, 0 is activeSlug.
        public const float SnapDistance = 10.0f;

        public IEnumerable<Entity> Entities
        {
	        get 
	        {
		        foreach (var key in _entityKeys)
		        {
			        yield return EntityAt(key);
		        }
	        }
        }
        //public Entity EntityAt(int key) => Data.EntityAt(key);

        //public IEnumerable<Focal> Focals => Data.Focals;
        //public Focal FocalAt(int key) => Data.FocalAt(key);

        private readonly List<SKSegment> _output = new List<SKSegment>();
        public IEnumerable<SKSegment> Output => _output;

        public Pad(PadKind padKind, Agent agent)
        {
            PadKind = padKind;
            _agent = agent;
            //Data = new PadData(PadKind, this);
            Clear();
        }

        public void AddElement(IElement element)
        {
	        _elements.Add(element.Key, element);
	        switch (element.ElementKind)
	        {
		        case ElementKind.Entity:
			        _entityKeys.Add(element.Key);
			        break;
	        }
        }

        public void RemoveElement(int key)
        {
	        if (HasElementAt(key, out var element))
	        {
		        switch (element.ElementKind)
		        {
			        case ElementKind.Entity:
				        _entityKeys.Remove(element.Key);
				        break;
		        }
            }
	        _elements.Remove(key);
        }

        public void ClearElements()
        {
            _entityKeys.Clear();
	        _elements.Clear();
        }

        public bool HasElementAt(int key, out IElement element)
        {
	        _elements.TryGetValue(key, out element);
	        return element.IsEmpty;
        }
        public Entity EntityAt(int key)
        {
	        var success = _elements.TryGetValue(key, out var result);
	        return success && (result.ElementKind == ElementKind.Entity) ? (Entity)result : Entity.Empty;
        }
        public Trait TraitAt(int key)
        {
	        var success = _elements.TryGetValue(key, out var result);
	        return success && (result.ElementKind == ElementKind.Trait) ? (Trait)result : Trait.Empty;
        }
        public Focal FocalAt(int key)
        {
	        var success = _elements.TryGetValue(key, out var result);
	        return success && (result.ElementKind == ElementKind.Focal) ? (Focal)result : Focal.Empty;
        }
        public Bond BondAt(int key)
        {
	        var success = _elements.TryGetValue(key, out var result);
	        return success && (result.ElementKind == ElementKind.Bond) ? (Bond)result : Bond.Empty;
        }
        public IPoint PointAt(int key)
        {
	        var success = _elements.TryGetValue(key, out var result);
	        return success && result.ElementKind.IsPoint() ? (IPoint)result : Point.Empty;
        }

        public Entity CreateEntity(params Trait[] traits)
        {
	        var entity = new Entity(PadKind, traits);
	        AddElement(entity);
	        return entity;
        }
        public Entity GetOrCreateEntity(int entityKey)
        {
	        var entity = EntityAt(entityKey);
	        return entity.IsEmpty ? CreateEntity() : entity;
        }
        public Trait CreateTrait(int entityKey, SKSegment seg, int traitKindIndex)
        {
	        var entity = GetOrCreateEntity(entityKey);
	        var segRef = CreateTerminalSegRef(seg);
	        var trait = new Trait(segRef, entity, traitKindIndex);
	        entity.EmbedTrait(trait);
	        return trait;
        }
        public SegRef CreateTerminalSegRef(SKSegment skSegment)
        {
	        var a = CreateTerminalPoint(skSegment.StartPoint);
	        var b = CreateTerminalPoint(skSegment.EndPoint);
	        return new SegRef(a, b);
        }
        public Point CreateTerminalPoint(SKPoint pt)
        {
	        var point = new Point(PadKind, pt);
	        AddElement(point);
	        return point;
        }
        public Focal CreateFocal(float focus, Slug slug)
        {
            var focal = new Focal(PadKind, focus, slug);
            AddElement(focal);
	        return focal;
        }

        public (Entity, Trait) AddEntity(SKSegment seg, int traitKindIndex)
        {
	        var entity = CreateEntity();
	        var trait = AddTrait(entity.Key, seg, traitKindIndex);
	        return (entity, trait);
        }
        public Trait AddTrait(int entityKey, SKSegment seg, int traitKindIndex)
        {
	        var entity = GetOrCreateEntity(entityKey);
	        var segRef = _agent.CreateTerminalSegRef(PadKind, seg);
            var trait = new Trait(segRef, entity, traitKindIndex);
            entity.EmbedTrait(trait);
	        return trait;
        }
        public void Clear()
        {
            ClearElements();
        }
        public void Refresh()
        {
            _output.Clear();
            foreach (var entity in Entities)
            {
	            foreach (var bond in entity.Traits)
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
        }
        public void UpdatePointRef(IPoint point, IPoint value)
        {
	        _agent.SetPointAt(point.Key, value);
        }

        public List<IPoint> GetSnapPoints(SKPoint input, IPoint[] ignorePoints, float maxDist = SnapDistance)
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
            foreach (var entity in Entities)
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
