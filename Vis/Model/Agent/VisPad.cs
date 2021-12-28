using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Connections;
using Vis.Model.Controller;
using Vis.Model.Primitives;

namespace Vis.Model.Agent
{
	public enum PadGrid { Rectangle, Oval, Hexagon }
	public enum PadKind { View, Focus, Working, Bitmap }
    public enum PadStyle {None, Hidden, Disabled, Normal, Enhanced, Offset, Zoomed}

    public interface IPad
    {
	   PadGrid Grid { get; }
	   bool AutoIndex { get; set; }
	   int Index { get; }
	   int StartIndex { get; }
        int Width { get; }
        int Height { get; }
	   PadStyle PadStyle { get; set;  }
	   List<ElementRecord> Paths { get; }

	   int GetNormalizedIndex(int index);
    }

    public class VisPad : IPad
    {
	    private static int _padIndexCounter;
	    public int Index { get; }
	    public int StartIndex { get; }
        private Type _minimumElementType;

        public PadGrid Grid { get; }
	    public PadKind PadKind { get; }
	    public PadStyle PadStyle { get; set; } = PadStyle.Normal;
        public bool AutoIndex { get; set; }
        private int _indexCounter;

        public List<ElementRecord> Paths { get; } = new List<ElementRecord>();
        public int Width { get; }
        public int Height { get; }

        public VisPad(Type minimumElementType, int width, int height, PadKind padKind, bool autoIndex = true, PadGrid padGrid = PadGrid.Rectangle)
        {
	        _minimumElementType = minimumElementType;
            Index = _padIndexCounter++;
	        PadKind = padKind;
            Width = width;
            Height = height;
            AutoIndex = autoIndex;
            StartIndex = (int)PadKind * 10000;
            _indexCounter = StartIndex;
        }

        public int GetNormalizedIndex(int index)
        {
	        int result = index - StartIndex;
	        if (result < 0 || result >= Paths.Count)
	        {
		        result = -1;
	        }
	        return result;
        }

        public ElementRecord Add(IElement item)
        {
            var element = AutoIndex ? new ElementRecord(item, PadKind, _indexCounter++) : new ElementRecord(item, PadKind);
            Paths.Add(element);
            return element;
        }
        public void Remove(ElementRecord item)
        {
	        Paths.Remove(item);
        }
        public void Clear()
        {
	        _indexCounter = StartIndex;
	        Paths.Clear();
        }
        public void RecalculateAll()
        {
            foreach (var path in Paths)
            {
                if(path.Element is VisStroke stroke)
                {
                    stroke.Recalculate();
                }
            }
        }
        public ElementRecord GetPadAttributesFor(IPath item)
        {
	        ElementRecord result = null;
	        foreach (var padAttributes in Paths)
	        {
		        if (padAttributes.Element.GetHashCode() == item.GetHashCode())
		        {
			        result = padAttributes;
			        break;
		        }
	        }
	        return result;
        }

        //public VisElement GetByLocation(VisElement reference, VisLocator locator) => null;
        //   public VisElement GetNearby(Node node, VisElementType elementType = VisElementType.Any) => null;

        //public VisJoint GetSimilar(VisJoint joint) => null;

        public VisNode NodeNear(VisPoint query)
        {
	        VisNode result = null;
	        foreach (var item in Paths)
	        {
		        if (item.Element is IPath path)
		        {
			        foreach (VisPoint p in path)
			        {
				        var dist = p.SquaredDistanceTo(query);
				        if (dist < query.NearThreshold)
				        {
					        result = path.NodeNear(p);
					        break;
				        }
			        }
		        }
	        }
	        return result;
        }

        public VisNode ClosestNode(VisPoint query)
        {
	        VisNode result = null;
            foreach (var item in Paths)
	        {
		        if (item.Element is IPath path)
		        {
			        var node = path.ClosestAnchor(query);
			        if (node.Location.DistanceTo(query) < query.NearThreshold)
			        {
				        result = node;
				        break;
			        }
			        //foreach (var p in path)
			        //{
				       // var dist = p.SquaredDistanceTo(query);
				       // if (dist < query.NearThreshold)
				       // {
					      //  result = path.BestNodeForPoint(p);
					      //  break;
				       // }
			        //}
		        }
	        }
	        return result;
        }

        public IPrimitive GetSimilar(IPrimitive query)
        {
            var result = default(IPrimitive);
            if (query is VisPoint qp)
            {
                foreach (var item in Paths)
                {
                    float dist = float.MaxValue;
                    if (item.Element is VisPoint p)
                    {
                        dist = p.SquaredDistanceTo(qp);
                        if (dist < qp.NearThreshold)
                        {
                            result = p;
                            break;
                        }
                    }
                    else if (item.Element is IPath path)
                    {
	                    var pp = path.ClosestAnchor(qp);
                        dist = pp.Location.SquaredDistanceTo(qp);
                        if (dist < qp.NearThreshold)
                        {
                            result = pp;
                            break;
                        }
                    }
                }
            }
            return result;
        }
            
        public IPath GetSimilar(IPath query, params VisJoint[] joints)
        {
            var result = default(IPath);
            foreach (var item in Paths)
            {
                if (item.Element is IPath padPath)
                {
                    IPath path = (IPath)query;
                    var centerPoint = new VisPoint(0.5f, 0.5f);
                    var lineDir = path.StartPoint.LinearDirection(path.EndPoint);
                    var loc = path.MidPoint.DirectionFrom(centerPoint);
                    if (padPath.StartPoint.LinearDirection(padPath.EndPoint) == lineDir && padPath.MidPoint.DirectionFrom(centerPoint) == loc)
                    {
                        result = padPath;
                    }
                }
            }
            return result;
        }
    }
}
