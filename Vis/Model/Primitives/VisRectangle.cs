using System;
using System.Collections.Generic;
using System.Linq;
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
        public float Left => TopLeft.X;
        public float Top => TopLeft.Y;
        public float Right => Center.X + HalfSize.X;
        public float Bottom => Center.Y + HalfSize.Y;
        public float Width => HalfSize.X * 2;
        public float Height => HalfSize.Y * 2;


        public VisRectangle(VisPoint center, VisPoint corner) : base(center.X, center.Y)
        {
            Initialize(corner.X, corner.Y);
        }
        public VisRectangle(float cx, float cy, float cornerX, float cornerY) : base(cx, cy)
        {
            Initialize(cornerX, cornerY);
        }
        private void Initialize(float cornerX, float cornerY)
        {
            TopLeft = new VisPoint(X - Math.Abs(X - cornerX), Y - Math.Abs(Y - cornerY));
            HalfSize = Subtract(TopLeft).Abs();
        }

        public VisPoint GetPoint(float xRatio, float yRatio)
        {
            return new VisPoint(TopLeft.X + HalfSize.X * xRatio, TopLeft.Y + HalfSize.Y * yRatio);
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



        public override string ToString()
        {
            return $"Rect:{TopLeft.X:0.##},{TopLeft.Y:0.##} {Size.X:0.##},{Size.Y:0.##}";
        }
    }

}
