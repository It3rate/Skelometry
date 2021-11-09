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
    public enum PadType { Rectangle, Oval, Hexagon }

    public interface IPad
    {
	   PadType Type { get; }
	   bool AutoIndex { get; set; }
	   int Width { get; }
	   int Height { get; }
    }

    public class VisPad<T> : IPad //where T : IPrimitive
    {
	    private static int _padIndexCounter;
	    public int Index;
	    public PadType Type { get; }
	    public bool AutoIndex { get; set; }

        public List<PadAttributes<T>> Paths { get; } = new List<PadAttributes<T>>();
        public int Width { get; }
        public int Height { get; }

        public VisPad(int width, int height, bool autoIndex = true, PadType padType = PadType.Rectangle)
        {
	        Index = _padIndexCounter++;
            Width = width;
            Height = height;
            AutoIndex = autoIndex;
        }

        private int _indexCounter = 1;
        public PadAttributes<T> Add(T item)
        {
            var element = AutoIndex ? new PadAttributes<T>(item, _indexCounter++) : new PadAttributes<T>(item);
            Paths.Add(element);
            return element;
        }
        public void Remove(PadAttributes<T> item)
        {
	        Paths.Remove(item);
        }
        public void Clear()
        {
	        Paths.Clear();
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
        public IPath PathWithNodeNear(VisPoint query)
        {
	        IPath result = null;
	        foreach (var item in Paths)
	        {
		        if (item.Element is IPath path)
		        {
			        foreach (VisPoint p in path)
			        {
				        var dist = p.SquaredDistanceTo(query);
				        if (dist < query.NearThreshold)
				        {
					        result = path;
					        break;
				        }
			        }
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
                        foreach (VisPoint pp in path)
                        {
                            dist = pp.SquaredDistanceTo(qp);
                            if (dist < qp.NearThreshold)
                            {
                                result = pp;
                                break;
                            }
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


        public static VisPad<T> CreateRectPad(int w, int h, Type T)
        {
            var result = new VisPad<T>(w, h, true, PadType.Rectangle);
            return result;
        }
    }
}
