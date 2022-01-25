using System;
using Slugs.Pads;
using Slugs.Primitives;

namespace Slugs.Entities
{
	public class Focal : ElementBase, IEquatable<Focal>
    {
	    public override ElementKind ElementKind => ElementKind.Focal;
	    public override IElement EmptyElement => Empty;
	    public static readonly Focal Empty = new Focal();
        private Focal() : base(true) {Range = Slug.Empty;}

        public Slug Range { get; }
	    public float Focus { get; set; }
        public float T => Range.IsZeroLength ? 0 : (float)(Range.Length() / Focus + Range.Start);

        public override IPoint[] Points => new IPoint[] { };

        //public Focal(int padKind, float focus) : this(padKind, focus, Slug.Unit){}
	    public Focal(PadKind padKind, float focus, Slug range) : base(padKind)
	    {
		    //SelectionKind = PointKind.Terminal;
		    Focus = focus;
		    Range = range;
        }

	    public static bool operator ==(Focal left, Focal right) => left.Key == right.Key && left.Range == right.Range && left.Focus == right.Focus;
        public static bool operator !=(Focal left, Focal right) => left.Key != right.Key || left.Range == right.Range || left.Focus == right.Focus;
	    public override bool Equals(object obj) => obj is Focal value && this == value;
	    public bool Equals(Focal value) => this == value;
	    public override int GetHashCode() => Key.GetHashCode() + 17*Range.GetHashCode() + 23*Focus.GetHashCode();
    }
}
