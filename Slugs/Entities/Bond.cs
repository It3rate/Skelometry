
using System.Collections.Generic;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public class Bond : ElementBase
    {
	    public override ElementKind ElementKind => ElementKind.Bond;
        public override IElement EmptyElement => Empty;
        public static readonly Bond Empty = new Bond();

        public override List<IPoint> Points => new List<IPoint>();

        public int StartFocalKey { get; }
	    public Slug StartSlug { get; }
	    public int EndFocalKey { get; }
	    public Slug EndSlug { get; }

	    private Bond() : base(true) // Empty ctor
	    {
		    StartFocalKey = -1;
            StartSlug = Slug.Empty;
		    EndFocalKey = -1;
            EndSlug = Slug.Empty;
	    }
	    public Bond(Trait startTrait, Slug startSlug, Trait endTrait, Slug endSlug) : base(startTrait.PadKind)
	    {
		    StartFocalKey = startTrait.Key;
		    StartSlug = startSlug;
		    EndFocalKey = endTrait.Key;
		    EndSlug = endSlug;
	    }
	    public static bool operator ==(Bond left, Bond right) => 
		    left.Key == right.Key && left.StartFocalKey == right.StartFocalKey && left.EndFocalKey == right.EndFocalKey && left.StartSlug == right.StartSlug && left.EndSlug == right.EndSlug;
	    public static bool operator !=(Bond left, Bond right) => 
		    left.Key != right.Key || left.StartFocalKey != right.StartFocalKey || left.EndFocalKey != right.EndFocalKey || left.StartSlug != right.StartSlug || left.EndSlug != right.EndSlug;
	    public override bool Equals(object obj) => obj is Bond value && this == value;
	    public override int GetHashCode() => Key.GetHashCode() + 17 * Key + 23 * StartFocalKey + 27 * EndFocalKey + StartSlug.GetHashCode() + EndSlug.GetHashCode();
    }
}
