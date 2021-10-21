using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    /// <summary>
    /// A rectangle defined by a center and corner (can not be rotated or skewed). This isn't a  path, but it's points and lines can be used for reference.
    /// For convenience it can be turned into four strokes if a box made of strokes is desired (though you may want more control over the stroke order).
    /// </summary>
    public class Rectangle : Point
    {
        public Point TopLeft { get; private set; }
        public Point Size => HalfSize.Multiply(2f);
        public Point HalfSize { get; private set; }
        public Point Center => this;

        public Rectangle(Point center, Point corner) : base(center.X, center.Y)
        {
            Initialize(corner.X, corner.Y);
        }
        public Rectangle(float cx, float cy, float cornerX, float cornerY) : base(cx, cy)
        {
            Initialize(cornerX, cornerY);
        }
        private void Initialize(float cornerX, float cornerY)
        {
            TopLeft = new Point(X - Math.Abs(X - cornerX), Y - Math.Abs(Y - cornerY));
            HalfSize = this.Subtract(TopLeft).Abs();
        }

        public Point GetPoint(float xRatio, float yRatio)
        {
            return new Point(TopLeft.X + HalfSize.X * xRatio, TopLeft.Y + HalfSize.Y * yRatio);
        }


        public Line GetLine(CompassDirection direction, float offset = 0)
        {
	        return direction.GetLineFrom(this, offset);
        }
        public Point GetPoint(CompassDirection direction)
        {
	        return direction.GetPointFrom(this);
        }

        public Point NearestIntersectionTo(Point p) => null;
        public bool IntersectsWith(Point p) => false;
        public bool IntersectsWith(Line line) => Math.Abs(Center.X - line.Center.X) <= HalfSize.X + line.MidPoint.X && Math.Abs(Center.Y - line.Center.Y) <= HalfSize.Y + line.MidPoint.Y;
        public bool IntersectsWith(Rectangle rect) => Math.Abs(Center.X - rect.Center.X) <= HalfSize.X + rect.HalfSize.X && Math.Abs(Center.Y - rect.Center.Y) <= HalfSize.Y + rect.HalfSize.Y;
        public bool Contains(Point p) => false;
        public bool Contains(Line line) => false;
        public bool Contains(Rectangle rect) => Math.Abs(Center.X - rect.Center.X) + rect.HalfSize.X <= HalfSize.X && Math.Abs(Center.Y - rect.Center.Y) + rect.HalfSize.Y <= HalfSize.Y;
	        


        public override string ToString()
        {
	        return $"Rect:{TopLeft.X:0.##},{TopLeft.Y:0.##} {Size.X:0.##},{Size.Y:0.##}";
        }
    }

}
