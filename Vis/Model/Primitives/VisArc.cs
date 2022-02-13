using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vis.Model.Connections;

namespace Vis.Model.Primitives
{
    // maybe primitive paths always need to reference volume primitives? Or this is a type of stroke only.
    public class VisArc : VisPoint, IPath, IPrimitivePath
    {
        public VisCircle Reference { get; }
        public ClockDirection Direction { get; }

        public override bool IsPath => true;
        public bool IsFixed { get; set; } = false;

        private readonly float _startAngle;
        private readonly float _endAngle;
        private readonly float _arcLength; // 0 to 2PI

        public float Radius => Reference.Radius;
        public VisPoint Center => Reference.Center;
        public override float Length() => _arcLength * Radius;

        public VisPoint StartPoint => this;
        public VisPoint MidPoint => Reference.GetPoint(0.5f, 0);
        public VisPoint EndPoint { get; }

        public int AnchorCount => 2;
        public VisNode ClosestAnchor(float shift) => StartNode;
        public VisNode ClosestAnchor(VisPoint point) => StartNode;

        public IPath UnitReference { get; set; }

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

            if (_arcLength < circle.NearThreshold)
            {
	            _arcLength = pi2;
            }
        }

        public OffsetNode NodeFor(VisPoint pt)
        {
	        throw new NotImplementedException();
        }

        public VisNode BestNodeForPoint(VisPoint pt)
        {
	        throw new NotImplementedException();
        }
        public VisPoint GetPoint(float shift, float offset = 0)
        {
            var len = _arcLength * shift;
            var pos = _startAngle + (Direction == ClockDirection.CW ? len : -len) + pi2;
            return new VisPoint(Reference.X + (float)Math.Cos(pos) * (Radius + offset), Reference.Y + (float)Math.Sin(pos) * (Radius + offset));
            //return Reference.GetPoint(pos, offset);
        }
        public VisPoint GetPointFromCenter(float centeredShift, float offset = 0)
        {
            return GetPoint(centeredShift * 2f - 1f, offset);
        }
        public VisPoint GetPoint(CompassDirection direction, float offset = 0)
        {
            var rads = direction.Radians();
            rads = Math.Max(_startAngle, Math.Max(_endAngle, rads));
            return new VisPoint(X + (float)Math.Cos(rads) * (Radius + offset), Y + (float)Math.Sin(rads) * (Radius + offset));
        }

        public VisNode CreateNodeAt(float shift) => new VisNode(this, shift);
        public VisNode CreateNodeAt(float shift, float offset) => new OffsetNode(this, shift, offset);
        public VisNode StartNode => new VisNode(this, 0f);
        public VisNode MidNode => new VisNode(this, 0.5f);
        public VisNode EndNode => new VisNode(this, 1f);

        public ClockDirection CounterDirection => Direction == ClockDirection.CW ? ClockDirection.CCW : ClockDirection.CW;
        public VisArc CounterArc => new VisArc(Reference, EndPoint, StartPoint, CounterDirection);

        public override void AddOffset(float x, float y)
        {
	        base.AddOffset(x, y);
	        Center.AddOffset(x, y);
            EndPoint.AddOffset(x, y);
        }
        public override VisPoint ProjectPointOnto(VisPoint p)
        {
	        return Reference.ProjectPointOnto(p);
        }
        public override VisPolyline GetPolyline()
        {
	        return new VisPolyline(GetPolylinePoints());
        }
        public VisPoint[] GetPolylinePoints(int pointCount = 64)
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

        public override bool Equals(object obj) => this.Equals(obj as IPrimitive);
        public override bool Equals(IPrimitive other)
        {
	        bool result = Object.ReferenceEquals(this, other);
	        if (!result && other != null && other is VisArc arc)
            {
                result = Object.ReferenceEquals(Reference, arc.Reference) &&
                    (EndPoint == arc.EndPoint) &&
                    (Direction == arc.Direction);
            }
            return result;
        }
        public static bool operator ==(VisArc lhs, VisArc rhs)
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
        public static bool operator !=(VisArc lhs, VisArc rhs) => !(lhs == rhs);
        public override int GetHashCode() => (Reference, EndPoint, Direction).GetHashCode();
        public override string ToString()
        {
	        return $"Arc:{X:0.##},{Y:0.##} r{Radius:0.##}";
        }
    }
}
