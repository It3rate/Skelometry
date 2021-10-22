using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Distributions;

namespace Vis.Model.Primitives
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
        VisPoint Sample(Gaussian g);
    }
    public interface IPrimitivePath : IPath
    {
        VisPoint[] GetPolylinePoints(int pointCount = 24);
    }

    public interface IPath
    {
        float Length { get; }
        VisPoint StartPoint { get; }
        VisPoint MidPoint { get; }
        VisPoint EndPoint { get; }

        VisPoint GetPoint(float position, float offset = 0);
        VisPoint GetPointFromCenter(float centeredPosition, float offset = 0);

        VisNode NodeAt(float position, float offset = 0);
        VisNode StartNode { get; }
        VisNode MidNode { get; }
        VisNode EndNode { get; }
    }

    public interface IArea
    {
        VisPoint Center { get; }
        float Area { get; }
        VisRectangle Bounds { get; }
        bool IsClosed { get; }
        bool IsConcave { get; }
        int JointCount { get; }
        int CornerCount { get; }
        float Sharpness { get; }
    }

    public class VisPoint : IPrimitive
    {
        public float X { get; }
        public float Y { get; }

        protected const float pi2 = (float)(Math.PI * 2.0);

        public VisPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public VisPoint(VisPoint p)
        {
            X = p.X;
            Y = p.Y;
        }

        public VisPoint Sample(Gaussian g) => new VisPoint(X + (float)g.Sample(), Y + (float)g.Sample());
        public virtual float Similarity(IPrimitive p) => 0;
        public VisPoint Transpose() => new VisPoint(Y, X);
        public VisPoint Abs() => new VisPoint(Math.Abs(X), Math.Abs(Y));
        public VisPoint Add(VisPoint pt) => new VisPoint(X + pt.X, Y + pt.Y);
        public VisPoint Subtract(VisPoint pt) => new VisPoint(X - pt.X, Y - pt.Y);
        public VisPoint Multiply(VisPoint pt) => new VisPoint(X * pt.X, Y * pt.Y);
        public VisPoint MidPointOf(VisPoint pt) => new VisPoint((pt.X - X) / 2f + X, (pt.Y - Y) / 2f + Y);
        public VisPoint Multiply(float scalar) => new VisPoint(X * scalar, Y * scalar);
        public VisPoint DivideBy(float scalar) => new VisPoint(X / scalar, Y / scalar);

        public float VectorLength() => (float)Math.Sqrt(X * X + Y * Y);
        public float VectorSquaredLength() => X * X + Y * Y;
        public float DistanceTo(VisPoint pt) => (float)Math.Sqrt((pt.X - X) * (pt.X - X) + (pt.Y - Y) * (pt.Y - Y));
        public float SquaredDistanceTo(VisPoint pt) => (pt.X - X) * (pt.X - X) + (pt.Y - Y) * (pt.Y - Y);
        public float DotProduct(VisPoint pt) => X * pt.X + Y * pt.Y; // negative because inverted Y
        public float Atan2(VisPoint pt) => (float)Math.Atan2(pt.Y - Y, pt.X - X);

        public PointF PointF => new PointF(X, Y);

        public LinearDirection LinearDirection(VisPoint pt)
        {
            // make this return probability as well
            LinearDirection result;
            var dir = Math.Atan2(pt.Y - Y, pt.X - X);
            var pi8 = Math.PI / 8f;
            if (dir < -(pi8 * 7))
            {
                result = Primitives.LinearDirection.Horizontal;
            }
            else if (dir < -(pi8 * 5))
            {
                result = Primitives.LinearDirection.TRDiagonal;
            }
            else if (dir < -(pi8 * 3))
            {
                result = Primitives.LinearDirection.Vertical;
            }
            else if (dir < -(pi8 * 1))
            {
                result = Primitives.LinearDirection.TLDiagonal;
            }
            else if (dir < pi8 * 1)
            {
                result = Primitives.LinearDirection.Horizontal;
            }
            else if (dir < pi8 * 3)
            {
                result = Primitives.LinearDirection.TRDiagonal;
            }
            else if (dir < pi8 * 5)
            {
                result = Primitives.LinearDirection.Vertical;
            }
            else if (dir < pi8 * 7)
            {
                result = Primitives.LinearDirection.TLDiagonal;
            }
            else
            {
                result = Primitives.LinearDirection.Horizontal;
            }

            return result;
        }

        public CompassDirection DirectionFrom(VisPoint pt)
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
                result = CompassDirection.SW;
            }
            else if (dir < -(pi8 * 3))
            {
                result = CompassDirection.S;
            }
            else if (dir < -(pi8 * 1))
            {
                result = CompassDirection.SE;
            }
            else if (dir < pi8 * 1)
            {
                result = CompassDirection.E;
            }
            else if (dir < pi8 * 3)
            {
                result = CompassDirection.NE;
            }
            else if (dir < pi8 * 5)
            {
                result = CompassDirection.N;
            }
            else if (dir < pi8 * 7)
            {
                result = CompassDirection.NW;
            }
            else
            {
                result = CompassDirection.W;
            }

            return result;
        }
        public VisPoint ProjectedOntoLine(VisLine line)
        {
            return line.ProjectPointOnto(this);
        }

        public override string ToString()
        {
            return string.Format("Pt:{0:0.##},{1:0.##}", X, Y);
        }
    }

    public enum LinearDirection { Centered, Horizontal, Vertical, TLDiagonal, TRDiagonal }

}
