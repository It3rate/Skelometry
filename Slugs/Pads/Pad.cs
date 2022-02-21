using System.Drawing;
using SkiaSharp;
using Slugs.Constraints;
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
        public const float SnapDistance = 5.0f;

        private readonly Dictionary<int, IElement> _elements = new Dictionary<int, IElement>();
        public List<IConstraint> Constraints { get; } = new List<IConstraint>();

        public Dictionary<TraitKind, int> UnitMap = new Dictionary<TraitKind, int>();
        public void SetUnit(Focal focal) => UnitMap[focal.TraitKind] = focal.Key;
        public int UnitKeyFor(TraitKind traitKind) => UnitMap.ContainsKey(traitKind) ? UnitMap[traitKind] : ElementBase.EmptyKeyValue;
        public Slug UnitFor(TraitKind traitKind) => UnitMap.ContainsKey(traitKind) ? FocalAt(UnitMap[traitKind]).AsUnitSlug : Slug.Unit;
        public Focal FocalUnitFor(Focal focal)
        {
	        var traitKind = focal.TraitKind;
	        return UnitMap.ContainsKey(traitKind) ? FocalAt(UnitMap[traitKind]) : focal;
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

        public Pad(PadKind padKind)
        {
            PadKind = padKind;
            //Data = new PadData(PadKind, this);
            Clear();
        }

        #region Elements
	    public IElement ElementAt(int key)
	    {
		    IElement result;
		    if (key == ElementBase.EmptyKeyValue)
		    {
			    result = TerminalPoint.Empty;
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

	        if (element is Trait)
	        {
		        _traitKeys.Add(element.Key);
	        }
            else if (element is Entity)
	        {
		        _entityKeys.Add(element.Key);
	        }
        }
        public void RemoveElement(int key)
        {
	        var element = ElementAt(key);
	        _elements.Remove(key);

	        if (element is Trait)
	        {
		        _traitKeys.Remove(element.Key);
	        }
	        else if (element is Entity)
	        {
		        _entityKeys.Remove(element.Key);
	        }
        }
        public void ClearElements()
        {
	        _elements.Clear();
            _entityKeys.Clear();
            _traitKeys.Clear();
        }
#endregion
        #region Entities
	    private readonly HashSet<int> _entityKeys = new HashSet<int>();
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
        public Entity EntityAt(int key)
        {
	        var element = ElementAt(key);
	        return element is Entity entity ? entity : Entity.Empty;
        }
        //public (Entity, Trait) CreateEntity(SKPoint startPoint, SKPoint endPoint, TraitKind traitKind)
        //{
        // var entity = CreateEntity();
        // var trait = CreateTrait(entity.Key, startPoint, endPoint, traitKind);
        // return (entity, trait);
        //}
        //public Entity CreateEntity()
        //{
        // return new Entity(PadKind);
        //}
        public Entity GetOrCreateEntity(int entityKey)
        {
            var entity = EntityAt(entityKey);
            return entity.IsEmpty ? new Entity(PadKind) : entity;
        }
        #endregion
        #region Traits
        private readonly HashSet<int> _traitKeys = new HashSet<int>();
	    public IEnumerable<Trait> Traits
	    {
		    get
		    {
			    foreach (var key in _traitKeys)
			    {
				    yield return TraitAt(key);
			    }
		    }
	    }
        public Trait TraitAt(int key)
        {
	        var element = ElementAt(key);
	        return element is Trait trait ? trait : Trait.Empty;
        }
        public Trait TraitWithPoint(IPoint point)
        {
	        var result = Trait.Empty;
	        if (!point.IsEmpty && point is TerminalPoint tp) // terminal points (original trait) only for now
	        {
		        foreach (var trait in Traits)
		        {
			        if (trait.StartKey == tp.Key || trait.EndKey == tp.Key)
			        {
				        result = trait;
				        break;
			        }
		        }
	        }
	        return result;
        }

        #endregion
        #region Points
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

        #endregion

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

	    public IEnumerable<DoubleBond> DoubleBonds()
	    {
		    var values = _elements.Where(kvp => kvp.Value.ElementKind.IsCompatible(ElementKind.DoubleBond));
		    foreach (var kvp in values)
		    {
			    yield return (DoubleBond)kvp.Value;
		    }
	    }
	    public List<DoubleBond> ConnectedDoubleBonds(DoubleBond sourceBond, HashSet<int> ignoreKeys)
	    {
		    var result = new List<DoubleBond>();
		    foreach (var db in DoubleBonds())
		    {
			    if (!ignoreKeys.Contains(db.Key) && (db.HasFocal(sourceBond.StartKey) || db.HasFocal(sourceBond.EndKey)))
                {
                    result.Add(db);
			    }
		    }
		    return result;
	    }
	    public List<(DoubleBond, Focal)> DoubleBondsWithPoint(FocalPoint point)
	    {
		    var result = new List<(DoubleBond, Focal)>();
		    foreach (var db in DoubleBonds())
		    {
			    var focal = db.ContainsFocalPoint(point);
			    if (!focal.IsEmpty)
			    {
				    result.Add((db, focal)); // should save focals as well.
			    }
		    }
		    return result;
	    }
        public void Clear()
        {
            ClearElements();
        }
        public void Refresh()
        {
        }

        public IPoint GetSnapPoint(SKPoint input, List<int> ignorePoints, bool ignoreLocked, ElementKind kind, float maxDist = SnapDistance * 2f)
        {
	        IPoint result = TerminalPoint.Empty;
	        foreach (var ptRef in PointsReversed)
	        {
		        var ignore = ignoreLocked && ptRef.IsLocked;
                if (ptRef.ElementKind.IsCompatible(kind) && !ignorePoints.Contains(ptRef.Key) && !ignore && input.DistanceTo(ptRef.Position) < maxDist)
	            {
                    result = ptRef;
                    break;
	            }
            }
            return result;
        }
        public IElement GetSnapElement(SKPoint point, List<int> ignoreKeys, bool ignoreLocked, ElementKind kind, float maxDist = SnapDistance)
        {
            IElement result = TerminalPoint.Empty;

            foreach (var element in ElementsOfKindReversed(kind))
            {
	            var ignore = ignoreLocked && element.IsLocked;
	            if (!element.HasArea && !ignoreKeys.Contains(element.Key) && !ignore && element.DistanceToPoint(point) < maxDist)
	            {
		            result = element;
		            break;
	            }
            }

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

        public void UpdateConstraints(IElement changedElement)
        {
	        var related = Constraints.Where(constraint => constraint.HasElement(changedElement.Key));
	        foreach (var constraint in related)
	        {
		        constraint.OnElementChanged(changedElement);
	        }

        }
    }
}
