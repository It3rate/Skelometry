using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Agent;
using Vis.Model.Connections;

namespace Vis.Model.Primitives
{
    /// <summary>
    /// A circle centered at the XY point, with a Radius. There is no natural 'home' angle -a second point is given to calculate the radius and orient it the circle (which becomes 0).
    /// Circles are the only natural primitive that can have an area - it is a 'large point', not a shape made of joints.
    /// The perimeter is 0-1 as it has a start and end from angle zero around (default is clockwise).
    /// It will be used as tangent targets to and from other points (it arrives and leaves, doesn't care about orientation), unless it is a point starting or ending on the circle.
    /// </summary>
    public class VisCircle : VisPoint, IPath
    {
        public ClockDirection Direction { get; }
        public VisPoint PerimeterOrigin { get; }
        public float Radius { get; private set; }

        public override bool IsPath => true;
        public bool IsFixed { get; set; } = false;

        public float OriginAngle { get; private set; }
        public new float Length => (float)(2f * Radius * Math.PI);

        public VisPoint StartPoint => PerimeterOrigin;
        public VisPoint MidPoint => GetPoint(0.5f, 0);
        public VisPoint EndPoint => PerimeterOrigin;

        public VisPoint Center => this;
        public VisNode CenterNode;

        public int AnchorCount => 2;
        public VisNode ClosestAnchor(float shift) => StartNode;
        public VisNode ClosestAnchor(VisPoint point) => StartNode;

        public VisNode BestNodeForPoint(VisPoint pt)
        {
	        throw new NotImplementedException();
        }

        public IPath UnitReference { get; set; }

        //public CircleRef(Point center, float radius) : base(center.X, center.Y)
        //{
        //	Radius = radius;
        //	PerimeterOrigin = new Point(X, Y - radius); // default north
        //}
        public VisCircle(float cx, float cy, float perimeterX, float perimeterY, ClockDirection direction = ClockDirection.CW) : base(cx, cy)
        {
            PerimeterOrigin = new VisPoint(perimeterX, perimeterY);
            Direction = direction;
            Initialize();
        }
        public VisCircle(VisPoint center, VisPoint perimeterOrigin, ClockDirection direction = ClockDirection.CW) : this(center.X, center.Y, perimeterOrigin.X, perimeterOrigin.Y, direction)
        {
        }
        public VisCircle(VisNode center, VisNode perimeterOrigin, ClockDirection direction = ClockDirection.CW) : this(center.Location, perimeterOrigin.Location, direction) { }
        public VisCircle(VisCircle circle) : this(circle.Center, circle.PerimeterOrigin, circle.Direction) { }

        /// <summary>
        /// Radius is origin to line, CW or CCW determines if it winds right or left to the line.
        /// </summary>
		public static VisCircle CircleFromLineAndPoint(VisLine line, VisNode perimeterOrigin, ClockDirection direction = ClockDirection.CW)
        {
            var p0 = perimeterOrigin.Location;
            var onLine = p0.ProjectedOntoLine(line);
            var diff = p0.Subtract(onLine);
            var radius = diff.Length();
            var center = direction == ClockDirection.CW ? p0.Add(diff.Transpose()) : p0.Subtract(diff.Transpose());
            VisCircle result = new VisCircle(center, p0, direction);
            return result;
        }
        private void Initialize()
        {
            Radius = Center.DistanceTo(PerimeterOrigin);
            OriginAngle = Center.Atan2(PerimeterOrigin);
        }

        /// <summary>
        /// Gets point along circumference of this circle using shift and offset.
        /// </summary>
        /// <param name="shift">Normalized (0-1) amount along the circle (0 is north, positive is clockwise, negative is counter clockwise). </param>
        /// <param name="offset">Offset from circumference. Negative is inside, positive is outside. Zero is default, -1 is start.</param>
        /// <returns></returns>
        public VisPoint GetPoint(float shift, float offset = 0)
        {
            var len = pi2 * shift;
            var pos = OriginAngle + (Direction == ClockDirection.CW ? len : -len);
            return new VisPoint(X + (float)Math.Cos(pos) * (Radius + offset), Y + (float)Math.Sin(pos) * (Radius + offset));
        }

        public VisPoint GetPointFromCenter(float centeredShift, float offset = 0)
        {
            return GetPoint(centeredShift * 2f - 1f, offset);
        }
        public VisPoint GetPoint(CompassDirection direction, float offset = 0)
        {
            var rads = direction.Radians();
            return new VisPoint(X + (float)Math.Cos(rads) * (Radius + offset), Y + (float)Math.Sin(rads) * (Radius + offset));
        }

        public VisStroke GetTangentArc(VisPoint leftPoint, VisPoint rightPoint) => null;

        public OffsetNode NodeFor(VisPoint pt)
        {
	        throw new NotImplementedException();
        }
        public VisNode CreateNodeAt(float shift) => new VisNode(this, shift);
        public VisNode CreateNodeAt(float shift, float offset) => new OffsetNode(this, shift, offset);

        public VisNode StartNode => new VisNode(this, 0f);
        public VisNode MidNode => new VisNode(this, 0.5f);
        public VisNode EndNode => new VisNode(this, 1f);

        public OffsetNode CreateNodeAt(CompassDirection direction, float offset = 0)
        {
            var rads = direction.Radians() - OriginAngle;
            rads = (rads + pi2) % pi2;
            return new OffsetNode(this, rads / pi2, offset);
        }

        public ClockDirection CounterDirection => Direction.Counter();
        public VisCircle CounterCircle => new VisCircle(Center, PerimeterOrigin, CounterDirection);

        public VisPoint FindTangentInDirection(VisPoint p, ClockDirection direction)
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

        public int FindTangents(VisPoint p, out VisPoint pt0, out VisPoint pt1)
        {
            var dist = Center.DistanceTo(p);
            var diameter = Radius * 2;
            var L = Math.Sqrt(dist - diameter);
            var numberOfSolutions = IntersectCircle(p, (float)L, out pt0, out pt1);
            return numberOfSolutions;
        }

        public int IntersectCircle(VisCircle c1, out VisPoint intersect0, out VisPoint intersect1) => IntersectCircle(c1.Center, c1.Radius, out intersect0, out intersect1);
        public int IntersectCircle(VisPoint c1, float r1, out VisPoint intersect0, out VisPoint intersect1)
        {
            var dist = Center.DistanceTo(c1);

            // See how many solutions there are.
            if (dist > Radius + r1)
            {
                // No solutions, the circles are too far apart.
                intersect0 = new VisPoint(float.NaN, float.NaN);
                intersect1 = new VisPoint(float.NaN, float.NaN);
                return 0;
            }
            else if (dist < Math.Abs(Radius - r1))
            {
                // No solutions, one circle contains the other.
                intersect0 = new VisPoint(float.NaN, float.NaN);
                intersect1 = new VisPoint(float.NaN, float.NaN);
                return 0;
            }
            else if (dist == 0 && Radius == r1)
            {
                // No solutions, the circles coincide.
                intersect0 = new VisPoint(float.NaN, float.NaN);
                intersect1 = new VisPoint(float.NaN, float.NaN);
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
                intersect0 = new VisPoint(p2.X + dif.Y, p2.Y - dif.X);
                intersect1 = new VisPoint(p2.X - dif.Y, p2.Y + dif.X);
                //var dif = c1.Subtract(Center).Transpose().DivideBy(dist).Multiply(h);
                //intersect0 = new Point(p2.X + dif.X, p2.Y - dif.Y);
                //intersect1 = new Point(p2.X - dif.X, p2.Y + dif.Y);

                // See if we have 1 or 2 solutions.
                return dist == Radius + r1 ? 1 : 2;
            }
        }

        public override void AddOffset(float x, float y)
        {
	        base.AddOffset(x, y);
            PerimeterOrigin.AddOffset(x, y);
            Initialize();
        }
        public override VisPoint ProjectPointOnto(VisPoint p)
        {
	        var dif = p.Subtract(Center);
	        var result = dif.DivideBy(dif.Length()).Multiply(Radius).Add(Center);
	        return result;
        }
        public override VisPolyline GetPolyline()
        {
	        return new VisPolyline(GetPolylinePoints());
        }
        public VisPoint[] GetPolylinePoints(int pointCount = 24)
        {
            var result = new List<VisPoint>(pointCount);
            var step = 1f / pointCount;
            for (float i = 0; i < 1.0; i += step)
            {
                result.Add(GetPoint(i));
            }
            return result.ToArray();
        }

        public virtual VisRectangle BoundingBox() => new VisRectangle(Center, new VisPoint(Center.X - Radius, Center.Y - Radius));

        public VisNode NodeNear(VisPoint point)
        {
	        VisNode result = null;
	        if (StartPoint.SquaredDistanceTo(point) < point.NearThreshold)
	        {
		        result = StartNode;
	        }
	        else if (EndPoint.SquaredDistanceTo(point) < point.NearThreshold)
	        {
		        result = EndNode;
	        }
	        else if (Center.SquaredDistanceTo(point) < point.NearThreshold)
	        {
		        //result = Center; // need to define nodes for things like centers and areas along perimeter in order to know the source of matches
	        }
            return result;
        }

        public IEnumerator<VisPoint> GetEnumerator()
        {
            yield return StartPoint;
            yield return EndPoint;
            yield return Center;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return StartPoint;
            yield return EndPoint;
            yield return Center;

        }
        public VisCircle CloneCircle()
        {
	        return new VisCircle(this);
        }
        public override object Clone()
        {
	        return new VisCircle(this);
        }
        public override bool Equals(object obj) => this.Equals(obj as IPrimitive);
        public override bool Equals(IPrimitive other)
        {
	        bool result = Object.ReferenceEquals(this, other);
	        if (!result && other != null && other is VisCircle circ)
            {
                result = (Center == circ.Center) && 
                    (PerimeterOrigin == circ.PerimeterOrigin) && 
                    (Direction == circ.Direction);
            }
            return result;
        }
        public static bool operator ==(VisCircle lhs, VisCircle rhs)
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
        public static bool operator !=(VisCircle lhs, VisCircle rhs) => !(lhs == rhs);
        public override int GetHashCode() => (Center, PerimeterOrigin, Direction).GetHashCode();
        public override string ToString()
        {
            return $"Circ:{X:0.##},{Y:0.##} r{Radius:0.##}";
        }
    }



}
