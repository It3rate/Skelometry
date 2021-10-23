﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Primitives;

namespace Vis.Model.Agent
{
    public enum PadType { Rectangle, Oval, Hexagon }

    public class VisPad<T> //where T : IPrimitive
    {
        public PadType Type { get; }

        public List<T> Paths { get; } = new List<T>();
        public int Width { get; }
        public int Height { get; }

        public VisPad(int width, int height, PadType padType = PadType.Rectangle)
        {
            Width = width;
            Height = height;
        }
        public void Remove(T item)
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
		        if (item is IPath path)
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
		        if (item is IPath path)
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
                    if (item is VisPoint p)
                    {
                        dist = p.SquaredDistanceTo(qp);
                        if (dist < qp.NearThreshold)
                        {
                            result = p;
                            break;
                        }
                    }
                    else if (item is IPath path)
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
                if (item is IPath padPath)
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
            var result = new VisPad<T>(w, h, PadType.Rectangle);
            return result;
        }
    }
}
