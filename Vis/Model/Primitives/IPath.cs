using Vis.Model.Connections;

namespace Vis.Model.Primitives
{
    using System;
    using System.Collections.Generic;

    public interface IPath : IEnumerable<VisPoint>
    {
	    float Length { get; }
	    VisPoint StartPoint { get; }
	    VisPoint MidPoint { get; }
	    VisPoint EndPoint { get; }

	    VisPoint GetPoint(float position, float offset = 0);
	    VisPoint GetPointFromCenter(float centeredPosition, float offset = 0);

	    VisNode NodeAt(float position, float offset = 0);
	    VisNode NodeNear(VisPoint point);
	    VisNode StartNode { get; }
	    VisNode MidNode { get; }
	    VisNode EndNode { get; }

	    VisPolyline GetPolyline();

        IPath UnitReference { get; set; }
    }
}
