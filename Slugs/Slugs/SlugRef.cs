namespace Slugs.Slugs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    // Slugs need to exist inside a single DataMap, and links are from map slug to map slug.
	public readonly struct SlugRef
	{
		public static readonly SlugRef Empty = new SlugRef(-1, -1, -1);
		public int PadIndex { get; }
		public int DataMapIndex { get; }
		public int SlugIndex { get; }

		public SlugRef(int padIndex, int dataMapIndex, int slugIndex)
		{
			PadIndex = padIndex;
			DataMapIndex = dataMapIndex;
			SlugIndex = slugIndex;
		}

		public bool IsEmpty => DataMapIndex == -1 && SlugIndex == -1;
	}

	public readonly struct SlugInteraction // Linkage
	{
        // need to be able to map to an already scaled linkage (Data/slug/slug) as well as regular Data/slug mapping
        // maybe this happens automatically when referencing a slug map that is already linked to something.
        // Need the concept of not passing updates of an instance to the class (a big dog doesn't affect the default dog, or more accurately only affects it slightly).
		public int SlugMapSource { get; }
		public int SlugMapTarget { get; }
        public double Ratio { get; }

		public SlugInteraction(int slugMapSource, int slugMapTarget, double ratio)
		{
			SlugMapSource = slugMapSource;
			SlugMapTarget = slugMapTarget;
			Ratio = ratio;
		}
	}
}