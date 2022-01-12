using Slugs.Slugs;

namespace Slugs.Entities
{
	public readonly struct Bond
    {
        public static readonly Bond Empty = new Bond(-2);
        public bool IsEmpty => StartIndex == -2;

	    public int StartIndex { get; }
	    public Slug StartSlug { get; }
	    public int EndIndex { get; }
	    public Slug EndSlug { get; }

	    private Bond(int index) // Empty ctor
	    {
		    StartIndex = index;
            StartSlug = Slug.Empty;
		    EndIndex = index;
            EndSlug = Slug.Empty;
	    }
	    public Bond(int startIndex, Slug startSlug, int endIndex, Slug endSlug)
	    {
		    StartIndex = startIndex;
		    StartSlug = startSlug;
		    EndIndex = endIndex;
		    EndSlug = endSlug;
	    }
    }
}
