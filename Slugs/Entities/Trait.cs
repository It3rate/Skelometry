namespace Slugs.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Trait
    {
	    public SegRef SegRef { get; }
	    public int TraitKind { get; }
        // Trait default properties]

	    public Trait(SegRef segRef, int traitKind)
	    {
		    SegRef = segRef;
		    TraitKind = traitKind;
	    }
    }
}
