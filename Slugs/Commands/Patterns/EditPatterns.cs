using System.Windows.Forms;
using Slugs.Agents;
using Slugs.Commands.EditCommands;
using Slugs.Commands.Tasks;
using Slugs.Entities;
using Slugs.Input;

namespace Slugs.Commands.Patterns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
        public SelectionKind SourceKind { get; protected set; }
        public SelectionKind SelectionKind { get; protected set; }

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
        // tasks:
        // get/create p0, p1
        // create trait with points
        // move p1 until mouse up
        // merge p1 to highlight point as needed
        private IPointTask StartPoint { get; }
        private IPointTask EndPoint { get; }
        private CreateTraitTask CreateTrait { get; }
        private MoveElementTask MoveEndPoint { get; } // or just directly change Endpoint?
        private MergePointsTask MergeEndPoint { get; }


        public override bool IsValid => true; //SourceSet.HasKind(SourceKind) && TargetSet.HadKind(SelectionKind);
                                              //public override string Name => "Edit Pattern";
                                              //public override Keys Keys => Keys.Escape;
                                              //public override EditKind EditKind => EditKind.Create;
                                              //public override SelectionKind SelectionKind => SelectionKind.WorkingElement;

        public CreateTraitPattern(Agent agent) : base(agent, agent.WorkingPad, agent.InputPad)
        {
            Name = "Edit Pattern";
            Keys = Keys.Escape;
            EditKind = EditKind.Create;
            //SelectionKind = SelectionKind.WorkingElement;
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
}
