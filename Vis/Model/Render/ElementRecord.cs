using System.Runtime.Remoting.Messaging;
using Vis.Model.Agent;
using Vis.Model.Primitives;
using Vis.Model.Render;

namespace Vis.Model.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ElementRecord
    {
	    public int Index { get; }

        public IElement Element { get; }
        public bool IsStroke => Element is VisStroke;

        public PadKind PadKind { get; set; }
	    public ElementType ElementType { get; set; } = ElementType.None;
        public ElementStyle ElementStyle { get; set; } = ElementStyle.None;
	    public ElementState ElementState { get; set; } = ElementState.None;
	    public ElementLinkage ElementLinkage { get; set; } = ElementLinkage.HasUnit;

	    public ElementRecord(IElement element, PadKind padKind, int index = -1)
	    {
		    Element = element;
		    PadKind = padKind;
		    Index = index;
	    }
    }
}
