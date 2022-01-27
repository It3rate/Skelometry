using System.Windows.Forms;
using OpenTK.Input;
using SkiaSharp;
using Slugs.Agents;
using Slugs.Commands;
using Slugs.Entities;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // recipes, data locations and constraints for possible commands.
    public interface IPattern
    {
        Pad SourcePad { get; }
        string Name { get; }

        bool IsValid { get; }
        Keys Keys { get; }

        ICommand CreateCommand();
        void AddToCommand(ICommand command);
    } 

    public abstract class EditPattern : IPattern
    {
	    public Agent Agent { get; }
	    public Pad SourcePad { get; }
	    public Pad TargetPad { get; }
	    public SelectionSet SourceSet { get; }
	    public SelectionSet TargetSet { get; }

        public abstract bool IsValid { get; }

        public string Name { get; protected set; }
	    public Keys Keys { get; protected set; }
	    public EditKind EditKind { get; protected set; }
	    public TargetKind SourceKind { get; protected set; }
	    public TargetKind TargetKind { get; protected set; }

        public EditPattern(Agent agent, Pad sourcePad, Pad targetPad)
	    {
		    Agent = agent;
		    SourcePad = sourcePad;
		    TargetPad = targetPad;
        }

	    public abstract ICommand CreateCommand();
	    public abstract void AddToCommand(ICommand command);
    }

    public class CreateTraitPattern : EditPattern
    {
	    public override bool IsValid => true; //SourceSet.HasKind(SourceKind) && TargetSet.HadKind(TargetKind);
	    //public override string Name => "Edit Pattern";
	    //public override Keys Keys => Keys.Escape;
	    //public override EditKind EditKind => EditKind.Create;
	    //public override TargetKind TargetKind => TargetKind.WorkingElement;

        public CreateTraitPattern(Agent agent) : base(agent, agent.WorkingPad, agent.InputPad)
        {
	        Name = "Edit Pattern";
            Keys = Keys.Escape;
	        EditKind = EditKind.Create;
	        TargetKind = TargetKind.WorkingElement;
        }
	    public override ICommand CreateCommand()
	    {
            // remember selection state
            // points to create, focus point key, other point keys
            // or: create points, merge point pairs, moves[key, prevpoint, nextpoint], construction points
            // select new element
            throw new NotImplementedException();
	    }

	    public override void AddToCommand(ICommand command)
	    {
		    throw new NotImplementedException();
	    }
    }

    public enum EditKind
    {
        Create,
        Delete,
        Merge,
        Move,
        Extend,
        Duplicate,
        Connect,
        TempGroup,
        Group,
        Ungroup,
    }

    public enum TargetKind
    {
	    WorkingPoint,
	    WorkingElement,
        StartPoint,
	    StartElement,
	    CurrentPoint,
	    CurrentElement,
        SelectedPoint,
	    SelectedElement,
	    HighlightPoint,
	    HighlightElement,
	    ClipboardPoint,
	    ClipboardElement,
	    LastCommand,
        ToolKind,
    }
    public enum UIMode
    {
	    Inspect,
	    Edit,
        Interact,
	    Animate,
    }
}
