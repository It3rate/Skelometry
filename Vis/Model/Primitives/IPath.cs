using Vis.Model.Connections;

namespace Vis.Model.Primitives
{
    using System;
    using System.Collections.Generic;

    public interface IPath : IElement, IEnumerable<VisPoint>
    {
	    float Length { get; }
	    bool IsFixed { get; set; }
        VisPoint StartPoint { get; }
	    VisPoint MidPoint { get; }
	    VisPoint EndPoint { get; }

	    VisPoint GetPoint(float shift, float offset = 0);
	    VisPoint GetPointFromCenter(float centeredShift, float offset = 0);

	    int AnchorCount { get; }
	    VisNode ClosestAnchor(float shift);
	    VisNode ClosestAnchor(VisPoint point);

        VisNode CreateNodeAt(float shift, float offset = 0);
	    VisNode NodeNear(VisPoint point);
	    VisNode StartNode { get; }
	    VisNode MidNode { get; }
	    VisNode EndNode { get; }
	    OffsetNode NodeFor(VisPoint pt);
	    VisNode BestNodeForPoint(VisPoint pt);

        IPath UnitReference { get; set; }

    }
}
