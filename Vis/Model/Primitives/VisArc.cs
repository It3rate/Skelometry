using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vis.Model.Primitives
{
    // maybe primitive paths always need to reference volume primitives? Or this is a type of stroke only.
    public class VisArc : VisPoint, IPath, IPrimitivePath
    {
        public VisCircle Reference { get; }
        public ClockDirection Direction { get; }

        private float _startAngle;
        private float _endAngle;
        private float _arcLength; // 0 to 2PI

        public float Radius => Reference.Radius;
        public VisPoint Center => Reference.Center;
        public float Length => _arcLength * Radius;

        public VisPoint StartPoint => this;
        public VisPoint MidPoint => Reference.GetPoint(0.5f, 0);
        public VisPoint EndPoint { get; }


        public VisArc(VisCircle circle, VisPoint startPoint, VisPoint endPoint, ClockDirection direction = ClockDirection.CW) : base(startPoint)
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

        public VisPoint GetPoint(float position, float offset = 0)
        {
            var len = _arcLength * position;
            var pos = _startAngle + (Direction == ClockDirection.CW ? len : -len) + pi2;
            return new VisPoint(Reference.X + (float)Math.Cos(pos) * (Radius + offset), Reference.Y + (float)Math.Sin(pos) * (Radius + offset));
            //return Reference.GetPoint(pos, offset);
        }
        public VisPoint GetPointFromCenter(float centeredPosition, float offset = 0)
        {
            return GetPoint(centeredPosition * 2f - 1f, offset);
        }
        public VisPoint GetPoint(CompassDirection direction, float offset = 0)
        {
            var rads = direction.Radians();
            rads = Math.Max(_startAngle, Math.Max(_endAngle, rads));
            return new VisPoint(X + (float)Math.Cos(rads) * (Radius + offset), Y + (float)Math.Sin(rads) * (Radius + offset));
        }

        public VisNode NodeAt(float position) => new VisNode(this, position);
        public VisNode NodeAt(float position, float offset) => new TipNode(this, position, offset);
        public VisNode StartNode => new VisNode(this, 0f);
        public VisNode MidNode => new VisNode(this, 0.5f);
        public VisNode EndNode => new VisNode(this, 1f);

        public ClockDirection CounterDirection => Direction == ClockDirection.CW ? ClockDirection.CCW : ClockDirection.CW;
        public VisArc CounterArc => new VisArc(Reference, EndPoint, StartPoint, CounterDirection);

        public VisPoint[] GetPolylinePoints(int pointCount = 32)
        {
            var result = new List<VisPoint>(pointCount);
            var step = 1f / pointCount;
            result.Add(StartPoint);
            for (var i = step; i < 1f - step; i += step)
            {
                result.Add(GetPoint(i));
            }
            result.Add(EndPoint);
            return result.ToArray();
        }

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
    }
}
