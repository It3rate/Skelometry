using System;
using Microsoft.ML.Probabilistic.Distributions;
using Vis.Model.Primitives;

namespace Vis.Model.Connections
{
    /// <summary>
    /// Nodes specify landmarks where things relate to each other in various ways.
    /// It is more abstract than a joint, in that it can apply to any type of reference, and
    /// can specify positions of unrelated things, like two lines that happen to overlap.
    /// They are the 'mental' version of joints, and do not need to map the solved reality of a scene.
    /// (they are read to the joints write)
    /// </summary>
    public class VisNode : IPrimitive
    {
        public IPath Reference { get; } // if reference is a stroke, this must be a joint
        public float Position { get; }

        public VisNode PreviousNode { get; private set; }
        public VisNode NextNode { get; private set; }

        // calculated
        public VisPoint Anchor => Reference.GetPoint(Position); //; protected set; }
        public float X => Anchor.X;
        public float Y => Anchor.Y;

        public virtual VisPoint Start => Anchor;
        public virtual VisPoint End => Anchor;

        public bool IsPath => true;

        public VisNode(IPath reference, float position)
        {
            Reference = reference;
            Position = position;
            //Anchor = reference.GetPoint(position);
        }


        public void AddOffset(float x, float y)
        {
	        if (Reference is IPrimitive primitive)
	        {
                primitive.AddOffset(x, y);
            }
            else if (Reference is VisNode node)
	        {
                node.AddOffset(x, y);
	        }
        }

        public VisPolyline GetPolyline()
        {
	        throw new NotImplementedException();
        }

        public VisPoint ProjectPointOnto(VisPoint p)
        {
	        throw new NotImplementedException();
        }

        public VisPoint GetPoint(float position, float offset = 0)
        {
            return Reference.GetPoint(position, offset);
        }

        public float Similarity(IPrimitive p) => 0;
        public VisPoint Sample(Gaussian g) => null;

        public VisNode CloneNode()
        {
	        return new VisNode(Reference, Position);
        }
        public virtual object Clone()
        {
	        return new VisNode(Reference, Position);
        }

        //public override bool Equals(object obj) => this.Equals(obj as IPrimitive);
        public virtual bool Equals(IPrimitive other)
        {
            return Object.ReferenceEquals(this, other);
        }
        //public override int GetHashCode() => this.GetHashCode();
        public override string ToString()
        {
            return "Node:" + Anchor.ToString();
        }
    }

    /// <summary>
    /// Node that joins the reference path perpendicularly at the nearest point. Mostly for circles. If inside, connects to closest edge. If center connects to origin.
    /// Line: if it isn't valid for a line segment, it will connect to the imaginary extended line?
    /// Arc: Connects to imaginary edge if arc isn't complete. If inside, connects to nearest interior point on arc, if invalid behaves as circle.
    /// </summary>
	public class PerpendicularNode : VisNode
    {
        public PerpendicularNode(IPath reference) : base(reference, 0)
        {
        }
    }

    // The first node on a circleRef needs to specify it's direction as there are two tangent lines. Points inside the circleRef will move to intersecting point of the second node's reference based on direction.
    public class TangentNode : VisNode
    {
        public ClockDirection Direction { get; }
        public VisCircle CircleRef;
        private VisPoint _start;
        public override VisPoint Start => _start;
        private VisPoint _end;
        public override VisPoint End => _end;

        public TangentNode(VisCircle circleRef, ClockDirection direction = ClockDirection.CW) : base(circleRef, 0)
        {
            CircleRef = circleRef;
            Direction = direction;
        }

        public VisPoint GetTangentFromPoint(VisNode node)
        {
            // if node is null, get point on circumference using position. 
            // if it is another Tangent node use circleRef tangents
            _start = CircleRef.FindTangentInDirection(node.End, Direction);
            return _start;
        }
        public VisPoint GetTangentToPoint(VisNode node)
        {
            // if p is null, get point on circumference using position.
            // if it is another Tangent node use circleRef tangents
            _end = CircleRef.FindTangentInDirection(node.Start, Direction.Counter());
            return _end;
        }
        public TangentNode CloneTangentNode()
        {
	        return new TangentNode(CircleRef, Direction);
        }
        public override object Clone()
        {
	        return new TangentNode(CircleRef, Direction);
        }
        public override string ToString()
        {
            return string.Format("tanNode:{0:0.##},{1:0.##} e{2:0.##},{3:0.##}", Start.X, Start.Y, End.X, End.Y);
        }
    }

    public class TipNode : VisNode
    {
        // Offset can't be zero in a middle node, as it causes overlap on lines that are tangent to each other. 
        // The corner of a P is part of the shape with potential overlap on the serif.
        // Maybe X could be a V with overlap.H would be a half U with 0.5 overlap. Maybe this is too obfuscated. Yes it is. Might work for serifs though.
        public float Offset { get; }

        public TipNode(IPath reference, float position, float offset) : base(reference, position)
        {
            Offset = offset;
            //Anchor = reference.GetPoint(position, offset);
        }
        //public TipNode(IPath reference, float position, float offset, float length) : base(reference, position)
        //{
        //	Offset = offset;
        //	Length = length;
        //}

        public TipNode CloneTipNode()
        {
	        return new TipNode(Reference, Position, Offset);
        }
        public override object Clone()
        {
	        return new TipNode(Reference, Position, Offset);
        }
        public override string ToString()
        {
            return string.Format("tipNode:{0:0.##},{1:0.##} o{2:0.##}", Anchor.X, Anchor.Y, Offset);
        }
    }

}
