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

        public bool IsPath => true;
        public bool IsFixed { get; set; } = false;

        public VisPoint Anchor => StartNode.Location;
        private float _length;
        public float Length() => _length;
        public VisPoint StartPoint => StartNode.Location;
        public VisPoint MidPoint => GetPoint(0.5f, 0);
        public VisPoint EndPoint => EndNode.Location;
        public VisPoint Center => MidPoint; // Will be the center of the bounds once that is calculated

        public VisNode StartNode => Nodes[0];
        public VisNode MidNode => new VisNode(this, 0.5f);
        public VisNode EndNode => Nodes[Nodes.Count - 1];

        private readonly List<VisPoint> GenPoints = new List<VisPoint>();
        public List<IPrimitivePath> Segments = new List<IPrimitivePath>();
        public IPath UnitReference { get; set; }

        public int AnchorCount => Segments.Sum(seg => seg.AnchorCount);
        public VisNode ClosestAnchor(float shift)
        {
	        var subNode = SubNodeAt(shift);
	        return subNode.Reference.ClosestAnchor(subNode.Shift);
        }
        public VisNode ClosestAnchor(VisPoint point)
        {
	        VisPoint result = null;
	        VisNode resultNode = null;
            var shortest = float.MaxValue;
	        foreach (var node in Nodes)
	        {
		        var closest = node.Location;
		        var dist = Math.Abs(point.SquaredDistanceTo(closest));
		        if(dist < shortest)
		        {
			        shortest = dist;
			        result = closest;
			        resultNode = node;

		        }
	        }
	        return resultNode;
        }

        public VisStroke(VisNode first, VisNode second, params VisNode[] remaining)
        {
            Nodes.Add(first);
            Nodes.Add(second);
            Nodes.AddRange(remaining);
            GenerateSegments();
        }

        public virtual void AddOffset(float x, float y)
        {
            List<IPath> refs = new List<IPath>();
	        foreach (var node in Nodes)
	        {
		        if (!refs.Contains(node.Reference))
		        {
			        if (!node.Reference.IsFixed)
			        {
				        node.Reference.AddOffset(x, y);
						refs.Add(node.Reference);
			        }
			        else
			        {
                        node.AddOffset(x, y);
			        }
		        }
	        }
        }
        public VisPolyline GetPolyline()
        {
            List<VisPoint> pts = new List<VisPoint>();
            foreach (var pt in GenPoints)
            {
	            pts.Add(pt);
            }
	        return new VisPolyline(pts);
        }

        public OffsetNode NodeFor(VisPoint pt)
        {
	        throw new NotImplementedException();
        }
        public VisNode BestNodeForPoint(VisPoint pt)
        {
	        VisNode result = null;
            IPath seg = null;
	        if (Segments.Count > 1 && Segments[1] is VisArc arc)
	        {
		        seg = arc;
	        }
	        else if (Segments[0] is VisLine line)
	        {
		        seg = line;
	        }

	        if (seg != null)
	        {
		        var nearest = ProjectPointOnto(pt);
		        var ratio = (pt.X - StartPoint.X) / (EndPoint.X - StartPoint.X);
		        return new VisNode(this, ratio);
            }
	        return result;
        }

        public VisPoint ProjectPointOnto(VisPoint p)
        {
	        var result = p;
	        if (Segments.Count > 1 && Segments[1] is VisArc arc)
	        {
		        result = arc.ProjectPointOnto(p);
	        }
            else if (Segments[0] is VisLine line)
	        {
		        result = line.ProjectPointOnto(p);
	        }
	        return result;
        }

        private void GenerateSegments()
        {
            Segments.Clear();
            GenPoints.Clear();
            VisPoint curPoint = Nodes[0].Start;
            GenPoints.Add(Nodes[0].Start);

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
                            GenPoints.AddRange(arc.GetPolylinePoints());
                            curPoint = p1;
                            break;
                        }
                    case OffsetNode tipNode:
	                    if (i > 0)
	                    {
		                    GenPoints.Add(curNode.Start);
		                    Segments.Add(VisLine.ByEndpoints(curPoint, curNode.End));
		                    curPoint = curNode.End;
	                    }

	                    break;
                    default:
                        {
                            if (i > 0)
                            {
                                GenPoints.Add(curNode.Start);
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
                _length += segment.Length();
            }
        }

        public VisPoint GetPoint(float shift, float offset = 0)
        {
            var pos = Length() * shift;
            var len = 0f;
            var targetSegment = Segments[0];
            foreach (var segment in Segments)
            {
                var segLen = segment.Length();
                if (len + segLen > pos)
                {
                    targetSegment = segment;
                    break;
                }
                else
                {
                    len += segment.Length();
                }
            }
            var targetPosition = (pos - len) / targetSegment.Length();
            return targetSegment.GetPoint(targetPosition, offset);
        }

        public VisPoint GetPointFromCenter(float centeredShift, float offset = 0)
        {
            return GetPoint(centeredShift * 2f - 1f, offset);
        }

        public void AddNodes(params VisNode[] nodes)
        {
            Nodes.AddRange(nodes);
            GenerateSegments();
        }
        public void Recalculate()
        {
            GenerateSegments();
        }

        public void Flip() { }
        public VisStroke OrientedClone() => null;

        public float CompareTo(IPath element) => 0;

        public bool IntersectsWith(VisStroke stroke) => false;
        public float DistanceTo(VisStroke stroke, out float shift, out float targetShift)
        {
            shift = 0;
            targetShift = 0;
            return 0;
        }

        public float LikelyVertical { get; }
        public float LikelyHorizontal { get; }
        public float LikelyDiagonalUp { get; }
        public float LikelyDiagonalDown { get; }

        public VisNode SubNodeAt(float shift)
        {
	        float targLen = Length() * shift;
	        float len = 0;
	        int index = 0;
	        float subShift = 0;
            foreach (var segment in Segments)
	        {
		        float refLen = segment.Length();
		        if (len + refLen >= targLen)
		        {
			        subShift = (targLen - len) / refLen;
			        break;
		        }
		        else
		        {
					index++;
			        len += refLen;
		        }
	        }
            return new VisNode(Nodes[index].Reference, subShift);
        }
        public VisNode CreateNodeAt(float shift) => new VisNode(this, shift);
        public VisNode CreateNodeAt(float shift, float offset) => new OffsetNode(this, shift, offset);
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
