using System.Drawing;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Pad
    {
        public readonly PadKind PadKind;
        public static int KeyCounter = 1;

        private Agent _agent;
        private readonly Dictionary<int, IElement> _elements = new Dictionary<int, IElement>();

        public static Slug ActiveSlug = Slug.Unit;
        private static readonly List<Slug> Slugs = new List<Slug>(); // -1 is 'none' position, 0 is activeSlug.
        public const float SnapDistance = 10.0f;

        public IEnumerable<Entity> Entities
        {
	        get
	        {
		        var values = _elements.Where(kvp => kvp.Value.ElementKind == ElementKind.Entity);
		        foreach (var kvp in values)
		        {
			        yield return (Entity)kvp.Value;
		        }
	        }
        }
        public IEnumerable<IPoint> Points
        {
	        get
	        {
		        var values = _elements.Where(kvp => kvp.Value.ElementKind.IsPoint() );
                foreach (var kvp in values)
		        {
			        yield return (IPoint)kvp.Value;
		        }
	        }
        }
        //public Entity EntityAt(int key) => Data.EntityAt(key);

        //public IEnumerable<AddedFocal> Focals => Data.Focals;
        //public AddedFocal FocalAt(int key) => Data.FocalAt(key);

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
	        if (_elements.ContainsKey(element.Key))
	        {
		        _elements.Remove(element.Key);
	        }
	        _elements.Add(element.Key, element);
        }

        public void RemoveElement(int key)
        {
	        _elements.Remove(key);
        }

        public void ClearElements()
        {
	        _elements.Clear();
        }

        public IElement ElementAt(int key)
        {
	        IElement result;
	        if (key == ElementBase.EmptyKeyValue)
	        {
		        result = TerminalPoint.Empty;
		        //throw new ArgumentOutOfRangeException("Empty key lookup.");
	        }
	        else
	        {
		        var success = _elements.TryGetValue(key, out IElement element);
		        result = success ? element : TerminalPoint.Empty;
	        }
	        return result;
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
	        return success && result.ElementKind.IsPoint() ? (IPoint)result : RefPoint.Empty;
        }
        public void SetPointAt(int key, IPoint point)
        {
	        _elements[key] = point;
        }

        public TerminalPoint TerminalPointFor(IPoint point)
        {
	        IElement result = point;
	        var success = true;
	        while (success && !result.IsEmpty && result is RefPoint refPoint)
	        {
		        success = _elements.TryGetValue(refPoint.TargetKey, out result);
	        }
	        return result.ElementKind.IsTerminal() ? (TerminalPoint)result : TerminalPoint.Empty;
        }

        public void MergePoints(IPoint from, IPoint to) => MergePoints(from, to, SKPoint.Empty);
        public void MergePoints(IPoint from, IPoint to, SKPoint position)
        {
	        var terminal = TerminalPointFor(to);
	        if (!terminal.IsEmpty)
	        {
		        if (!position.IsEmpty)
		        {
			        terminal.Position = position;
		        }
	            var point = new RefPoint(PadKind, terminal.Key);
	            SetPointAt(from.Key, point);
	        }
        }
        public void MergePoints(int fromKey, int toKey)
        {
	        var terminal = TerminalPointFor(PointAt(toKey));
	        SetPointAt(fromKey, terminal);
        }

        public Entity CreateEntity(params Trait[] traits)
        {
	        var entity = new Entity(PadKind, traits);
	        return entity;
        }
        public Entity GetOrCreateEntity(int entityKey)
        {
	        var entity = EntityAt(entityKey);
	        return entity.IsEmpty ? CreateEntity() : entity;
        }
        public TerminalPoint CreateTerminalPoint(SKPoint pt)
        {
	        var point = new TerminalPoint(PadKind, pt);
	        return point;
        }
        public RefPoint CreateRefPoint(int targetKey)
        {
	        var point = new RefPoint(PadKind, targetKey);
	        return point;
        }
        public Focal CreateFocal(float focus, Slug slug)
        {
	        throw new NotImplementedException();
	        //   var focal = new AddedFocal(PadKind, focus, slug);
	        //return focal;
        }

        public (Entity, Trait) AddEntity(SKPoint startPoint, SKPoint endPoint, int traitKindIndex)
        {
	        var entity = CreateEntity();
	        var trait = AddTrait(entity.Key, startPoint, endPoint, traitKindIndex);
	        return (entity, trait);
        }
        public Trait AddTrait(int entityKey, SKPoint startPoint, SKPoint endPoint, int traitKindIndex)
        {
	        var entity = GetOrCreateEntity(entityKey);
	        var start = CreateTerminalPoint(startPoint);
	        var end = CreateTerminalPoint(endPoint);
	        var trait = new Trait(entity, start, end, traitKindIndex);
	        entity.EmbedTrait(trait);
	        return trait;
        }
        public Trait AddTrait(int entityKey, int startPointKey, int endPointKey, int traitKindIndex)
        {
	        var entity = GetOrCreateEntity(entityKey);
	        var trait = new Trait(entity, startPointKey, endPointKey, traitKindIndex);
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
                    //var multStart = line.PointAlongLine(0, 1, norm.IsForward ? -(float)norm.Aft : (float)norm.Fore);
                    //var multEnd = line.PointAlongLine(0, 1, norm.IsForward ? (float)norm.Fore : -(float)norm.Aft);
                    //_output.AddEntity(new SKSegment(multStart, multEnd));
                }
            }
        }

        public IPoint GetSnapPoint(SKPoint input, List<int> ignorePoints, ElementKind kind, float maxDist = SnapDistance)
        {
	        IPoint result = TerminalPoint.Empty;
            foreach (var ptRef in Points)
            {
	            if (ptRef.ElementKind.IsCompatible(kind) && !ignorePoints.Contains(ptRef.Key) && input.SquaredDistanceTo(ptRef.Position) < maxDist)
	            {
                    result = ptRef;
                    break;
	            }
            }
            return result;
        }

        public IElement GetSnapElement(SKPoint point, List<int> ignoreKeys, ElementKind kind, float maxDist = SnapDistance)
        {
            IElement result = TerminalPoint.Empty;
            int lineIndex = 0;
            foreach (var entity in Entities)
            {
	            if (!ignoreKeys.Contains(entity.Key))
	            {
		            if (kind.IsCompatible(ElementKind.Trait)) // todo: need to cycle through all elements to avoid filtering sub elements.
		            {
			            foreach (var trait in entity.Traits)
			            {
				            if (!ignoreKeys.Contains(trait.Key))
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
		            }
		            if (kind.IsCompatible(ElementKind.Focal)) // todo: need to cycle through all elements to avoid filtering sub elements.
		            {
			            foreach (var focal in entity.Focals)
			            {
				            if (!ignoreKeys.Contains(focal.Key))
				            {
					            var closest = focal.ProjectPointOnto(point);
					            var dist = point.SquaredDistanceTo(closest);
					            if (dist < maxDist)
					            {
						            result = focal;
						            goto End;
					            }

					            lineIndex++;
				            }
			            }
                    }
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
