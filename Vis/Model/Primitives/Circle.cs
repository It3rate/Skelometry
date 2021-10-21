using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vis.Model
{
    /// <summary>
    /// A circle centered at the XY point, with a Radius. There is no natural 'home' angle -a second point is given to calculate the radius and orient it the circle (which becomes 0).
    /// Circles are the only natural primitive that can have an area - it is a 'large point', not a shape made of joints.
    /// The perimeter is 0-1 as it has a start and end from angle zero around (default is clockwise).
    /// It will be used as tangent targets to and from other points (it arrives and leaves, doesn't care about orientation), unless it is a point starting or ending on the circle.
    /// </summary>
    public class Circle : Point, IPath
	{
		public ClockDirection Direction { get; }
        public Point PerimeterOrigin { get; }
		public float Radius { get; private set; }

        public float OriginAngle { get; private set; }
		public float Length => (float)(2f * Radius * Math.PI);

		public Point StartPoint => PerimeterOrigin;
		public Point MidPoint => GetPoint(0.5f, 0);
		public Point EndPoint => PerimeterOrigin;

		public Point Center => this;

        //public CircleRef(Point center, float radius) : base(center.X, center.Y)
        //{
        //	Radius = radius;
        //	PerimeterOrigin = new Point(X, Y - radius); // default north
        //}
        public Circle(Point center, Point perimeterOrigin, ClockDirection direction = ClockDirection.CW) : base(center.X, center.Y)
		{
			PerimeterOrigin = perimeterOrigin;
			Direction = direction;
			Initialize();
        }
        public Circle(float cx, float cy, float perimeterX, float perimeterY, ClockDirection direction = ClockDirection.CW) : base(cx, cy)
		{
			PerimeterOrigin = new Point(perimeterX, perimeterY);
			Direction = direction;
			Initialize();
		}
		public Circle(Node center, Node perimeterOrigin, ClockDirection direction = ClockDirection.CW) : this(center.Anchor, perimeterOrigin.Anchor, direction){ }

        /// <summary>
        /// Radius is origin to line, CW or CCW determines if it winds right or left to the line.
        /// </summary>
		public static Circle CircleFromLineAndPoint(Line line, Node perimeterOrigin, ClockDirection direction = ClockDirection.CW)
        {
	        var p0 = perimeterOrigin.Anchor;
	        var onLine = p0.ProjectedOntoLine(line);
	        var diff = p0.Subtract(onLine);
	        var radius = diff.VectorLength();
	        var center = direction == ClockDirection.CW ? p0.Add(diff.Transpose()) : p0.Subtract(diff.Transpose());
            Circle result = new Circle(center, p0, direction);      
			return result;
		}
        private void Initialize()
		{
			Radius = Center.DistanceTo(PerimeterOrigin);
			OriginAngle = Center.Atan2(PerimeterOrigin);
        }

		/// <summary>
		/// Gets point along circumference of this circle using position and offset.
		/// </summary>
		/// <param name="position">Normalized (0-1) amount along the circle (0 is north, positive is clockwise, negative is counter clockwise). </param>
		/// <param name="offset">Offset from circumference. Negative is inside, positive is outside. Zero is default, -1 is start.</param>
		/// <returns></returns>
		public Point GetPoint(float position, float offset = 0)
		{
			var len = pi2 * position;
            var pos = OriginAngle + (Direction == ClockDirection.CW ? len : -len);
            return new Point(X + (float)Math.Cos(pos) * (Radius + offset), Y + (float)Math.Sin(pos) * (Radius + offset));
        }

		public Point GetPointFromCenter(float centeredPosition, float offset = 0)
		{
			return GetPoint(centeredPosition * 2f - 1f, offset);
		}
		public Point GetPoint(CompassDirection direction, float offset = 0)
		{
			var rads = direction.Radians();
			return new Point(X + (float)Math.Cos(rads) * (Radius + offset), Y + (float)Math.Sin(rads) * (Radius + offset));
		}

        public Stroke GetTangentArc(Point leftPoint, Point rightPoint) => null;

		public Node NodeAt(float position) => new Node(this, position);
		public Node NodeAt(float position, float offset) => new TipNode(this, position, offset);
		
        public Node StartNode => new Node(this, 0f);
		public Node MidNode => new Node(this, 0.5f);
		public Node EndNode => new Node(this, 1f);

		public TipNode NodeAt(CompassDirection direction, float offset = 0)
		{
			var rads = direction.Radians() - OriginAngle;
			rads = (rads + pi2) % pi2;
			return new TipNode(this, rads / pi2, offset);
		}

		public ClockDirection CounterDirection => Direction.Counter();
        public Circle CounterCircle => new Circle(Center, PerimeterOrigin, CounterDirection);

        public Point FindTangentInDirection(Point p, ClockDirection direction)
        {
	        var distSquared = Center.SquaredDistanceTo(p);
	        if (distSquared < Radius * Radius)
	        {
		        return p; // on line -- need to account for inside circle as well.
	        }
            var L = Math.Sqrt(distSquared - Radius * Radius);
	        var numberOfSolutions = IntersectCircle(p, (float)L, out var pt0, out var pt1);
	        return direction == ClockDirection.CW ? pt1 : pt0;
        }

        public int FindTangents(Point p, out Point pt0, out Point pt1)
        {
	        var dist = Center.DistanceTo(p);
	        var diameter = Radius * 2;
	        var L = Math.Sqrt(dist - diameter);
            var numberOfSolutions = IntersectCircle(p, (float)L, out pt0, out pt1);
            return numberOfSolutions;
        }

        public int IntersectCircle(Circle c1, out Point intersect0, out Point intersect1) => IntersectCircle(c1.Center, c1.Radius, out intersect0, out intersect1);
        public int IntersectCircle(Point c1, float r1, out Point intersect0, out Point intersect1)
        {
	        var dist = Center.DistanceTo(c1);

            // See how many solutions there are.
            if (dist > Radius + r1)
            {
                // No solutions, the circles are too far apart.
                intersect0 = new Point(float.NaN, float.NaN);
                intersect1 = new Point(float.NaN, float.NaN);
                return 0;
            }
            else if (dist < Math.Abs(Radius - r1))
            {
                // No solutions, one circle contains the other.
                intersect0 = new Point(float.NaN, float.NaN);
                intersect1 = new Point(float.NaN, float.NaN);
                return 0;
            }
            else if ((dist == 0) && (Radius == r1))
            {
                // No solutions, the circles coincide.
                intersect0 = new Point(float.NaN, float.NaN);
                intersect1 = new Point(float.NaN, float.NaN);
                return 0;
            }
            else
            {
                // Find a and h.
                var a = (Radius * Radius - r1 * r1 + dist * dist) / (2f * dist);
                var h = (float)Math.Sqrt(Radius * Radius - a * a);

                // Find P2.
                var p2 = c1.Subtract(Center).DivideBy(dist).Multiply(a).Add(Center);

                // Get the points P3.
                var dif = c1.Subtract(Center).DivideBy(dist).Multiply(h);
                intersect0 = new Point(p2.X + dif.Y, p2.Y - dif.X);
                intersect1 = new Point(p2.X - dif.Y, p2.Y + dif.X);
                //var dif = c1.Subtract(Center).Transpose().DivideBy(dist).Multiply(h);
                //intersect0 = new Point(p2.X + dif.X, p2.Y - dif.Y);
                //intersect1 = new Point(p2.X - dif.X, p2.Y + dif.Y);

                // See if we have 1 or 2 solutions.
                return (dist == Radius + r1) ? 1 : 2;
            }
        }

        public Point[] GetPolylinePoints(int pointCount = 24)
        {
	        var result = new List<Point>(pointCount);
	        var step = 1f / (float)pointCount;
	        for (float i = 0; i < 1.0; i += step)
	        {
		        result.Add(GetPoint(i));
	        }
	        return result.ToArray();
        }

        public override string ToString()
        {
	        return $"Circ:{X:0.##},{Y:0.##} r{Radius:0.##}";
        }
    }



}
