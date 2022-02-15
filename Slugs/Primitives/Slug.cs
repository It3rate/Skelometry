using System;
using System.Globalization;

namespace Slugs.Primitives
{
	public struct Slug : IEquatable<Slug>
    {
        // This should all be done with ints, setting a unit size abs(fore-aft), and a max value. I think.

        public static readonly Slug Empty = new Slug(0.0, 1.0, true); // instead of empty, perhaps the fall back slug is always Unit?

        public static readonly Slug Zero = new Slug(0.0, 0.0);
	    public static readonly Slug Unit = new Slug(0.0, 1.0);
	    public static readonly Slug Unot = new Slug(1.0, 0.0);
	    public static readonly Slug Unil = new Slug(-0.5, 0.5);
	    public static readonly Slug Max = new Slug(double.MaxValue, double.MaxValue);
	    public static readonly Slug Min = new Slug(double.MinValue, double.MinValue);

        public double Aft { get; set; } // should be able to make these int and everything rational
	    public double Fore { get; set; }
	    private readonly bool _hasValue;

	    public float Natural => (float)Fore;
	    public float Complex => (float)Aft;
	    public float End => (float)Fore;
	    public float Start => (float)Aft;

        public Slug(int aft, int fore)
	    {
		    _hasValue = true;
		    Aft = aft;
		    Fore = fore;
	    }
        public Slug(double aft, double fore)
	    {
		    _hasValue = true;
            Aft = aft;
		    Fore = fore;
	    }
	    public Slug(Slug value)
	    {
		    _hasValue = true;
            Aft = value.Aft;
		    Fore = value.Fore;
	    }

	    private Slug(double aft, double fore, bool isEmpty) // empty ctor
	    {
		    _hasValue = !isEmpty;
		    Aft = aft;
		    Fore = fore;
	    }

        public bool IsEmpty => _hasValue;

	    public Slug Clone() => new Slug(Aft, Fore);

	    public double Length => Slug.DirectedLength(this);
        public double AbsLength() => Slug.AbsLength(this);
        public Slug Conjugate() => Slug.Conjugate(this);
        public Slug Reciprocal() => Slug.Reciprocal(this);
        public Slug Square() => Slug.Square(this);
        public Slug Normalize() => Slug.Normalize(this);
        public Slug NormalizeTo(Slug value) => Slug.NormalizeTo(this, value);

        public bool IsZero => Fore == 0 && Aft == 0;
        public bool IsZeroLength => (Fore == Aft);
	    public bool IsForward => Fore >= Aft;
	    public double Direction => Fore >= Aft ? 1.0 : -1.0;

        // because everything is segments, can add 'prepositions' (before, after, between, entering, leaving, near etc)
        public bool IsWithin(Slug value) => Aft >= value.Aft && Fore <= value.Fore; 
        public bool IsBetween(Slug value) => Aft > value.Aft && Fore < value.Fore;
        public bool IsBefore(Slug value) => Aft < value.Aft && Fore < value.Aft;
        public bool IsAfter(Slug value) => Aft > value.Fore && Fore > value.Fore;
        public bool IsBeginning(Slug value) => Aft <= value.Aft && Fore > value.Aft;
        public bool IsEnding(Slug value) => Aft >= value.Fore && Fore > value.Fore;
        public bool IsTouching(Slug value) => (Aft >= value.Aft && Aft <= value.Fore) || (Fore >= value.Aft && Fore <= value.Fore);
        public bool IsNotTouching(Slug value) => !IsTouching(value);



        public static Slug operator -(Slug value) => new Slug(-value.Aft, -value.Fore);

        public static Slug operator +(Slug a, double value) => new Slug(a.Aft + value, a.Fore + value);
        public static Slug operator -(Slug a, double value) => new Slug(a.Aft - value, a.Fore - value);
        public static Slug operator *(Slug a, double value) => new Slug(a.Aft * value, a.Fore * value);
        public static Slug operator /(Slug a, double value) => new Slug(value == 0 ? double.MaxValue : a.Aft / value, value == 0 ? double.MaxValue : a.Fore / value);

        public static Slug operator +(Slug a, Slug b) => new Slug(a.Aft + b.Aft, b.Aft + b.Fore);
        public static Slug operator -(Slug a, Slug b) =>  new Slug(a.Aft - b.Aft, b.Aft - b.Fore);
        public static Slug operator *(Slug a, Slug b) => new Slug(a.Aft * b.Aft - a.Fore * b.Fore, a.Aft * b.Fore + a.Fore * b.Aft);
        public static Slug operator /(Slug a, Slug b)
        {
            var acbd = a.Aft * b.Fore + a.Fore * b.Aft;
            var bc_ad = a.Fore * b.Aft - a.Aft * b.Fore;
            var c2d2 = b.Aft * b.Aft + b.Fore * b.Fore;
            return c2d2 == 0 ? Max : new Slug(acbd/c2d2, bc_ad/c2d2);
        }

        // Need to decide if aft's positive points left or not. Probably does, but this will affect other calculations.
        public static double DirectedLength(Slug value) => value.Fore - -value.Aft;
        public static double AbsLength(Slug value) => Math.Abs(value.Fore - -value.Aft);
        public static Slug Conjugate(Slug a) => new Slug(a.Fore, -a.Aft);
        public static Slug Reciprocal(Slug value) => value.Fore == 0.0 && value.Aft == 0.0 ? Slug.Zero : Slug.Unit / value;
        public static Slug Square(Slug a) => new Slug(a.Aft * a.Aft + (a.Fore * a.Fore) * -1, 0); // value * value;
        
        public static Slug Normalize(Slug value)
        {
            // should be divide by self?
            return value.IsZeroLength ? new Slug(0.5, 0.5) : value / value;
	        //Range result;
	        //if (value.IsZeroLength)
	        //{
		       // result = value.IsForward ? new Range(0.5, 0.5) : new Range(-0.5, -0.5); // (-0.5, 0.5); // (0, 1.0);
	        //}
	        //else
	        //{
		       // double scale = 1.0 / value.AbsLength();
		       // result = new Range(value.Aft * scale, value.Fore * scale);
	        //}

	        //return result;
        }
        public static Slug NormalizeTo(Slug from, Slug to)
        {
	        return from.Normalize() * to;
	        //var norm = Normalize(value);
	        //var scale = value.Length();
	        //var result = new Range(norm.Aft * scale, norm.Fore * scale);

	        //var offset = value.Aft - result.Aft;
	        //result.Offset((int)offset); // not sure if measure needs to be offset or multiplied here, probably everything needs to be wedged etc.
	        //return result;
        }

        public static bool operator ==(Slug left, Slug right) => left.Fore == right.Fore && left.Aft == right.Aft;
        public static bool operator !=(Slug left, Slug right) => left.Fore != right.Fore || left.Aft != right.Aft;
        public override bool Equals(object obj) => obj is Slug slug && this == slug;
        public bool Equals(Slug value) => this.Fore.Equals(value.Fore) && this.Aft.Equals(value.Aft);
        public override int GetHashCode() => Fore.GetHashCode() % 99999997 ^ Aft.GetHashCode();
        
        public override string ToString() => string.Format((IFormatProvider)CultureInfo.CurrentCulture, "({0}, {1})", new object[2]
        {
	        (object) this.Aft,
	        (object) this.Fore
        });
    }
}
