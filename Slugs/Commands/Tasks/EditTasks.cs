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
            PreviousSelection.AddRange(Selected.ElementKeys);
        }
    }
    public interface IPointTask : ITask
    {
	    int PointKey { get; }
	    IPoint IPoint { get; }
    }
    public interface ICreateTask
    {
    }
    public interface IChangeTask
    {
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
    public class MoveFocalPointTask : EditTask, IChangeTask // add to start or end (not stretch)
    {
	    public int PointKey { get; set; }
	    public float T { get; } // always absolute
	    public float OriginalT { get; }

	    public MoveFocalPointTask(PadKind padKind, int pointKey, float t) : base(padKind)
	    {
		    PointKey = pointKey;
		    T = t;
		    OriginalT = ((FocalPoint)Pad.PointAt(PointKey)).GetT();
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
		   // OriginalPosition = Pad.PointAt(Key).Position;
	    //}
	    public StretchSegmentTask(PadKind padKind, int key, float length) : base(padKind)
	    {
            PointKey = key;
            Length = length;
		    OriginalPosition = Pad.PointAt(PointKey).Position;
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


