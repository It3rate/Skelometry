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

    public class MoveElementCommand : EditCommand, IDraggablePointCommand
    {
	    public MoveElementTask MoveTask { get; }
	    public int ElementKey => MoveTask?.ElementKey ?? ElementBase.EmptyKeyValue;

	    public IPoint DraggablePoint => ElementKey >= 0 ? Pad.PointAt(ElementKey) : TerminalPoint.Empty;

	    public MoveElementCommand(Pad pad, int elementKey) : base(pad)
	    {
            MoveTask = new MoveElementTask(Pad.PadKind, elementKey, SKPoint.Empty);
            AddTaskAndRun(MoveTask);
        }
	    public virtual void Execute()
	    {
            // Add move task
	    }
	    public virtual void Update(SKPoint point)
	    {
	    }
	    public virtual void Completed()
	    {
	    }

    }
}
