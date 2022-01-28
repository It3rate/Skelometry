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
	    public List<int> PreviousSelection = new List<int>();
	    public SelectionKind StartKind { get; protected set; }
	    public SelectionKind EndKind { get; protected set; } = SelectionKind.None;
        public override bool IsValid => true;

        public EditTask(PadKind padKind, SelectionKind startKind) : base(padKind)
        {
	        StartKind = startKind;
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
	    public int Key { get; } // don't remove elements, just deactivate them or put into a deleted list.

	    public RemoveElementTask(PadKind padKind, int key) : base(padKind, SelectionKind.SelectedElement)
	    {
		    Key = key;
	    }
    }
    public class CreateTerminalPointTask : EditTask
    {
	    public int Key { get; }
	    public SKPoint Location { get; }

        public CreateTerminalPointTask(PadKind padKind, int key) : base(padKind, SelectionKind.None) { Key = key; }
    }
    public class CreateRefPointTask : EditTask
    {
	    public int Key { get; }
        public int TargetKey { get; }

        public CreateRefPointTask(PadKind padKind, int key) : base(padKind, SelectionKind.None) { Key = key; }
    }
    public class CreateVPointTask : EditTask
    {
	    public int Key { get; }
        public int SegmentKey { get; }
	    public float T { get; }

	    public CreateVPointTask(PadKind padKind, int key) : base(padKind, SelectionKind.None) { Key = key; }
    }
    public class CreateSegmentTask : EditTask
    {
	    public int Key { get; }
        public int StartPointKey { get; }
	    public int EndPointKey { get; }
	    public ElementKind SegmentKind { get; }

	    public CreateSegmentTask(PadKind padKind, int key) : base(padKind, SelectionKind.BeginElement) { Key = key; }
    }
    public class CreateEnitityTask : EditTask
    {
	    public int Key { get; }

	    public CreateEnitityTask(PadKind padKind, int key) : base(padKind, SelectionKind.None) { Key = key; }
    }
    public class DuplicateElementTask : EditTask
    {
	    public int Key { get; }
        public int SourceKey { get; }

        public DuplicateElementTask(PadKind padKind, int key) : base(padKind, SelectionKind.SelectedElement) { Key = key; }
    }
    public class MergePointsTask : EditTask
    {
        public int FromKey { get; }
	    public int ToKey { get; }

	    public MergePointsTask(PadKind padKind, int fromKey, int toKey) : base(padKind, SelectionKind.HighlightPoint)
	    {
		    FromKey = fromKey;
		    ToKey = toKey;
	    }
    }
    public class GroupPointsTask : EditTask
    {
        public List<int> FromKeys { get; }
	    public int ToKey { get; }

	    public GroupPointsTask(PadKind padKind, int toKey, params int[] fromKeys) : base(padKind, SelectionKind.SelectedElement)
	    {
		    ToKey = toKey;
		    FromKeys = new List<int>(fromKeys);
	    }
    }
    public class UngroupPointsTask : EditTask
    {
        public int FromKey { get; }
	    public List<int> ToKeys { get; }
	    public UngroupPointsTask(PadKind padKind, int fromKey, params int[] toKeys) : base(padKind, SelectionKind.SelectedElement)
	    {
		    FromKey = fromKey;
		    ToKeys = new List<int>(toKeys);
	    }
    }
    public class MoveElementTask : EditTask
    {
        public int ElementKey { get; }
	    public SKPoint Diff { get; } // always relative

	    public MoveElementTask(PadKind padKind, SKPoint diff) : base(padKind, SelectionKind.SelectedElement)
	    {
		    Diff = diff;
	    }
    }
    public class MoveVPointTask : EditTask // add to start or end (not stretch)
    {
        public int VPointKey { get; }
	    public float T { get; } // always absolute
	    public float OriginalT { get; }

	    public MoveVPointTask(PadKind padKind, int vPointKey, float t) : base(padKind, SelectionKind.SelectedPoint)
	    {
		    VPointKey = vPointKey;
            T = t;
            OriginalT = ((VPoint)Pad.PointAt(VPointKey)).GetT();
	    }
    }
    public class StretchSegmentTask : EditTask
    {
	    public int PointKey { get; }
	    public float Length { get; }
	    public SKPoint OriginalPosition { get; }

	    public StretchSegmentTask(PadKind padKind, float length) : base(padKind, SelectionKind.SelectedPoint)
	    {
		    Length = length;
		    OriginalPosition = Pad.PointAt(PointKey).SKPoint;
	    }
    }
    public class AppendSelectionTask : EditTask 
    {
	    public int AppendedKey { get; }

	    public AppendSelectionTask(PadKind padKind, int keyToAppend) : base(padKind, SelectionKind.HighlightElement)
	    {
		    AppendedKey = keyToAppend;
	    }
    }
    public class RemoveSelectionTask : EditTask
    {
	    public int RemovedKey { get; }

	    public RemoveSelectionTask(PadKind padKind, int keyToRemove) : base(padKind, SelectionKind.HighlightElement)
	    {
		    RemovedKey = keyToRemove;
	    }
    }
    public class MultiSelectionTask : EditTask // probably multi-selections with always be temporary groups (or a fixed group on Working), so this may not be needed.
    {
	    public List<int> AppendKeys { get; } = new List<int>();

	    public MultiSelectionTask(PadKind padKind, params int[] keysToAppend) : base(padKind, SelectionKind.HighlightElement)
	    {
            AppendKeys.AddRange(keysToAppend);
	    }
    }
    public class ClearSelectionTask : EditTask
    {
	    public List<int> RemoveKeys { get; } = new List<int>();

	    public ClearSelectionTask(PadKind padKind, params int[] keysToRemove) : base(padKind, SelectionKind.HighlightElement)
	    {
		    RemoveKeys.AddRange(keysToRemove);
	    }
    }
}
