using Vis.Model.Primitives;

namespace Vis.Model.Connections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OffsetNode : VisNode
    {
	    // Offset can't be zero in a middle node, as it causes overlap on lines that are tangent to each other. 
	    // The corner of a P is part of the shape with potential overlap on the serif.
	    // Maybe X could be a V with overlap.H would be a half U with 0.5 overlap. Maybe this is too obfuscated. Yes it is. Might work for serifs though.
	    public float Offset { get; protected set; }

	    public override VisPoint Location => Reference.GetPoint(Shift, Offset);

	    public OffsetNode(IPath reference, float shift, float offset) : base(reference, shift)
	    {
		    Offset = offset;
	    }

	    public override VisPoint GetPoint(float shift, float offset = 0)
	    {
		    return Location;
	    }
	    public override VisNode ClosestAnchor()
	    {
		    return this;
	    }
	    public void SetLocation(VisPoint pt)
	    {
		    var node = Reference.NodeFor(pt);
		    Shift = node.Shift;
		    Offset = node.Offset;
        }
        public override void AddOffset(float x, float y)
	    {
		    var newPt = Location.ClonePoint();
		    newPt.AddOffset(x, y);
		    var node = Reference.NodeFor(newPt);
		    Shift = node.Shift;
		    Offset = node.Offset;
	    }


        public OffsetNode CloneTipNode()
	    {
		    return new OffsetNode(Reference, Shift, Offset);
	    }
	    public override object Clone()
	    {
		    return new OffsetNode(Reference, Shift, Offset);
	    }
	    public override string ToString()
	    {
		    return string.Format("offsetNode:{0:0.##},{1:0.##} o{2:0.##}", Location.X, Location.Y, Offset);
	    }
    }

}
