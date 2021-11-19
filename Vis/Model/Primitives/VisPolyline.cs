using System.Drawing;

namespace Vis.Model.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class VisPolyline : VisPoint
    {
        public List<VisPoint> Points { get; } = new List<VisPoint>();

	    public VisPolyline(float x, float y, params float[] remainingPoints) : base(x, y)
	    {
            Points.Add(new VisPoint(X, Y));
		    for (int i = 0; i < remainingPoints.Length; i += 2)
		    {
			    Points.Add(new VisPoint(remainingPoints[i], remainingPoints[i+1]));
		    }
	    }
	    public VisPolyline(VisPoint point, params VisPoint[] remainingPoints) : base(point)
	    {
		    Points.Add(new VisPoint(X, Y));
		    foreach (var pt in remainingPoints)
		    {
			    Points.Add(new VisPoint(pt));
		    }
	    }
	    public VisPolyline(IEnumerable<VisPoint> points) : base(points.First())
	    {
		    foreach (var pt in points)
		    {
			    Points.Add(new VisPoint(pt));
		    }
	    }

	    public void AddOffset(VisPoint offset)
	    {
		    base.AddOffset(offset.X, offset.Y);
            foreach (var point in Points)
		    {
			    point.AddOffset(offset.X, offset.Y);
		    }
	    }
	    public override void AddOffset(float x, float y)
	    {
		    base.AddOffset(x, y);
            foreach (var visPoint in Points)
		    {
			    visPoint.UpdateWith(visPoint.X + x, visPoint.Y + y);
		    }
	    }

        public VisPoint ClonePolyline()
	    {
		    return new VisPolyline(Points);
	    }
	    public override object Clone()
	    {
		    return new VisPolyline(Points);
	    }

	    public override bool Equals(object obj) => this.Equals(obj as IPrimitive);
	    public override bool Equals(IPrimitive other)
	    {
		    bool result = Object.ReferenceEquals(this, other);
		    if (!result && other is VisPolyline polyline && polyline.Points.Count() == Points.Count())
		    {
			    for (int i = 0; i < Points.Count(); i++)
			    {
				    if (polyline.Points[i] != Points[i])
				    {
					    break;
				    }
				    result = true;
			    }
		    }
		    return result;
	    }

	    public static bool operator ==(VisPolyline lhs, VisPolyline rhs)
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
	    public static bool operator !=(VisPolyline lhs, VisPolyline rhs) => !(lhs == rhs);
	    public override int GetHashCode() => Points.GetHashCode();

	    public override string ToString()
	    {
		    return $"poly: {X:0.##}, {Y:0.##}, {Points.Count:0.##}";
	    }
    }
}
