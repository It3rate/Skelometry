using SkiaSharp;
using Slugs.Entities;

namespace Slugs.Commands.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EditTask : TaskBase
    {
	    public List<SKPoint> PreviousSelection;
    }
    public class RemoveElementTask : EditTask
    {
	    public int Key { get; } // don't remove elements, just deactivate them or put into a deleted list.
    }
    public class CreateTerminalPointTask : EditTask
    {
	    public int Key { get; }
	    public SKPoint Location { get; }
    }
    public class CreateRefPointTask : EditTask
    {
	    public int Key { get; }
	    public int TargetKey { get; }
    }
    public class CreateVPointTask : EditTask
    {
	    public int Key { get; }
	    public int SegmentKey { get; }
	    public float T { get; }
    }
    public class CreateSegmentTask : EditTask
    {
	    public int Key { get; }
	    public int StartPointKey { get; }
	    public int EndPointKey { get; }
	    public ElementKind SegmentKind { get; }
    }
    public class CreateEnitityTask : EditTask
    {
	    public int Key { get; }
    }
    public class DuplicateElementTask : EditTask
    {
	    public int Key { get; }
	    public int SourceKey { get; }
    }
    public class MergePointsTask : EditTask
    {
	    public int FromKey { get; }
	    public int ToKey { get; }
    }
    public class GroupPointsTask : EditTask
    {
	    public List<int> FromKeys { get; }
	    public int ToKey { get; }
    }
    public class UngroupPointsTask : EditTask
    {
	    public int FromKey { get; }
	    public List<int> ToKeys { get; }
    }
    public class MoveElementTask : EditTask
    {
	    public int ElementKey { get; }
	    public SKPoint Amount { get; }
	    public SKPoint OriginalAmount { get; }
        public bool AsAbsolute { get; }
    }
    public class MoveVPointTask : EditTask // add to start or end (not stretch)
    {
	    public int VPointKey { get; }
	    public float Amount { get; }
	    public float OriginalAmount { get; }
	    public bool AsAbsolute { get; }
    }
    public class StretchVPointTask : EditTask // add to start or end (not stretch)
    {
	    public int VPointKey { get; }
	    public float T { get; }
	    public float OriginalT { get; }
	    public bool AsAbsolute { get; }
    }
}
