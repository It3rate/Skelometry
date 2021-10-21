using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vis.Model
{
    public class Stroke : IPath
    {
        private List<Node> Nodes { get; } = new List<Node>();

        public Point Anchor => StartNode.Anchor;
        private float _length;
        public float Length => _length;
        public Point StartPoint => StartNode.Anchor;
        public Point MidPoint => GetPoint(0.5f, 0);
        public Point EndPoint => EndNode.Anchor;
        public Point Center => MidPoint; // Will be the center of the bounds once that is calculated

        private List<Point> Anchors = new List<Point>();
        public List<IPrimitivePath> Segments = new List<IPrimitivePath>();


        public Stroke(Node first, Node second, params Node[] remaining)
	    {
		    Nodes.Add(first);
		    Nodes.Add(second);
		    Nodes.AddRange(remaining);
		    GenerateSegments();
	    }

        private void GenerateSegments()
        {
	        Segments.Clear();
            Anchors.Clear();
            Point curPoint = Nodes[0].Start;
            Anchors.Add(Nodes[0].Start);

            for (var i = 0; i < Nodes.Count; i++)
            {
	            var curNode = Nodes[i];
	            switch (curNode)
	            {
		            case TangentNode tanNode:
		            {
			            var pn = i > 0 ? Nodes[i - 1] : null;
			            var p0 = tanNode.GetTangentFromPoint(pn);
			            var nn = i < Nodes.Count - 1 ? Nodes[i + 1] : null;
			            var p1 = tanNode.GetTangentToPoint(nn);
			            var arc = new Arc(tanNode.CircleRef, p0, p1, tanNode.Direction);
			            Segments.Add(Line.ByEndpoints(curPoint, p0));
			            Segments.Add(arc);
			            Anchors.AddRange(arc.GetPolylinePoints());
			            curPoint = p1;
			            break;
		            }
		            case TipNode tipNode:
			            Anchors.Add(curNode.Start);
			            Segments.Add(Line.ByEndpoints(curPoint, curNode.End));
			            curPoint = curNode.End;
			            break;
		            default:
		            {
			            if (i > 0)
			            {
				            Anchors.Add(curNode.Start);
				            Segments.Add(Line.ByEndpoints(curPoint, curNode.Start));
				            curPoint = curNode.End;
			            }

			            break;
		            }
	            }
            }

            _length = 0;
            foreach (var segment in Segments)
            {
	            _length += segment.Length;
            }
        }

	    public Point GetPoint(float position, float offset = 0)
	    {
		    var pos = Length * position;
		    var len = 0f;
		    var targetSegment = Segments[0];
		    foreach (var segment in Segments)
		    {
			    var segLen = segment.Length;
			    if (len + segLen > pos)
			    {
				    targetSegment = segment;
				    break;
			    }
			    else
			    {
					len += segment.Length;
			    }
		    }
		    var targetPosition = (pos - len) / targetSegment.Length;
		    return targetSegment.GetPoint(targetPosition, offset);
	    }

	    public Point GetPointFromCenter(float centeredPosition, float offset = 0)
	    {
		    return GetPoint(centeredPosition * 2f - 1f, offset);
	    }

        public void AddNodes(params Node[] nodes)
        {
	        Nodes.AddRange(nodes);
            GenerateSegments();
        }

        public void Flip(){ }
	    public Stroke OrientedClone() => null;

	    public float CompareTo(IPath element) => 0;

	    public bool IntersectsWith(Stroke stroke) => false;
	    public float DistanceTo(Stroke stroke, out float position, out float targetPosition)
	    {
		    position = 0;
		    targetPosition = 0;
		    return 0;
	    }

	    public float LikelyVertical { get; }
	    public float LikelyHorizontal { get; }
	    public float LikelyDiagonalUp { get; }
	    public float LikelyDiagonalDown { get; }


	    public Node NodeAt(float position) => new Node(this, position);
	    public Node NodeAt(float position, float offset) => new TipNode(this, position, offset);
	    public Node StartNode => Nodes[0];
	    public Node MidNode => new Node(this, 0.5f);
	    public Node EndNode => Nodes[Nodes.Count - 1];
    }
}
