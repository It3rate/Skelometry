using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Vis.Model.Connections;

namespace Vis.Model.Primitives
{
    /// <summary>
    /// Maybe primitives are always 0-1 (lengths are always positive) and joints/nodes are -1 to 1 (we balance by midpoints of objects)?
    /// Or because lines have a start and endPoint (no volume) they are 0-1, where rects and circles are a point mass that is centered (no start and endPoint).
    /// How does inside/outside map to start/endPoint? (center0, edge1, outside>1)
    /// We only use rects and circles to express containment boundaries so they are 0 centered, the corner (or edge) of a rect isn't a volume so it has a start (and endPoint).
    /// 0 is past (known duration), 1 is present, > 1 is future (unknown potentially infinite duration)
    /// </summary>
    public class VisLine : VisPoint, IPath, IPrimitivePath
    {
	    public VisPoint StartPoint => this;
	    public VisPoint MidPoint => GetPoint(0.5f, 0);
        public VisPoint EndPoint { get; private set; }

        public VisPoint Center => GetPoint(0.5f, 0);

        public IPath UnitReference { get; set; }

        private float _length;
        public float Length
        {
            get
            {
                if (_length == 0)
                {
	                var dist = (EndPoint.X - X) * (EndPoint.X - X) + (EndPoint.Y - Y) * (EndPoint.Y - Y);
                    _length = dist > 0.0001 ? (float)Math.Sqrt(dist) : 0;
                }
                return _length;
            }
        }

        private VisLine(float startX, float startY, float endX, float endY) : base(startX, startY)
        {
            EndPoint = new VisPoint(endX, endY);
        }
        private VisLine(VisPoint start, VisPoint endPoint) : this(start.X, start.Y, endPoint.X, endPoint.Y) { }
        private VisLine(VisLine line) : this(line.StartPoint, line.EndPoint) { }

        public static VisLine ByCenter(float centerX, float centerY, float originX, float originY)
        {
            return new VisLine(centerX, centerY, originX, originY);
        }
        public static VisLine ByEndpoints(VisPoint start, VisPoint end)
        {
            return new VisLine(start, end);
        }
        public static VisLine ByEndpoints(float startX, float startY, float endX, float endY)
        {
            var start = new VisPoint(startX, startY);
            var end = new VisPoint(endX, endY);
            return new VisLine(start, end);
        }

        public VisPoint GetPoint(float position, float offset = 0)
        {
	        var xOffset = 0f;
	        var yOffset = 0f;
	        var xDif = EndPoint.X - X;
	        var yDif = EndPoint.Y - Y;
	        if (offset != 0)
	        {
		        var ang = (float)(Math.Atan2(yDif, xDif));
		        xOffset = (float)(-Math.Sin(ang) * Math.Abs(offset) * Math.Sign(-offset));
		        yOffset = (float)(Math.Cos(ang) * Math.Abs(offset) * Math.Sign(-offset));
	        }
	        return new VisPoint(X + xDif * position + xOffset, Y + yDif * position - yOffset);
        }

        public VisPoint GetPointFromCenter(float centeredPosition, float offset = 0)
        {
            return GetPoint(centeredPosition * 2f - 1f, offset);
        }

        public VisNode NodeAt(float position) => new VisNode(this, position);
        public VisNode NodeAt(float position, float offset) => new TipNode(this, position, offset);
        public VisNode StartNode => new VisNode(this, 0f);
        public VisNode MidNode => new VisNode(this, 0.5f);
        public VisNode EndNode => new VisNode(this, 1f);


        
        public virtual bool IsInBoundingBox(double x, double y)
        {
	        return (x >= X && x <= EndPoint.X || x >= EndPoint.X && x <= X) && 
	               (y >= Y && y <= EndPoint.Y || y >= EndPoint.Y && y <= Y);
        }
        public virtual VisRectangle BoundingBox() => new VisRectangle(this, EndPoint);
        public VisRectangle RectangleFrom() => new VisRectangle(this, EndPoint);
        public VisCircle CircleFrom() => new VisCircle(this, EndPoint);

        public (float, float, float) ABCLine()
        {
	        return StartPoint.ABCLine(EndPoint);
        }
        public float Determinant(VisLine line) => (EndPoint.Y - Y) * (line.X - line.EndPoint.X) - (X - EndPoint.X) * (line.EndPoint.Y - line.Y);
        public VisPoint IntersectionPoint(VisLine line)
        {
	        VisPoint result = null;
	        var (a0, b0, c0) = ABCLine();
	        var (a1, b1, c1) = line.ABCLine();
            var determinant = a0 * b0 - a1 * b0;
            if (determinant != 0) // not parallel
            {
	            var x = (b1 * c0 - b0 * c1) / determinant;
	            var y = (a0 * c1 - a1 * c0) / determinant;
	            result = new VisPoint(x, y);
            }
            return result;
        }
        public VisPoint ProjectPointOnto(VisPoint p)
        {
	        var e1 = EndPoint.Subtract(this);
	        var e2 = p.Subtract(this);
	        var dp = e1.DotProduct(e2);
	        var len2 = e1.VectorSquaredLength();
	        return new VisPoint(X + (dp * e1.X) / len2, Y + (dp * e1.Y) / len2);
         //   var e1 = EndPoint.Subtract(StartPoint);
	        //var e2 = p.Subtract(StartPoint);
	        //var dp = e1.DotProduct(e2);
	        //var len2 = e1.VectorSquaredLength();
	        //return new Point(StartPoint.X + (dp * e1.X) / len2, StartPoint.Y + (dp * e1.Y) / len2);
        }

        public VisPoint[] GetPolylinePoints(int pointCount = 24)
        {
	        var result = new List<VisPoint>() {StartPoint, EndPoint};
	        return result.ToArray();
        }

        public VisNode NodeNear(VisPoint point)
        {
	        VisNode result = null;
	        if (StartPoint.SquaredDistanceTo(point) < point.NearThreshold)
	        {
		        result = StartNode;
	        }
	        if (EndPoint.SquaredDistanceTo(point) < point.NearThreshold)
	        {
		        result = EndNode;
	        }
	        return result;
        }

        public IEnumerator<VisPoint> GetEnumerator()
        {
            yield return StartPoint;
            yield return EndPoint;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return StartPoint;
            yield return EndPoint;
        }

        public VisLine CloneLine()
        {
	        return new VisLine(this);
        }
        public override object Clone()
        {
	        return new VisLine(this);
        }

        public override bool Equals(IPrimitive other)
        {
            bool result = false;
            if (other != null && other is VisLine line)
            {
                result = (StartPoint == line.StartPoint) && (EndPoint == line.EndPoint);
            }
            return result;
        }
        public static bool operator ==(VisLine lhs, VisLine rhs)
        {
            bool result = false;
            if (lhs is null)
            {
                if (rhs is null)
                {
                    result = true;
                }
            }
            else
            {
                result = lhs.Equals(rhs);
            }
            return result;
        }
        public static bool operator !=(VisLine lhs, VisLine rhs) => !(lhs == rhs);
        public override int GetHashCode() => (StartPoint, EndPoint).GetHashCode();
        public override string ToString()
        {
	        return String.Format("Ln:{0:0.##},{1:0.##} {2:0.##},{3:0.##}", X, Y, EndPoint.X, EndPoint.Y);
        }

    }
}
