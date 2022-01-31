using Slugs.Agents;
using Slugs.Commands.EditCommands;
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

    public interface ITask
    {
	    ICommand Command { get; }
	    Pad Pad { get; }
	    PadKind PadKind { get; }
        int TaskKey { get; }
        bool IsValid { get; }
        void RunTask();
        void UnRunTask();
        //void OnAddedToCommand(ICommand command);
    }

    public abstract class TaskBase : ITask
    {
	    private static int TaskCounter = 1;

	    public ICommand Command { get; }
	    public Pad Pad => Agent.Current.PadAt(PadKind);
        public PadKind PadKind { get; }
        public SelectionSet Begin => Agent.Current.Data.Begin;
        public SelectionSet Current => Agent.Current.Data.Current;
        public SelectionSet Highlight => Agent.Current.Data.Highlight;
        public SelectionSet Selected => Agent.Current.Data.Selected;
        public SelectionSet Clipboard => Agent.Current.Data.Clipboard;

        public int TaskKey { get; }
        public abstract bool IsValid { get; }
        public virtual void Initialize() { }

        protected TaskBase(PadKind padKind)
        {
	        PadKind = padKind;
	        TaskKey = TaskCounter++;
        }
        public virtual void RunTask() { }
        public virtual void UnRunTask() { }

        protected int ElementKeyForSelectionKind(SelectionKind kind)
        {
	        int result = ElementBase.EmptyKeyValue;
	        switch (kind)
	        {
		        case SelectionKind.BeginPoint:
			        result = Begin.Point.Key;
			        break;
		        case SelectionKind.BeginElement:
			        result = Begin.Element.Key;
			        break;
		        case SelectionKind.CurrentPoint:
			        result = Current.Point.Key;
			        break;
		        case SelectionKind.CurrentElement:
			        result = Current.Element.Key;
			        break;
		        case SelectionKind.SelectedPoint:
			        result = Selected.Point.Key;
			        break;
		        case SelectionKind.SelectedElement:
			        result = Selected.Element.Key;
			        break;
		        case SelectionKind.HighlightPoint:
			        result = Highlight.Point.Key;
			        break;
		        case SelectionKind.HighlightElement:
			        result = Highlight.Element.Key;
			        break;
		        case SelectionKind.ClipboardPoint:
			        result = Clipboard.Point.Key;
			        break;
		        case SelectionKind.ClipboardElement:
			        result = Clipboard.Element.Key;
			        break;
                default:
	                break;
	        }

	        return result;
        }
        protected SelectionKind SelectionKindForElementKey(int key)
        {
	        SelectionKind result = SelectionKind.None;
	        if (Begin.Point.Key == key)
	        {
		        result = SelectionKind.BeginPoint;
	        }
	        else if (Begin.Element.Key == key)
	        {
		        result = SelectionKind.BeginElement;
	        }
	        else if (Current.Point.Key == key)
	        {
		        result = SelectionKind.CurrentPoint;
	        }
	        else if (Current.Element.Key == key)
	        {
		        result = SelectionKind.CurrentElement;
	        }
	        else if (Selected.Point.Key == key)
	        {
		        result = SelectionKind.SelectedPoint;
	        }
	        else if (Selected.Element.Key == key)
	        {
		        result = SelectionKind.SelectedElement;
	        }
	        else if (Highlight.Point.Key == key)
	        {
		        result = SelectionKind.HighlightPoint;
	        }
	        else if (Highlight.Element.Key == key)
	        {
		        result = SelectionKind.HighlightElement;
	        }
	        else if (Clipboard.Point.Key == key)
	        {
		        result = SelectionKind.ClipboardPoint;
	        }
	        else if (Clipboard.Element.Key == key)
	        {
		        result = SelectionKind.ClipboardElement;
	        }

            return result;
        }
    }
}
