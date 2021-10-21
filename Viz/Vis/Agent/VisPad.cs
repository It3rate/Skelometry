using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
	public enum PadType { Rectangle, Oval, Hexagon }

    public class VisPad<T> //where T : IPath
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
        public void Clear()
        {
            Paths.Clear();
        }

	    //public VisElement GetByLocation(VisElement reference, VisLocator locator) => null;
     //   public VisElement GetNearby(Node node, VisElementType elementType = VisElementType.Any) => null;

        public VisJoint GetSimilar(VisJoint joint) => null;
        public T GetSimilar(IPath path, params VisJoint[] joints)
        {
	        var result = default(T);
	        var centerPoint = new Point(0.5f, 0.5f);
	        var lineDir = path.StartPoint.LinearDirection(path.EndPoint);
	        var loc = path.MidPoint.DirectionFrom(centerPoint);
            foreach (var item in Paths)
	        {
                if(item is IPath padPath)
                {
			        if (padPath.StartPoint.LinearDirection(padPath.EndPoint) == lineDir && padPath.MidPoint.DirectionFrom(centerPoint) == loc)
			        {
				        result = item;
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
