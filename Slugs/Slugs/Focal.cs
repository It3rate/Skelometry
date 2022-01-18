using Slugs.Entities;

namespace Slugs.Slugs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Focal
    {
        public static Focal Empty = new Focal();
        public bool IsEmpty => Key == -1;

	    private static int _counter = 1;
	    public int PadIndex { get; private set; }
        public int Key { get; set; }

        public Slug Range { get; }
	    public float Focus { get; set; }
	    public PointKind Kind { get; set; }

        public float T => Range.IsZeroLength ? 0 : (float)(Range.Length() / Focus + Range.Start);

        private Focal() {Key = -1; Range = Slug.Empty;}
        //public Focal(int padIndex, float focus) : this(padIndex, focus, Slug.Unit){}
	    public Focal(int padIndex, float focus, Slug range)
	    {
		    Key = _counter++;
		    PadIndex = padIndex;
		    Kind = PointKind.Terminal;
		    Focus = focus;
		    Range = range;
	    }
    }
}
