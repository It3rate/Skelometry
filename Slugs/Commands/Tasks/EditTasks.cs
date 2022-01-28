using SkiaSharp;
using Slugs.Entities;
using Slugs.Input;
using Slugs.Pads;

namespace Slugs.Commands.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditTask : TaskBase
    {
	    public int Key { get; }
        public List<int> PreviousSelection = new List<int>();
	    public SelectionKind BasedOn { get; protected set; }
        public override bool IsValid => true;

        protected EditTask(PadKind padKind) : base(padKind)
        {
	        Key = ElementBase.EmptyKeyValue;
	        BasedOn = SelectionKind.None;
	        Initialize();
        }
        protected EditTask(PadKind padKind, SelectionKind basedOn) : base(padKind)
        {
	        BasedOn = basedOn;
	        Key = ElementKeyForSelectionKind(basedOn);
	        Initialize();
        }
        protected EditTask(PadKind padKind, int key) : base(padKind)
        {
	        Key = key;
	        BasedOn = SelectionKindForElementKey(key);
	        Initialize();
        }

        public override void Initialize()
        {
	        base.Initialize();
            // record previous selection
            PreviousSelection.Add(Selected.Selection.Key);
        }
    }

    public class RemoveElementTask : EditTask
    {
	    public RemoveElementTask(PadKind padKind) : base(padKind, SelectionKind.SelectedElement) { }
    }
    public class CreateTerminalPointTask : EditTask
    {
	    public SKPoint Location { get; }

	    public CreateTerminalPointTask(PadKind padKind, SKPoint point) : base(padKind)
	    {
		    Location = point;
            // create point, assign key
	    }
    }
    public class CreateRefPointTask : EditTask
    {
        public int TargetKey { get; }

        public CreateRefPointTask(PadKind padKind, int targetKey) : base(padKind)
        {
	        TargetKey = targetKey;
	        // create ref, assign key
        }
    }
    public class CreateVPointTask : EditTask
    {
        public int SegmentKey { get; }
	    public float T { get; }

	    public CreateVPointTask(PadKind padKind, int segmentkey, float t) : base(padKind)
	    {
		    SegmentKey = segmentkey;
		    T = t;
            // create vpoint, assign key
	    }
    }
    public class CreateSegmentTask : EditTask
    {
        public int StartPointKey { get; }
	    public int EndPointKey { get; }
	    public ElementKind SegmentKind { get; }

	    public CreateSegmentTask(PadKind padKind, int startPointKey, int endPointKey, ElementKind segmentKind) : base(padKind, SelectionKind.BeginElement)
	    {
		    StartPointKey = startPointKey;
		    EndPointKey = endPointKey;
		    SegmentKind = segmentKind;
		    // create segment, assign key
	    }
    }
    public class CreateEntityTask : EditTask
    {
	    public CreateEntityTask(PadKind padKind) : base(padKind)
	    {
            // create entity, assign key
	    }
    }
    public class DuplicateElementTask : EditTask
    {
        public int SourceKey { get; }

        public DuplicateElementTask(PadKind padKind) : base(padKind)
        {
	        BasedOn = SelectionKind.SelectedElement;
            SourceKey = ElementKeyForSelectionKind(SelectionKind.SelectedElement);
            // dup element, assign key
        }
        public DuplicateElementTask(PadKind padKind, int sourceKey) : base(padKind)
        {
	        SourceKey = sourceKey;
	        BasedOn = SelectionKindForElementKey(sourceKey);
	        // dup element, assign key
        }
    }
    public class MergePointsTask : EditTask
    {
	    public int FromKey { get; }
	    public int ToKey => Key;

	    public MergePointsTask(PadKind padKind, int fromKey) : base(padKind, SelectionKind.HighlightPoint)
	    {
		    FromKey = fromKey;
            //merge points
	    }
	    public MergePointsTask(PadKind padKind, int fromKey, int toKey) : base(padKind, toKey)
	    {
		    FromKey = fromKey;
		    //merge points
        }
    }
    public class GroupPointsTask : EditTask
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
    public class MoveElementTask : EditTask
    {
	    public SKPoint Diff { get; } // always relative

	    public MoveElementTask(PadKind padKind, SKPoint diff) : base(padKind, SelectionKind.SelectedElement)
	    {
		    Diff = diff;
	    }
	    public MoveElementTask(PadKind padKind, int key, SKPoint diff) : base(padKind, key)
	    {
		    Diff = diff;
	    }
    }
    public class MoveVPointTask : EditTask // add to start or end (not stretch)
    {
	    public int VPointKey => Key;
	    public float T { get; } // always absolute
	    public float OriginalT { get; }

	    public MoveVPointTask(PadKind padKind, float t) : base(padKind, SelectionKind.SelectedPoint)
	    {
		    T = t;
		    OriginalT = ((VPoint)Pad.PointAt(Key)).GetT();
	    }
	    public MoveVPointTask(PadKind padKind, int vPointKey, float t) : base(padKind, vPointKey)
	    {
		    T = t;
		    OriginalT = ((VPoint)Pad.PointAt(Key)).GetT();
	    }
    }
    public class StretchSegmentTask : EditTask
    {
	    public float Length { get; }
	    public SKPoint OriginalPosition { get; }

	    public StretchSegmentTask(PadKind padKind, float length) : base(padKind, SelectionKind.SelectedPoint)
	    {
		    Length = length;
		    OriginalPosition = Pad.PointAt(Key).SKPoint;
	    }
	    public StretchSegmentTask(PadKind padKind, int key, float length) : base(padKind, key)
	    {
		    Length = length;
		    OriginalPosition = Pad.PointAt(Key).SKPoint;
	    }
    }
    public class AppendSelectionTask : EditTask
    {
	    public int KeyToAppend => Key;
        public AppendSelectionTask(PadKind padKind) : base(padKind, SelectionKind.HighlightElement)
	    {
	    }
	    public AppendSelectionTask(PadKind padKind, int keyToAppend) : base(padKind, keyToAppend)
	    {
	    }
    }
    public class RemoveSelectionTask : EditTask
    {
	    public int KeyToRemove => Key;
        public RemoveSelectionTask(PadKind padKind) : base(padKind, SelectionKind.HighlightElement)
	    {
	    }
	    public RemoveSelectionTask(PadKind padKind, int keyToRemove) : base(padKind, keyToRemove)
	    {
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
