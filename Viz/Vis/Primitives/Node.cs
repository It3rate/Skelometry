using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Distributions;

namespace MLTest.Vis
{
	public class Node : IPrimitive
    {
	    public IPath Reference { get; } // if reference is a stroke, this must be a joint
		public float Position { get; }

		public Node PreviousNode { get; private set; }
		public Node NextNode { get; private set; }

        // calculated
        public Point Anchor { get; protected set; }
        public float X => Anchor.X;
        public float Y => Anchor.Y;

		public virtual Point Start => Anchor;
		public virtual Point End => Anchor;

        public Node(IPath reference, float position)
		{
			Reference = reference;
			Position = position;

			Anchor = reference.GetPoint(position);
		}


        public Point GetPoint(float position, float offset=0)
		{
			throw new NotImplementedException();
		}


        public float Similarity(IPrimitive p) => 0;
        public Point Sample(Gaussian g) => null;

        public override string ToString()
		{
			return Anchor.ToString();
		}
    }

    /// <summary>
    /// Node that joins the reference path perpendicularly at the nearest point. Mostly for circles. If inside, connects to closest edge. If center connects to origin.
    /// Line: if it isn't valid for a line segment, it will connect to the imaginary extended line?
    /// Arc: Connects to imaginary edge if arc isn't complete. If inside, connects to nearest interior point on arc, if invalid behaves as circle.
    /// </summary>
	public class PerpendicularNode : Node
	{
		public PerpendicularNode(IPath reference): base(reference, 0)
		{
		}
    }

	// The first node on a circleRef needs to specify it's direction as there are two tangent lines. Points inside the circleRef will move to intersecting point of the second node's reference based on direction.
    public class TangentNode : Node
	{
		public ClockDirection Direction { get; }
		public Circle CircleRef;
		private Point _start;
		public override Point Start => _start;
		private Point _end;
		public override Point End => _end;

        public TangentNode(Circle circleRef, ClockDirection direction = ClockDirection.CW) : base(circleRef, 0)
        {
	        CircleRef = circleRef;
	        Direction = direction;
        }

        public Point GetTangentFromPoint(Node node)
        {
            // if node is null, get point on circumference using position. 
            // if it is another Tangent node use circleRef tangents
	        _start = CircleRef.FindTangentInDirection(node.End, Direction);
	        return _start;
        }
        public Point GetTangentToPoint(Node node)
        {
            // if p is null, get point on circumference using position.
            // if it is another Tangent node use circleRef tangents
            _end = CircleRef.FindTangentInDirection(node.Start, Direction.Counter());
            return _end;
        }
        public override string ToString()
        {
	        return String.Format("tanNode:{0:0.##},{1:0.##} e{2:0.##},{3:0.##}", Start.X, Start.Y, End.X, End.Y);
        }
    }

	public class TipNode : Node
	{
		// Offset can't be zero in a middle node, as it causes overlap on lines that are tangent to each other. 
		// The corner of a P is part of the shape with potential overlap on the serif.
		// Maybe X could be a V with overlap.H would be a half U with 0.5 overlap. Maybe this is too obfuscated. Yes it is. Might work for serifs though.
		public float Offset { get; }

		public TipNode(IPath reference, float position, float offset) : base(reference, position)
		{
			Offset = offset;
			Anchor = reference.GetPoint(position, offset);
        }
        //public TipNode(IPath reference, float position, float offset, float length) : base(reference, position)
        //{
        //	Offset = offset;
        //	Length = length;
        //}
        public override string ToString()
        {
	        return String.Format("tipNode:{0:0.##},{1:0.##} o{2:0.##}", Anchor.X, Anchor.Y, Offset);
        }
    }

}
