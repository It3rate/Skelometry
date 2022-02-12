using SkiaSharp;
using Slugs.Commands.Tasks;
using Slugs.Entities;
using Slugs.Pads;

namespace Slugs.Commands.EditCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MoveElementCommand : EditCommand, IDraggableCommand
    {
	    public MoveElementTask MoveTask { get; }
	    public int ElementKey => MoveTask?.ElementKey ?? ElementBase.EmptyKeyValue;

	    public IPoint DraggablePoint => ElementKey >= 0 ? Pad.PointAt(ElementKey) : TerminalPoint.Empty;
	    public bool HasDraggablePoint => !DraggablePoint.IsEmpty;

	    public MoveElementCommand(Pad pad, IElement element) : base(pad)
	    {
            MoveTask = new MoveElementTask(Pad.PadKind, element, SKPoint.Empty);
            AddTaskAndRun(MoveTask);
        }
	    public override void Execute()
	    {
            // Add move task
	    }
	    public override void Update(SKPoint point)
	    {
	    }
	    public override void Completed()
	    {
	    }

    }
}
