using System;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public class Focal : IElement, IEquatable<Focal>
    {
	    public ElementKind ElementKind => ElementKind.Focal;

        public static Focal Empty = new Focal();
        public bool IsEmpty => Key == -1;

	    private static int _counter = 1;
	    public PadKind PadKind { get; private set; }
        public int Key { get; set; }

        public Slug Range { get; }
	    public float Focus { get; set; }
	    public PointKind Kind { get; set; }

        public float T => Range.IsZeroLength ? 0 : (float)(Range.Length() / Focus + Range.Start);

        private Focal() {Key = -1; Range = Slug.Empty;}
        //public Focal(int padKind, float focus) : this(padKind, focus, Slug.Unit){}
	    public Focal(PadKind padKind, float focus, Slug range)
	    {
		    Key = _counter++;
		    PadKind = padKind;
		    Kind = PointKind.Terminal;
		    Focus = focus;
		    Range = range;
        }

	    public static bool operator ==(Focal left, Focal right) => left.Key == right.Key;
	    public static bool operator !=(Focal left, Focal right) => left.Key != right.Key;
	    public override bool Equals(object obj) => obj is Focal value && this == value;
	    public bool Equals(Focal value) => this == value;
	    public override int GetHashCode() => Key.GetHashCode();
    }
}
