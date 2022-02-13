﻿using System.Drawing;
using SkiaSharp;
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

        private readonly Dictionary<int, IElement> _elements = new Dictionary<int, IElement>();

        public static Slug ActiveSlug = Slug.Unit;
        private static readonly List<Slug> Slugs = new List<Slug>(); // -1 is 'none' position, 0 is activeSlug.
        public const float SnapDistance = 10.0f;

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

        public void Clear()
        {
            ClearElements();
        }
        public void Refresh()
        {
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
