using Slugs.Entities;

namespace Slugs.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IConstraint
    {
	    IElement StartElement { get; }
	    IElement EndElement { get; }
        bool HasElement(int key);
	    ConstraintTarget ConstraintTarget { get; }

	    void OnElementChanged(IElement changedElement);
    }
}
