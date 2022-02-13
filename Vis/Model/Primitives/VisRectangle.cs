using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Vis.Model.Primitives
{
    /// <summary>
    /// A rectangle defined by a center and corner (can not be rotated or skewed). This isn't a  path, but it's points and lines can be used for reference.
    /// For convenience it can be turned into four strokes if a box made of strokes is desired (though you may want more control over the stroke order).
    /// </summary>
    public class VisRectangle : VisPoint
    {
	    public VisPoint TopLeft { get; private set; }
        public VisPoint Size => HalfSize.Multiply(2f);
        public VisPoint HalfSize { get; private set; }
        public VisPoint Center => this;

        public override bool IsPath => true;

        public float Left => TopLeft.X;
        public float Top => TopLeft.Y;
        public float Right => Center.X + HalfSize.X;
        public float Bottom => Center.Y + HalfSize.Y;
        public float Width => HalfSize.X * 2;
        public float Height => HalfSize.Y * 2;

        protected float _cornerX;
        protected float _cornerY;

        public VisRectangle(float cx, float cy, float cornerX, float cornerY) : base(cx, cy)
        {
	        _cornerX = cornerX;
	        _cornerY = cornerY;
            Initialize(cornerX, cornerY);
        }
        public VisRectangle(VisPoint center, VisPoint corner) : this(center.X, center.Y, corner.X, corner.Y) { }
        public VisRectangle(VisRectangle r) : this(r.Center.X, r.Center.Y, r._cornerX, r._cornerY) {  }

        private void Initialize(float cornerX, float cornerY)
        {
            TopLeft = new VisPoint(X - Math.Abs(X - cornerX), Y - Math.Abs(Y - cornerY));
            HalfSize = this.Subtract(TopLeft).Abs();
        }

        public VisPoint GetPoint(float xRatio, float yRatio)
        {
            return new VisPoint(TopLeft.X + HalfSize.X * xRatio, TopLeft.Y + HalfSize.Y * yRatio);
        }

        public override void AddOffset(float x, float y)
        {
	        base.AddOffset(x, y);
            TopLeft.AddOffset(x, y);
        }

        public VisLine GetLine(CompassDirection direction, float offset = 0)
        {
            return direction.GetLineFrom(this, offset);
        }
        public VisPoint GetPoint(CompassDirection direction)
        {
            return direction.GetPointFrom(this);
        }

        public virtual VisRectangle BoundingBox() => new VisRectangle(this, TopLeft);
        
        public VisPoint NearestIntersectionTo(VisPoint p) => null;
        public bool IntersectsWith(VisPoint p) => false;
        public bool IntersectsWith(VisLine line) => Math.Abs(Center.X - line.Center.X) <= HalfSize.X + line.MidPoint.X && Math.Abs(Center.Y - line.Center.Y) <= HalfSize.Y + line.MidPoint.Y;
        public bool IntersectsWith(VisRectangle rect) => Math.Abs(Center.X - rect.Center.X) <= HalfSize.X + rect.HalfSize.X && Math.Abs(Center.Y - rect.Center.Y) <= HalfSize.Y + rect.HalfSize.Y;
        public bool Contains(VisPoint p) => false;
        public bool Contains(VisLine line) => false;
        public bool Contains(VisRectangle rect) => Math.Abs(Center.X - rect.Center.X) + rect.HalfSize.X <= HalfSize.X && Math.Abs(Center.Y - rect.Center.Y) + rect.HalfSize.Y <= HalfSize.Y;

        public override VisPolyline GetPolyline()
        {
	        return new VisPolyline(Left, Top, Right, Top, Right, Bottom, Right, Left);
        }

        public VisRectangle CloneRectangle()
        {
	        return new VisRectangle(this);
        }

        public override object Clone()
        {
	        return new VisRectangle(this);
        }
        public override bool Equals(object obj) => this.Equals(obj as IPrimitive);
        public override bool Equals(IPrimitive other)
        {
	        bool result = Object.ReferenceEquals(this, other);
	        if (!result && other != null && other is VisRectangle rect)
            {
                result = (Center == rect.Center) && (TopLeft == rect.TopLeft) && (HalfSize == rect.HalfSize);
            }
            return result;
        }
        public static bool operator ==(VisRectangle lhs, VisRectangle rhs)
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
        public static bool operator !=(VisRectangle lhs, VisRectangle rhs) => !(lhs == rhs);
        public override int GetHashCode() => (Center, TopLeft, HalfSize).GetHashCode();
        public override string ToString()
        {
            return $"Rect:{TopLeft.X:0.##},{TopLeft.Y:0.##} {Size.X:0.##},{Size.Y:0.##}";
        }
    }

}
