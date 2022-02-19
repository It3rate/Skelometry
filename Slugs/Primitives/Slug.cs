using System;
using System.Globalization;
using System.Numerics;

namespace Slugs.Primitives
{
	public struct Slug : IEquatable<Slug>
    {
        // This should all be done with ints, setting a unit size abs(real-img), and a max value. I think.

        public static readonly Slug Empty = new Slug(0.0, 1.0, true); // instead of empty, perhaps the fall back slug is always UnitSlug?

        public static readonly Slug Zero = new Slug(0.0, 0.0);
	    public static readonly Slug Unit = new Slug(0.0, 1.0);
	    public static readonly Slug Unot = new Slug(1.0, 0.0);
	    public static readonly Slug Unil = new Slug(-0.5, 0.5);
	    public static readonly Slug Max = new Slug(double.MaxValue, double.MaxValue);
	    public static readonly Slug Min = new Slug(double.MinValue, double.MinValue);

        public double Img { get; set; } // should be able to make these int and everything rational
	    public double Real { get; set; }
	    private readonly bool _hasValue;

	    //public float Natural => (float)Real;
	    //public float Complex => (float)Img;
	    public float End => (float)Real;
	    public float Start => (float)Img;
        public Complex Complex => new Complex(Real, Img);

        public Slug(int img, int real)
	    {
		    _hasValue = true;
		    Img = img;
		    Real = real;
	    }
        public Slug(double img, double real)
	    {
		    _hasValue = true;
            Img = img;
		    Real = real;
	    }
	    public Slug(Slug value)
	    {
		    _hasValue = true;
            Img = value.Img;
		    Real = value.Real;
	    }

	    private Slug(double img, double real, bool isEmpty) // empty ctor
	    {
		    _hasValue = !isEmpty;
		    Img = img;
		    Real = real;
	    }

        public bool IsEmpty => _hasValue;

	    public Slug Clone() => new Slug(Img, Real);

	    public double Length => Slug.AbsLength(this);
	    public double DirectedLength() => Slug.DirectedLength(this);
        public double AbsLength() => Slug.AbsLength(this);
        public Slug Conjugate() => Slug.Conjugate(this);
        public Slug Reciprocal() => Slug.Reciprocal(this);
        public Slug Square() => Slug.Square(this);
        public Slug Normalize() => Slug.Normalize(this);
        public Slug NormalizeTo(Slug value) => Slug.NormalizeTo(this, value);

        public bool IsZero => Real == 0 && Img == 0;
        public bool IsZeroLength => (Real == Img);
	    public bool IsForward => Real >= Img;
	    public double Direction => Real >= Img ? 1.0 : -1.0;

        // because everything is segments, can add 'prepositions' (before, after, between, entering, leaving, near etc)
        public bool IsWithin(Slug value) => Img >= value.Img && Real <= value.Real; 
        public bool IsBetween(Slug value) => Img > value.Img && Real < value.Real;
        public bool IsBefore(Slug value) => Img < value.Img && Real < value.Img;
        public bool IsAfter(Slug value) => Img > value.Real && Real > value.Real;
        public bool IsBeginning(Slug value) => Img <= value.Img && Real > value.Img;
        public bool IsEnding(Slug value) => Img >= value.Real && Real > value.Real;
        public bool IsTouching(Slug value) => (Img >= value.Img && Img <= value.Real) || (Real >= value.Img && Real <= value.Real);
        public bool IsNotTouching(Slug value) => !IsTouching(value);




        public static Slug operator +(Slug a, double value) => new Slug(a.Img + value, a.Real + value);
        public static Slug operator -(Slug a, double value) => new Slug(a.Img - value, a.Real - value);
        public static Slug operator *(Slug a, double value) => new Slug(a.Img * value, a.Real * value);
        public static Slug operator /(Slug a, double value) => new Slug(value == 0 ? double.MaxValue : a.Img / value, value == 0 ? double.MaxValue : a.Real / value);

        //public static Slug operator -(Slug value) => new Slug(-value.Img, -value.Real);
        //public static Slug operator +(Slug a, Slug b) => new Slug(a.Img + b.Img, b.Img + b.Real);
        //public static Slug operator -(Slug a, Slug b) =>  new Slug(a.Img - b.Img, b.Img - b.Real);
        ////public static Slug operator *(Slug a, Slug b) => new Slug(a.Img * b.Img - a.Real * b.Real, a.Img * b.Real + a.Real * b.Img);
        //// ac + adi + bci + bdi2
        //public static Slug operator *(Slug a, Slug b) => new Slug(a.Real * b.Real + a.Real * b.Img, b.Img * b.Real + a.Img * b.Img);
        //public static Slug operator /(Slug a, Slug b)
        //{
        //    var acbd = a.Img * b.Real + a.Real * b.Img;
        //    var bc_ad = a.Real * b.Img - a.Img * b.Real;
        //    var c2d2 = b.Img * b.Img + b.Real * b.Real;
        //    return c2d2 == 0 ? Max : new Slug(acbd/c2d2, bc_ad/c2d2);
        //}


        public static Slug operator -(Slug value) => new Slug(-value.Real, -value.Img);
        public static Slug operator +(Slug left, Slug right) => new Slug(left.Real + right.Real, left.Img + right.Img);
        public static Slug operator -(Slug left, Slug right) => new Slug(left.Real - right.Real, left.Img - right.Img);
        public static Slug operator *(Slug left, Slug right) => new Slug(left.Real * right.Real - left.Img * right.Img, left.Img * right.Real + left.Real * right.Img);
        public static Slug operator /(Slug left, Slug right)
        {
            double real1 = left.Real;
            double imaginary1 = left.Img;
            double real2 = right.Real;
            double imaginary2 = right.Img;
            if (Math.Abs(imaginary2) < Math.Abs(real2))
            {
                double num = imaginary2 / real2;
                return new Slug((real1 + imaginary1 * num) / (real2 + imaginary2 * num), (imaginary1 - real1 * num) / (real2 + imaginary2 * num));
            }
            double num1 = real2 / imaginary2;
            return new Slug((imaginary1 + real1 * num1) / (imaginary2 + real2 * num1), (-real1 + imaginary1 * num1) / (imaginary2 + real2 * num1));
        }




        // Need to decide if img's positive points left or not. Probably does, but this will affect other calculations.
        public static double DirectedLength(Slug value) => value.Real - value.Img;
        public static double AbsLength(Slug value) => Math.Abs(value.Real - value.Img);
        public static Slug Conjugate(Slug a) => new Slug(a.Real, -a.Img);
        public static Slug Reciprocal(Slug value) => value.Real == 0.0 && value.Img == 0.0 ? Slug.Zero : Slug.Unit / value;
        public static Slug Square(Slug a) => new Slug(a.Img * a.Img + (a.Real * a.Real) * -1, 0); // value * value;
        
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
		       // result = new Range(value.Img * scale, value.Real * scale);
	        //}

	        //return result;
        }
        public static Slug NormalizeTo(Slug from, Slug to)
        {
	        return from.Normalize() * to;
	        //var norm = Normalize(value);
	        //var scale = value.Length();
	        //var result = new Range(norm.Img * scale, norm.Real * scale);

	        //var offset = value.Img - result.Img;
	        //result.Offset((int)offset); // not sure if measure needs to be offset or multiplied here, probably everything needs to be wedged etc.
	        //return result;
        }

        public static bool operator ==(Slug left, Slug right) => left.Real == right.Real && left.Img == right.Img;
        public static bool operator !=(Slug left, Slug right) => left.Real != right.Real || left.Img != right.Img;
        public override bool Equals(object obj) => obj is Slug slug && this == slug;
        public bool Equals(Slug value) => this.Real.Equals(value.Real) && this.Img.Equals(value.Img);
        public override int GetHashCode() => Real.GetHashCode() % 99999997 ^ Img.GetHashCode();
        
        public override string ToString() => string.Format((IFormatProvider)CultureInfo.CurrentCulture, "({0}, {1})", new object[2]
        {
	        (object) this.Img,
	        (object) this.Real
        });
    }
}
