using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Distributions;
using Vis.Model.Connections;

namespace Vis.Model.Primitives
{
    // VisPoint was specifically not an IPath, with the idea that all nodes should reference a structure 
    // rather than an arbitrary point (all strokes are relative to something else, and ultimately there
    // is a mental frame things get placed in, like a letterbox).
    // With abstact math this is still true, but user input with a mouse kind of requires points that are picked.
    // So may make this an IPath (so nodes can reference them), but if that happens use sparingly!
    // Or just use a letterbox the size of the screen. hmm.

    public class VisPoint : IPrimitive
    {
        public float X { get; protected set; }
        public float Y { get; protected set; }
        public virtual bool IsPath => false;

        public static int _index = 0;
        public int Index;

        public float NearThreshold = 0.003f;
        protected const float pi2 = (float)(Math.PI * 2.0);

        public VisPoint(float x, float y)
        {
            X = x;
            Y = y;
            Index = _index++;
        }
        public VisPoint(VisPoint p):this(p.X, p.Y) { }

        public virtual void AddOffset(float x, float y)
        {
	        UpdateWith(X + x, Y + y);
        }

        public void UpdateWith(VisPoint p)
        {
	        X = p.X;
	        Y = p.Y;
        }
        public void UpdateWith(float x, float y)
        {
	        X = x;
	        Y = y;
        }

        public void SetIndex()
        {

        }
        public bool IsNear(VisPoint p)
        {
	        return this.SquaredDistanceTo(p) < NearThreshold;
        }
        public virtual VisPoint ProjectPointOnto(VisPoint p)
        {
	        return ClonePoint();
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

        public float Length() => (float)Math.Sqrt(X * X + Y * Y);
        public float SquaredLength() => X * X + Y * Y;
        public float DistanceTo(VisPoint pt) => (float)Math.Sqrt((pt.X - X) * (pt.X - X) + (pt.Y - Y) * (pt.Y - Y));
        public float SquaredDistanceTo(VisPoint pt) => (float)Math.Abs((pt.X - X) * (pt.X - X) + (pt.Y - Y) * (pt.Y - Y));
        public float DotProduct(VisPoint pt) => X * pt.X + Y * pt.Y; // negative because inverted Y
        public float Atan2(VisPoint pt) => (float)Math.Atan2(pt.Y - Y, pt.X - X);
        public float SignedDistanceTo(VisPoint pt)
        {
	        var sDist = (pt.X - X) * (pt.X - X) + (pt.Y - Y) * (pt.Y - Y);
	        return (float)Math.Sqrt(sDist) * (sDist >= 0 ? 1f : -1f);
        } 
        public (float, float, float) ABCLine(VisPoint pt)
        {
	        var a = pt.Y - Y;
	        var b = X - pt.X;
	        var c = a * X + b * Y;
	        return (a, b, c);
        }

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

        public virtual VisPolyline GetPolyline()
        {
            return new VisPolyline(X, Y, X, Y);
        }

        public VisPoint ClonePoint()
        {
	        return new VisPoint(this.X, this.Y);
        }
        public virtual object Clone()
        {
	        return new VisPoint(this.X, this.Y);
        }

        public override bool Equals(object obj) => this.Equals(obj as IPrimitive);
        public virtual bool Equals(IPrimitive other)
        {
            return other != null && X == other.X && Y == other.Y;
        }

        public static bool operator ==(VisPoint lhs, VisPoint rhs)
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
        public static bool operator !=(VisPoint lhs, VisPoint rhs) => !(lhs == rhs);
        public override int GetHashCode() => (X, Y).GetHashCode();

        public override string ToString()
        {
            return string.Format("p:{0:0.##},{1:0.##}", X, Y);
        }
    }

    public enum LinearDirection { Centered, Horizontal, Vertical, TLDiagonal, TRDiagonal }

}
