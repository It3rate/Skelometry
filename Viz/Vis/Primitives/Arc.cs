using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTest.Vis
{
    // maybe primitive paths always need to reference volume primitives? Or this is a type of stroke only.
    public class Arc : Point, IPath, IPrimitivePath
    {
        public Circle Reference { get; }
        public ClockDirection Direction { get; }

        private float _startAngle;
        private float _endAngle;
        private float _arcLength; // 0 to 2PI

        public float Radius => Reference.Radius;
        public Point Center => Reference.Center;
        public float Length =>_arcLength * Radius;

        public Point StartPoint => this;
        public Point MidPoint => Reference.GetPoint(0.5f, 0);
        public Point EndPoint { get; }


        public Arc(Circle circle, Point startPoint, Point endPoint, ClockDirection direction = ClockDirection.CW) : base(startPoint)
        {
            Reference = circle;
            EndPoint = endPoint;
            Direction = direction;
            _startAngle = circle.Center.Atan2(startPoint);
            _endAngle = circle.Center.Atan2(endPoint);
            _arcLength = (Math.Max(_startAngle, _endAngle) - Math.Min(_startAngle, _endAngle)) % pi2;
            if (direction == ClockDirection.CCW)
            {
                _arcLength = pi2 - _arcLength;
            }
        }

        public Point GetPoint(float position, float offset = 0)
        {
            var len = _arcLength * position;
            var pos = _startAngle + (Direction == ClockDirection.CW ? len : -len) + pi2;
            return new Point(Reference.X + (float)Math.Cos(pos) * (Radius + offset), Reference.Y + (float)Math.Sin(pos) * (Radius + offset));
            //return Reference.GetPoint(pos, offset);
        }
        public Point GetPointFromCenter(float centeredPosition, float offset = 0)
        {   
            return GetPoint(centeredPosition * 2f - 1f, offset);
        }
        public Point GetPoint(CompassDirection direction, float offset = 0)
        {
	        var rads = direction.Radians();
	        rads = Math.Max(_startAngle, Math.Max(_endAngle, rads));
	        return new Point(X + (float)Math.Cos(rads) * (Radius + offset), Y + (float)Math.Sin(rads) * (Radius + offset));
        }

        public Node NodeAt(float position) => new Node(this, position);
        public Node NodeAt(float position, float offset) => new TipNode(this, position, offset);
        public Node StartNode => new Node(this, 0f);
        public Node MidNode => new Node(this, 0.5f);
        public Node EndNode => new Node(this, 1f);

        public ClockDirection CounterDirection => Direction == ClockDirection.CW ? ClockDirection.CCW : ClockDirection.CW;
        public Arc CounterArc => new Arc(Reference, EndPoint, StartPoint, CounterDirection);

        public Point[] GetPolylinePoints(int pointCount = 32)
        {
            var result = new List<Point>(pointCount);
            var step = 1f / (float)pointCount;
            result.Add(StartPoint);
            for (var i = step; i < 1f - step; i += step)
            {
                result.Add(GetPoint(i));
            }
            result.Add(EndPoint);
            return result.ToArray();
        }
    }
}
