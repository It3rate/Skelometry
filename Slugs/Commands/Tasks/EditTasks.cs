using SkiaSharp;
using Slugs.Entities;
using Slugs.Input;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Commands.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditTask : TaskBase
    {
	    //public int Key { get; }
        public List<int> PreviousSelection = new List<int>();
	    //public SelectionKind BasedOn { get; protected set; }
        public override bool IsValid => true;

        protected EditTask(PadKind padKind) : base(padKind)
        {
	        //Key = ElementBase.EmptyKeyValue;
	        //BasedOn = SelectionKind.None;
	        Initialize();
        }
        //protected EditTask(PadKind padKind, SelectionKind basedOn) : base(padKind)
        //{
	       // //BasedOn = basedOn;
	       // //Key = ElementKeyForSelectionKind(basedOn);
	       // Initialize();
        //}
        //protected EditTask(PadKind padKind, int key) : base(padKind)
        //{
	       // Key = key;
	       // BasedOn = SelectionKindForElementKey(key);
	       // Initialize();
        //}

        public override void Initialize()
        {
	        base.Initialize();
            // record previous selection
            PreviousSelection.Add(Selected.Element.Key);
        }
    }

    public interface IPointTask : ITask
    {
	    int PointKey { get; }
	    void UpdateLocation(SKPoint location);
    }
    public interface ICreateTask
    {
    }
    public interface IChangeTask
    {
    }
    public class CreateTerminalPointTask : EditTask, IPointTask, ICreateTask
    {
	    public SKPoint Location { get; private set; }
	    public int PointKey => Point?.Key ?? TerminalPoint.EmptyKeyValue;
        private TerminalPoint Point { get; set; }

	    public CreateTerminalPointTask(PadKind padKind, SKPoint point) : base(padKind)
	    {
		    Location = point;
            // create point, assign key
        }

	    public override void RunTask()
	    {
		    Point = Pad.CreateTerminalPoint(Location);
	    }

	    public override void UnRunTask()
	    {
		    //Location = Point.SKPoint; // get location in case it has been updated
            Pad.RemoveElement(Point.Key);
            // todo: decrement key counter on undo
	    }

	    public void UpdateLocation(SKPoint location)
	    {
		    Location = location;
		    Point.SKPoint = Location;
	    }
    }
    public class CreateRefPointTask : EditTask, IPointTask, ICreateTask
    {
	    public int TargetKey { get; }
	    public int PointKey { get; }
        private RefPoint Point { get; set; }
        public CreateRefPointTask(PadKind padKind, int targetKey) : base(padKind)
        {
	        TargetKey = targetKey;
            // create ref, assign key
        }
        public void UpdateLocation(SKPoint location)
        {
	        Point.SKPoint = location;
        }
    }
    public class CreateVPointTask : EditTask, IPointTask, ICreateTask
    {
	    public int PointKey { get; }
        public int SegmentKey { get; }
	    public float T { get; }
	    private VPoint Point { get; set; }

        public CreateVPointTask(PadKind padKind, int segmentkey, float t) : base(padKind)
	    {
		    SegmentKey = segmentkey;
		    T = t;
            // create vpoint, assign key
	    }
	    public void UpdateLocation(SKPoint location)
	    {
		    Point.SKPoint = location;
	    }
    }
    public class MergePointsTask : EditTask, IPointTask, IChangeTask
    {
	    public int FromKey { get; }
	    public int ToKey { get; }
    
	    public int PointKey { get; }

        private VPoint Point { get; set; }

     //   public MergePointsTask(PadKind padKind, int fromKey) : base(padKind, SelectionKind.HighlightPoint)
	    //{
		   // FromKey = fromKey;
		   // //merge points
	    //}
	    public MergePointsTask(PadKind padKind, int fromKey, int toKey) : base(padKind)
	    {
		    FromKey = fromKey;
		    ToKey = toKey;
		    //merge points
	    }
	    public void UpdateLocation(SKPoint location)
	    {
		    Point.SKPoint = location;
	    }
    }
    
    public class CreateSegmentTask : EditTask, ICreateTask
    {
	    public int EntityKey { get; set; }
        public int StartPointKey { get; set; }
	    public int EndPointKey { get; set; }
	    public ElementKind SegmentKind { get; }

	    public SegmentBase Segment;

        public CreateSegmentTask(PadKind padKind, int entityKey, int startPointKey, int endPointKey, ElementKind segmentKind) : 
	        base(padKind)
        {
	        EntityKey = entityKey;
		    StartPointKey = startPointKey;
		    EndPointKey = endPointKey;
		    SegmentKind = segmentKind;
            // create segment, assign key
        }

        public override void RunTask()
        {
	        base.RunTask();
	        switch (SegmentKind)
	        {
                case ElementKind.Trait:
	                Segment = Pad.AddTrait(EntityKey, StartPointKey, EndPointKey, 66);
	                break;
                case ElementKind.Focal:
	                break;
                case ElementKind.Bond:
	                break;
            }
        }
    }
    public class CreateEntityTask : EditTask, ICreateTask
    {
	    public int EntityKey { get; set; }
        public CreateEntityTask(PadKind padKind) : base(padKind)
	    {
            // create entity, assign key
	    }

        public override void RunTask()
        {
	        base.RunTask();
	        var entity = new Entity(PadKind);
	        EntityKey = entity.Key;
        }
    }
    public class DuplicateElementTask : EditTask, ICreateTask
    {
        public int SourceKey { get; }

        //public DuplicateElementTask(PadKind padKind) : base(padKind)
        //{
	       // BasedOn = SelectionKind.SelectedElement;
        //    SourceKey = ElementKeyForSelectionKind(SelectionKind.SelectedElement);
        //    // dup element, assign key
        //}
        public DuplicateElementTask(PadKind padKind, int sourceKey) : base(padKind)
        {
	        SourceKey = sourceKey;
	        //BasedOn = SelectionKindForElementKey(sourceKey);
	        // dup element, assign key
        }
    }
    public class RemoveElementTask : EditTask
    {
	    public int ElementKey { get; }

	    public RemoveElementTask(PadKind padKind, int key) : base(padKind)
	    {
		    ElementKey = key;
	    }
    }
    public class GroupPointsTask : EditTask, ICreateTask
    {
        public List<int> FromKeys { get; }

        public GroupPointsTask(PadKind padKind, params int[] fromKeys) : base(padKind)
        {
	        FromKeys = new List<int>(fromKeys);
            // create group, assign key
        }
    }
    public class UngroupPointsTask : EditTask
    {
	    public int FromKey { get; }
        public List<int> ToKeys { get; }
	    public UngroupPointsTask(PadKind padKind, int fromKey) : base(padKind)
	    {
		    FromKey = fromKey;
            //ToKeys = new List<int>(toKeys);
		    // ungroup, map new keys, assign key
        }
    }
    public class MoveElementTask : EditTask, IChangeTask
    {
	    public int ElementKey { get; set; }
        public SKPoint Diff { get; } // always relative

	    public MoveElementTask(PadKind padKind, int key, SKPoint diff) : base(padKind)
	    {
		    ElementKey = key;
		    Diff = diff;
	    }
    }
    public class MoveVPointTask : EditTask, IChangeTask // add to start or end (not stretch)
    {
	    public int VPointKey { get; set; }
	    public float T { get; } // always absolute
	    public float OriginalT { get; }

	    //public MoveVPointTask(PadKind padKind, float t) : base(padKind, SelectionKind.SelectedPoint)
	    //{
		   // T = t;
		   // OriginalT = ((VPoint)Pad.PointAt(VPointKey)).GetT();
	    //}
	    public MoveVPointTask(PadKind padKind, int vPointKey, float t) : base(padKind)
	    {
		    VPointKey = vPointKey;
		    T = t;
		    OriginalT = ((VPoint)Pad.PointAt(VPointKey)).GetT();
	    }
    }
    public class StretchSegmentTask : EditTask, IChangeTask
    {
	    public int PointKey { get; set; }
        public float Length { get; }
	    public SKPoint OriginalPosition { get; }

	    //public StretchSegmentTask(PadKind padKind, float length) : base(padKind, SelectionKind.SelectedPoint)
	    //{
		   // Length = length;
		   // OriginalPosition = Pad.PointAt(Key).SKPoint;
	    //}
	    public StretchSegmentTask(PadKind padKind, int key, float length) : base(padKind)
	    {
            PointKey = key;
            Length = length;
		    OriginalPosition = Pad.PointAt(PointKey).SKPoint;
	    }
    }
    public class AppendSelectionTask : EditTask
    {
	    public int KeyToAppend { get; }
     //   public AppendSelectionTask(PadKind padKind) : base(padKind, SelectionKind.HighlightElement)
	    //{
	    //}
	    public AppendSelectionTask(PadKind padKind, int keyToAppend) : base(padKind)
	    {
		    KeyToAppend = keyToAppend;
	    }
    }
    public class RemoveSelectionTask : EditTask
    {
	    public int KeyToRemove { get; }
     //   public RemoveSelectionTask(PadKind padKind) : base(padKind, SelectionKind.HighlightElement)
	    //{
	    //}
	    public RemoveSelectionTask(PadKind padKind, int keyToRemove) : base(padKind)
	    {
		    KeyToRemove = keyToRemove;
	    }
    }
    //public class MultiSelectionTask : EditTask // probably multi-selections with always be temporary groups (or a fixed group on Working), so this may not be needed.
    //{
	   // public List<int> AppendKeys { get; } = new List<int>();

	   // public MultiSelectionTask(PadKind padKind, params int[] keysToAppend) : base(padKind, SelectionKind.HighlightElement)
	   // {
    //        AppendKeys.AddRange(keysToAppend);
	   // }
    //}
    //public class ClearSelectionTask : EditTask
    //{
	   // public List<int> RemoveKeys { get; } = new List<int>();

	   // public ClearSelectionTask(PadKind padKind, params int[] keysToRemove) : base(padKind, SelectionKind.HighlightElement)
	   // {
		  //  RemoveKeys.AddRange(keysToRemove);
	   // }
    //}
}


