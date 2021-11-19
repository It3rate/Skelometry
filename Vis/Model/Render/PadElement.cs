using Vis.Model.Agent;
using Vis.Model.Render;

namespace Vis.Model.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PadAttributes
    {
        public static readonly PadAttributes Empty = new PadAttributes();

	    public int Index { get; }
	    public PadKind PadKind { get; set; } = PadKind.None;
	    public ElementType ElementType { get; set; } = ElementType.None;
        public ElementStyle ElementStyle { get; set; } = ElementStyle.None;
	    public ElementState ElementState { get; set; } = ElementState.None;
	    public ElementLinkage ElementLinkage { get; set; } = ElementLinkage.HasUnit;

	    public PadAttributes(int index = -1)
	    {
		    Index = index;
	    }
    }

    public class PadAttributes<T> : PadAttributes //where T : IPrimitive
    {
	    public T Element { get; }

	    public PadAttributes(T element, int index = -1) : base(index)
	    {
		    Element = element;
	    }
    }

}
