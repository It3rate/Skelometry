using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Connections;

namespace Vis.Model.Primitives
{
    public class VisStroke : IPath
    {
        public List<VisNode> Nodes { get; } = new List<VisNode>();

        public VisPoint Anchor => StartNode.Anchor;
        private float _length;
        public float Length => _length;
        public VisPoint StartPoint => StartNode.Anchor;
        public VisPoint MidPoint => GetPoint(0.5f, 0);
        public VisPoint EndPoint => EndNode.Anchor;
        public VisPoint Center => MidPoint; // Will be the center of the bounds once that is calculated

        public VisNode StartNode => Nodes[0];
        public VisNode MidNode => new VisNode(this, 0.5f);
        public VisNode EndNode => Nodes[Nodes.Count - 1];

        private List<VisPoint> Anchors = new List<VisPoint>();
        public List<IPrimitivePath> Segments = new List<IPrimitivePath>();
        public IPath UnitReference { get; set; }


        public VisStroke(VisNode first, VisNode second, params VisNode[] remaining)
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
            VisPoint curPoint = Nodes[0].Start;
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
                            var arc = new VisArc(tanNode.CircleRef, p0, p1, tanNode.Direction);
                            Segments.Add(VisLine.ByEndpoints(curPoint, p0));
                            Segments.Add(arc);
                            Anchors.AddRange(arc.GetPolylinePoints());
                            curPoint = p1;
                            break;
                        }
                    case TipNode tipNode:
                        Anchors.Add(curNode.Start);
                        Segments.Add(VisLine.ByEndpoints(curPoint, curNode.End));
                        curPoint = curNode.End;
                        break;
                    default:
                        {
                            if (i > 0)
                            {
                                Anchors.Add(curNode.Start);
                                Segments.Add(VisLine.ByEndpoints(curPoint, curNode.Start));
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

        public VisPoint GetPoint(float position, float offset = 0)
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

        public VisPoint GetPointFromCenter(float centeredPosition, float offset = 0)
        {
            return GetPoint(centeredPosition * 2f - 1f, offset);
        }

        public void AddNodes(params VisNode[] nodes)
        {
            Nodes.AddRange(nodes);
            GenerateSegments();
        }

        public void Flip() { }
        public VisStroke OrientedClone() => null;

        public float CompareTo(IPath element) => 0;

        public bool IntersectsWith(VisStroke stroke) => false;
        public float DistanceTo(VisStroke stroke, out float position, out float targetPosition)
        {
            position = 0;
            targetPosition = 0;
            return 0;
        }

        public float LikelyVertical { get; }
        public float LikelyHorizontal { get; }
        public float LikelyDiagonalUp { get; }
        public float LikelyDiagonalDown { get; }

        public VisNode NodeAt(float position) => new VisNode(this, position);
        public VisNode NodeAt(float position, float offset) => new TipNode(this, position, offset);
        public VisNode NodeNear(VisPoint point)
        {
	        VisNode result = null;
	        foreach (var node in Nodes)
	        {
		        if (node.GetPoint(0).SquaredDistanceTo(point) < point.NearThreshold)
		        {
			        result = node;
			        break;
		        }
                else if (node.GetPoint(1).SquaredDistanceTo(point) < point.NearThreshold)
		        {
			        result = node;
			        break;
		        }
            }
	        return result;
        }

        public IEnumerator<VisPoint> GetEnumerator()
        {
            foreach(var node in Nodes)
            {
                yield return node.GetPoint(0);
                yield return node.GetPoint(1); // need to get all relevant points
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var node in Nodes)
            {
                yield return node.GetPoint(0);
                yield return node.GetPoint(1);
            }
        }
        public override string ToString()
        {
	        return String.Format("Stroke:{0},{1:0.##}, {2:0.##},{3:0.##},{4:0.##}", Nodes.Count, StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);
        }
    }
}
