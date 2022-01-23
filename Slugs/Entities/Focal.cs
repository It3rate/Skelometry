using System;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public class Focal : ElementBase, IEquatable<Focal>
    {
	    public override ElementKind ElementKind => ElementKind.Focal;
	    public override IElement EmptyElement => Empty;
	    public static Focal Empty = new Focal();

        public Slug Range { get; }
	    public float Focus { get; set; }
	    //public PointKind Kind { get; set; }

        public float T => Range.IsZeroLength ? 0 : (float)(Range.Length() / Focus + Range.Start);

        private Focal() : base(PadKind.None) {Range = Slug.Empty;}
        //public Focal(int padKind, float focus) : this(padKind, focus, Slug.Unit){}
	    public Focal(PadKind padKind, float focus, Slug range) : base(padKind)
	    {
		    //Kind = PointKind.Terminal;
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
