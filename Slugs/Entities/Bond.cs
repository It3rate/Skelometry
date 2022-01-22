
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public class Bond : ElementBase
    {
	    public new ElementKind ElementKind => ElementKind.Bond;
        public new IElement EmptyElement => Empty;
        public static readonly Bond Empty = new Bond();

	    public int StartTraitKey { get; }
	    public Slug StartSlug { get; }
	    public int EndTraitKey { get; }
	    public Slug EndSlug { get; }

	    private Bond() : base(true) // Empty ctor
	    {
		    StartTraitKey = -1;
            StartSlug = Slug.Empty;
		    EndTraitKey = -1;
            EndSlug = Slug.Empty;
	    }
	    public Bond(Trait startTrait, Slug startSlug, Trait endTrait, Slug endSlug) : base(startTrait.PadKind)
	    {
		    StartTraitKey = startTrait.Key;
		    StartSlug = startSlug;
		    EndTraitKey = endTrait.Key;
		    EndSlug = endSlug;
	    }
    }
}
