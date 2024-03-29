﻿using System.Drawing;
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
	        if (element is TerminalPoint tp)
	        {
		        var values = _elements.Values.Where(val => val is RefPoint rp && rp.TargetKey == tp.Key).ToList();
		        foreach (var val in values)
		        {
			        Dereferece((RefPoint)val);
		        }
	        }

            _elements.Remove(key);

	        if (element is Trait)
	        {
		        _traitKeys.Remove(element.Key);
	        }
	        else if (element is Entity)
	        {
		        _entityKeys.Remove(element.Key);
	        }
	        else if (element is Focal focal)
	        {
		        focal.Trait.RemoveFocal(focal);
	        }
        }

        public void Dereferece(RefPoint refPoint)
        {
	        _elements[refPoint.Key] = new TerminalPoint(PadKind, refPoint.Position);
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
        public int[] TraitsWithKey(int key)
        {
	        var result = new List<int>();
	        foreach (var trait in Traits)
	        {
		        if (trait.StartKey == key || trait.EndKey == key)
		        {
			        result.Add(trait.Key);
		        }
	        }
	        return result.ToArray();
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
        #region _constraints
	    private Dictionary<int, IConstraint> _constraintMap { get; } = new Dictionary<int, IConstraint>();

        public bool CanAddConstraint(IConstraint constraint)
	    {
		    return !_constraintMap.Keys.Contains(constraint.Key); // todo: check for validity of constraint
	    }
        public bool AddConstraint(IConstraint constraint)
        {
	        var result = CanAddConstraint(constraint);
	        if (result)
	        {
		        _constraintMap.Add(constraint.Key, constraint);
	        }
	        
	        return result;
        }
        public void AddConstraints(IEnumerable<IConstraint> constraints)
        {
	        foreach (var constraint in constraints.ToArray())
	        {
		        AddConstraint(constraint);
	        }
        }
        public bool RemoveConstraint(IConstraint constraint)
        {
	        var result = _constraintMap.Keys.Contains(constraint.Key);
	        if (result)
	        {
		        _constraintMap.Remove(constraint.Key);
	        }
	        return result;
        }
        public void RemoveConstraints(IEnumerable<IConstraint> constraints)
        {
	        foreach (var constraint in constraints.ToArray())
	        {
		        RemoveConstraint(constraint);
	        }
        }
        public IEnumerable<IConstraint> GetRelatedConstraints(IElement element)
        {
	        return _constraintMap.Values.Where(constraint => constraint.HasElement(element.Key));
        }

        public void UpdateConstraints(IElement changedElement, Dictionary<int, SKPoint> adjustedElements)
	    {
		    if (!changedElement.IsLocked && !adjustedElements.ContainsKey(changedElement.Key))
		    {
			    adjustedElements.Add(changedElement.Key, changedElement.Center);
			    var related = GetRelatedConstraints(changedElement);
			    //_constraintMap.Values.Where(constraint => constraint.HasElement(changedElement.Key));
			    foreach (var constraint in related)
			    {
				    if (!adjustedElements.ContainsKey(constraint.Key))
				    {
					    constraint.OnElementChanged(changedElement, adjustedElements);
				    }
			    }
		    }
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

    }
}
