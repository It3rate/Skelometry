using System;
using System.Windows.Forms.VisualStyles;
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
	    public static readonly VisNode Empty = new VisNode(null, -1);

        public IPath Reference { get; } // if reference is a stroke, this must be a joint
        public float Shift { get; protected set; } // 0f is beginning, 1f is end

        // calculated
        private VisPoint _noRefLocation;
        public virtual VisPoint Location => Reference != null ? GetPoint(Shift) : _noRefLocation;
        public float X => Location.X;
        public float Y => Location.Y;

        public virtual VisPoint Start => Location;
        public virtual VisPoint End => Location;

        public bool IsPath => true;
        public bool IsEmpty => Object.ReferenceEquals(this, Empty);

        public VisNode(IPath reference, float shift)
        {
            Reference = reference;
            Shift = shift;
            _noRefLocation = new VisPoint(-1,-1);
        }
        public VisNode(VisPoint pt)
        {
	        _noRefLocation = pt;
        }

        public virtual VisPoint GetPoint(float shift, float offset = 0)
        {
	        return Reference.GetPoint(shift, offset);
        }
        public virtual VisNode ClosestAnchor()
        {
	        return Reference.ClosestAnchor(Shift);
        }

        public virtual void AddOffset(float x, float y)
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

        public float Similarity(IPrimitive p) => 0;
        public VisPoint Sample(Gaussian g) => null;

        public VisNode CloneNode()
        {
	        return new VisNode(Reference, Shift);
        }
        public virtual object Clone()
        {
	        return new VisNode(Reference, Shift);
        }

        //public override bool Equals(object obj) => this.Equals(obj as IPrimitive);
        public virtual bool Equals(IPrimitive other)
        {
            return Object.ReferenceEquals(this, other);
        }
        //public override int GetHashCode() => this.GetHashCode();
        public override string ToString()
        {
            return "Node:" + Location.ToString();
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
            // if node is null, get point on circumference using shift. 
            // if it is another Tangent node use circleRef tangents
            _start = CircleRef.FindTangentInDirection(node.End, Direction);
            return _start;
        }
        public VisPoint GetTangentToPoint(VisNode node)
        {
            // if p is null, get point on circumference using shift.
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

}
