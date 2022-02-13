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
	    public MoveElementTask MoveTask { get; private set; }
        public IElement Element { get; }
        public int ElementKey => Element.Key;

        public SKPoint Diff { get; }

        public IPoint DraggablePoint => ElementKey >= 0 ? Pad.PointAt(ElementKey) : TerminalPoint.Empty;
	    public bool HasDraggablePoint => !DraggablePoint.IsEmpty;

	    public MoveElementCommand(Pad pad, IElement element, SKPoint diff) : base(pad)
	    {
		    Element = element;
		    Diff = diff;
	    }
	    public override void Execute()
	    {
            base.Execute();
            if (MoveTask == null)
            {
				MoveTask = new MoveElementTask(Pad.PadKind, Element, Diff);
            }
            AddTaskAndRun(MoveTask);
	    }
	    public override void Unexecute()
	    {
		    base.Unexecute();
	    }
        public override void Update(SKPoint point)
	    {
	    }
	    public override void Completed()
	    {
	    }

    }
}
