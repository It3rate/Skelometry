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
		        var values = _elements.Where(kvp => kvp.Value.ElementKind.IsPoint());
		        foreach (var kvp in values)
		        {
			        yield return (IPoint)kvp.Value;
		        }
	        }
        }
        public IEnumerable<IPoint> PointsReversed
        {
	        get
	        {
		        var values = _elements.Where(kvp => kvp.Value.ElementKind.IsPoint()).Reverse();
		        foreach (var kvp in values)
		        {
			        yield return (IPoint)kvp.Value;
		        }
	        }
        }
        public IEnumerable<IElement> ElementsOfKind(ElementKind kind)
        {
	        var values = _elements.Where(kvp => kvp.Value.ElementKind.IsCompatible(kind));
	        foreach (var kvp in values)
	        {
		        yield return kvp.Value;
	        }
        }
        public IEnumerable<IElement> ElementsOfKindReversed(ElementKind kind)
        {
	        var values = _elements.Where(kvp => kvp.Value.ElementKind.IsCompatible(kind)).Reverse();
	        foreach (var kvp in values)
	        {
		        yield return kvp.Value;
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
	        if (element.Key == ElementBase.EmptyKeyValue)
	        {
		        throw new ArgumentException("Can not add empty key to Pad");
	        }
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
        public SingleBond BondAt(int key)
        {
	        var success = _elements.TryGetValue(key, out var result);
	        return success && (result.ElementKind == ElementKind.SingleBond) ? (SingleBond)result : SingleBond.Empty;
        }
        public DoubleBond DoubleBondAt(int key)
        {
	        var success = _elements.TryGetValue(key, out var result);
	        return success && (result.ElementKind == ElementKind.DoubleBond) ? (DoubleBond)result : DoubleBond.Empty;
        }
        public IPoint PointAt(int key)
        {
	        var success = _elements.TryGetValue(key, out var result);
	        return success && result.ElementKind.IsPoint() ? (IPoint)result : TerminalPoint.Empty;
        }
        public void SetPointAt(int key, IPoint point)
        {
	        _elements[key] = point;
        }

        public IPoint ResolvedPointFor(IPoint point)
        {
	        IElement result = point;
	        var success = true;
	        while (success && !result.IsEmpty && result is RefPoint refPoint)
	        {
		        success = _elements.TryGetValue(refPoint.TargetKey, out result);
	        }
	        return (IPoint)result;// result.ElementKind.IsPoint() ? (TerminalPoint)result : TerminalPoint.Empty;
        }

        public void MergePoints(IPoint from, IPoint to) => MergePoints(from, to, SKPoint.Empty);
        public void MergePoints(IPoint from, IPoint to, SKPoint position)
        {
	        var terminal = ResolvedPointFor(to);
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
	        var terminal = ResolvedPointFor(PointAt(toKey));
	        //SetPointAt(fromKey, terminal);
            // need to let the point merge, as some points are virtual (eg a singleBond that maps to a focal endpoint).
	        var from = PointAt(fromKey);
            from.MergeInto(terminal);
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

        public (Entity, Trait) AddEntity(SKPoint startPoint, SKPoint endPoint, TraitKind traitKind)
        {
	        var entity = CreateEntity();
	        var trait = AddTrait(entity.Key, startPoint, endPoint, traitKind);
	        return (entity, trait);
        }
        // todo: entities should automatically have access to all traits in their pad. Focals tie together traits and entities.
        public Trait AddTrait(int entityKey, SKPoint startPoint, SKPoint endPoint, TraitKind traitKind)
        {
	        var entity = GetOrCreateEntity(entityKey);
	        var start = CreateTerminalPoint(startPoint);
	        var end = CreateTerminalPoint(endPoint);
	        var trait = new Trait(entity, start, end, traitKind);
	        entity.EmbedTrait(trait);
	        return trait;
        }
        public Trait AddTrait(int entityKey, int startPointKey, int endPointKey, TraitKind traitKind)
        {
	        var entity = GetOrCreateEntity(entityKey);
	        var trait = new Trait(entity, startPointKey, endPointKey, traitKind);
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
        }

        public IPoint GetSnapPoint(SKPoint input, List<int> ignorePoints, ElementKind kind, float maxDist = SnapDistance * 2f)
        {
	        IPoint result = TerminalPoint.Empty;
	        foreach (var ptRef in PointsReversed)
	        {
	            if (ptRef.ElementKind.IsCompatible(kind) && !ignorePoints.Contains(ptRef.Key) && input.DistanceTo(ptRef.Position) < maxDist)
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

            foreach (var element in ElementsOfKindReversed(kind))
            {
	            if (!element.HasArea && !ignoreKeys.Contains(element.Key) && element.DistanceToPoint(point) < maxDist)
	            {
		            result = element;
		            break;
	            }
            }
            // todo: check for area hits with double bonds if line/points not found.
            if (result.IsEmpty)
            {
	            foreach (var entity in Entities)
	            {
		            foreach (var db in entity.DoubleBonds)
		            {
			            if (db.Path.Contains(point.X, point.Y))
			            {
				            result = db;
				            goto End;
			            }
		            }
	            }
            }
            End:
            return result;
        }

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
    }
}
