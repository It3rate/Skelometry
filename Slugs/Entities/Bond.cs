
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public class Bond : ElementBase
    {
	    public override ElementKind ElementKind => ElementKind.Bond;
        public override IElement EmptyElement => Empty;
        public static readonly Bond Empty = new Bond();

        public override IPoint[] Points => new IPoint[] { };

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
	    public static bool operator ==(Bond left, Bond right) => 
		    left.Key == right.Key && left.StartTraitKey == right.StartTraitKey && left.EndTraitKey == right.EndTraitKey && left.StartSlug == right.StartSlug && left.EndSlug == right.EndSlug;
	    public static bool operator !=(Bond left, Bond right) => 
		    left.Key != right.Key || left.StartTraitKey != right.StartTraitKey || left.EndTraitKey != right.EndTraitKey || left.StartSlug != right.StartSlug || left.EndSlug != right.EndSlug;
	    public override bool Equals(object obj) => obj is Bond value && this == value;
	    public override int GetHashCode() => Key.GetHashCode() + 17 * Key + 23 * StartTraitKey + 27 * EndTraitKey + StartSlug.GetHashCode() + EndSlug.GetHashCode();
    }
}
