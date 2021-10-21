using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Distributions;


namespace Vis.Model
{
	//public enum VisElementType { Any, Point, Node, Circle, Square, Rectangle, Oval, Joint, Stroke, Shape }

    /// <summary>
    /// The mental map primitives when we conceptualize things at a high level. These are meant to be (ideally) what we use, not what is mathematically possible or even simple.
    /// </summary>
    public interface IPrimitive
    {
	    float X { get; }
	    float Y { get; }
        float Similarity(IPrimitive p);
        Point Sample(Gaussian g);
    }
    public interface IPrimitivePath : IPath
    {
	    Point[] GetPolylinePoints(int pointCount = 24);
    }

    public interface IPath
    {
	    float Length { get; }
	    Point StartPoint { get; }
	    Point MidPoint { get; }
	    Point EndPoint { get; }

        Point GetPoint(float position, float offset = 0);
        Point GetPointFromCenter(float centeredPosition, float offset = 0);

        Node NodeAt(float position, float offset = 0);
        Node StartNode { get; }
        Node MidNode { get; }
        Node EndNode { get; }
    }

    public interface IArea
    {
	    Point Center { get; }
	    float Area { get; }
	    Rectangle Bounds { get; }
        bool IsClosed { get; }
	    bool IsConcave { get; }
	    int JointCount { get; }
	    int CornerCount { get; }
	    float Sharpness { get; }
    }

    public class Point : IPrimitive
    {
	    public float X { get; }
	    public float Y { get; }

	    protected const float pi2 = (float)(Math.PI * 2.0);

	    public Point(float x, float y)
	    {
		    X = x;
		    Y = y;
	    }

	    public Point(Point p)
	    {
		    X = p.X;
		    Y = p.Y;
	    }

	    public Point Sample(Gaussian g) => new Point(X + (float) g.Sample(), Y + (float) g.Sample());
	    public virtual float Similarity(IPrimitive p) => 0;
	    public Point Transpose() => new Point(Y, X);
	    public Point Abs() => new Point(Math.Abs(X), Math.Abs(Y));
	    public Point Add(Point pt) => new Point(X + pt.X, Y + pt.Y);
	    public Point Subtract(Point pt) => new Point(X - pt.X, Y - pt.Y);
	    public Point Multiply(Point pt) => new Point(X * pt.X, Y * pt.Y);
	    public Point MidPointOf(Point pt) => new Point((pt.X - X) / 2f + X, (pt.Y - Y) / 2f + Y);
	    public Point Multiply(float scalar) => new Point(X * scalar, Y * scalar);
	    public Point DivideBy(float scalar) => new Point(X / scalar, Y / scalar);

	    public float VectorLength() => (float)Math.Sqrt(X * X + Y * Y);
	    public float VectorSquaredLength() => X * X + Y * Y;
	    public float DistanceTo(Point pt) => (float)Math.Sqrt((pt.X - X) * (pt.X - X) + (pt.Y - Y) * (pt.Y - Y));
	    public float SquaredDistanceTo(Point pt) => (pt.X - X) * (pt.X - X)  + (pt.Y - Y) * (pt.Y - Y);
	    public float DotProduct(Point pt) => (X * pt.X) + (Y * pt.Y); // negative because inverted Y
	    public float Atan2(Point pt) => (float)Math.Atan2(pt.Y - Y, pt.X - X);

        public PointF PointF => new PointF(X, Y);

        public LinearDirection LinearDirection(Point pt)
	    {
		    // make this return probability as well
		    LinearDirection result;
		    var dir = Math.Atan2(pt.Y - Y, pt.X - X);
		    var pi8 = Math.PI / 8f;
		    if (dir < -(pi8 * 7))
		    {
			    result = Vis.Model.LinearDirection.Horizontal;
		    }
		    else if (dir < -(pi8 * 5))
		    {
			    result = Vis.Model.LinearDirection.TRDiagonal;
		    }
		    else if (dir < -(pi8 * 3))
		    {
			    result = Vis.Model.LinearDirection.Vertical;
		    }
		    else if (dir < -(pi8 * 1))
		    {
			    result = Vis.Model.LinearDirection.TLDiagonal;
		    }
		    else if (dir < (pi8 * 1))
		    {
			    result = Vis.Model.LinearDirection.Horizontal;
		    }
		    else if (dir < (pi8 * 3))
		    {
			    result = Vis.Model.LinearDirection.TRDiagonal;
		    }
		    else if (dir < (pi8 * 5))
		    {
			    result = Vis.Model.LinearDirection.Vertical;
		    }
		    else if (dir < (pi8 * 7))
		    {
			    result = Vis.Model.LinearDirection.TLDiagonal;
		    }
		    else
		    {
			    result = Vis.Model.LinearDirection.Horizontal;
		    }

		    return result;
	    }

	    public CompassDirection DirectionFrom(Point pt)
	    {
            // make this return probability as well
            CompassDirection result;
		    var dir = Math.Atan2(Y - pt.Y, X - pt.X);
		    var pi8 = Math.PI / 8f;
		    if (dir < -(pi8 * 7))
		    {
			    result = CompassDirection.W;
		    }
		    else if (dir < -(pi8 * 5))
		    {
			    result =CompassDirection.SW;
		    }
		    else if (dir < -(pi8 * 3))
		    {
			    result =CompassDirection.S;
		    }
		    else if (dir < -(pi8 * 1))
		    {
			    result =CompassDirection.SE;
		    }
		    else if (dir < (pi8 * 1))
		    {
			    result =CompassDirection.E;
		    }
		    else if (dir < (pi8 * 3))
		    {
			    result =CompassDirection.NE;
		    }
		    else if (dir < (pi8 * 5))
		    {
			    result =CompassDirection.N;
		    }
		    else if (dir < (pi8 * 7))
		    {
			    result =CompassDirection.NW;
		    }
		    else
		    {
			    result =CompassDirection.W;
		    }

		    return result;
        }
	    public Point ProjectedOntoLine(Line line)
	    {
		    return line.ProjectPointOnto(this);
	    }

	    public override string ToString()
	    {
		    return String.Format("Pt:{0:0.##},{1:0.##}", X, Y);
	    }
    }

    public enum LinearDirection{Centered, Horizontal, Vertical, TLDiagonal, TRDiagonal }

}
