using System;
using System.Globalization;
using System.Numerics;

namespace Slugs.Primitives
{
	public struct Slug : IEquatable<Slug>
    {
        // This should all be done with ints, setting a unit size abs(real-imaginary), and a max value. I think.

        public static readonly Slug Empty = new Slug(0.0, 1.0, true); // instead of empty, perhaps the fall back slug is always UnitSlug?

        public static readonly Slug Zero = new Slug(0.0, 0.0);
	    public static readonly Slug Unit = new Slug(0.0, 1.0);
	    public static readonly Slug Unot = new Slug(1.0, 0.0);
	    public static readonly Slug Umid = new Slug(-0.5, 0.5);
	    public static readonly Slug Max = new Slug(double.MaxValue, double.MaxValue);
	    public static readonly Slug Min = new Slug(double.MinValue, double.MinValue);

        public double Imaginary { get; set; } // should be able to make these int and everything rational
	    public double Real { get; set; }
	    private readonly bool _hasValue;

	    //public float Natural => (float)Real;
	    //public float Slug => (float)Imaginary;
	    public float End => (float)Real;
	    public float Start => (float)Imaginary;

        public Slug(int imaginary, int real)
	    {
		    _hasValue = true;
		    Imaginary = imaginary;
		    Real = real;
	    }
        public Slug(double imaginary, double real)
	    {
		    _hasValue = true;
            Imaginary = imaginary;
		    Real = real;
	    }
	    public Slug(Slug value)
	    {
		    _hasValue = true;
            Imaginary = value.Imaginary;
		    Real = value.Real;
	    }

	    private Slug(double imaginary, double real, bool isEmpty) // empty ctor
	    {
		    _hasValue = !isEmpty;
		    Imaginary = imaginary;
		    Real = real;
	    }

        public bool IsEmpty => _hasValue;

	    public Slug Clone() => new Slug(Imaginary, Real);

	    public double Length => Slug.AbsLength(this);
	    public double DirectedLength() => Slug.DirectedLength(this);
        public double AbsLength() => Slug.AbsLength(this);
        public Slug Conjugate() => Slug.Conjugate(this);
        public Slug Reciprocal() => Slug.Reciprocal(this);
        public Slug Square() => Slug.Square(this);
        public Slug Normalize() => Slug.Normalize(this);
        public Slug NormalizeTo(Slug value) => Slug.NormalizeTo(this, value);

        public bool IsZero => Real == 0 && Imaginary == 0;
        public bool IsZeroLength => (Real == Imaginary);
	    public bool IsForward => Real >= Imaginary;
	    public double Direction => Real >= Imaginary ? 1.0 : -1.0;

        // because everything is segments, can add 'prepositions' (before, after, between, entering, leaving, near etc)
        public bool IsWithin(Slug value) => Imaginary >= value.Imaginary && Real <= value.Real; 
        public bool IsBetween(Slug value) => Imaginary > value.Imaginary && Real < value.Real;
        public bool IsBefore(Slug value) => Imaginary < value.Imaginary && Real < value.Imaginary;
        public bool IsAfter(Slug value) => Imaginary > value.Real && Real > value.Real;
        public bool IsBeginning(Slug value) => Imaginary <= value.Imaginary && Real > value.Imaginary;
        public bool IsEnding(Slug value) => Imaginary >= value.Real && Real > value.Real;
        public bool IsTouching(Slug value) => (Imaginary >= value.Imaginary && Imaginary <= value.Real) || (Real >= value.Imaginary && Real <= value.Real);
        public bool IsNotTouching(Slug value) => !IsTouching(value);


        public static Slug operator +(Slug a, double value) => new Slug(a.Real + value, a.Imaginary + value);
        public static Slug operator -(Slug a, double value) => new Slug(a.Real - value, a.Imaginary - value);
        public static Slug operator *(Slug a, double value) => new Slug(a.Real * value, a.Imaginary * value);
        public static Slug operator /(Slug a, double value) => new Slug(value == 0 ? double.MaxValue : a.Real / value, value == 0 ? double.MaxValue : a.Imaginary / value);

        public static Slug Negate(Slug value) => -value;
        public static Slug Add(Slug left, Slug right) => left + right;
        public static Slug Subtract(Slug left, Slug right) => left - right;
        public static Slug Multiply(Slug left, Slug right) => left * right;
        public static Slug Divide(Slug dividend, Slug divisor) => dividend / divisor;

        public static Slug operator -(Slug value) => new Slug(-value.Imaginary, -value.Real);
        public static Slug operator +(Slug left, Slug right) => new Slug(left.Imaginary + right.Imaginary, left.Real + right.Real);
        public static Slug operator -(Slug left, Slug right) => new Slug(left.Imaginary - right.Imaginary, left.Real - right.Real);
        public static Slug operator *(Slug left, Slug right) => new Slug(left.Imaginary * right.Real + left.Real * right.Imaginary, left.Real * right.Real - left.Imaginary * right.Imaginary);
        public static Slug operator /(Slug left, Slug right)
        {
            double real1 = left.Real;
            double imaginary1 = left.Imaginary;
            double real2 = right.Real;
            double imaginary2 = right.Imaginary;
            if (Math.Abs(imaginary2) < Math.Abs(real2))
            {
                double num = imaginary2 / real2;
                return new Slug((imaginary1 - real1 * num) / (real2 + imaginary2 * num), (real1 + imaginary1 * num) / (real2 + imaginary2 * num));
            }
            double num1 = real2 / imaginary2;
            return new Slug((-real1 + imaginary1 * num1) / (imaginary2 + real2 * num1), (imaginary1 + real1 * num1) / (imaginary2 + real2 * num1));
        }

        public static double DirectedLength(Slug value) => value.Real - value.Imaginary;
        public static double AbsLength(Slug value) => Math.Abs(value.Real - value.Imaginary);
        public static Slug Conjugate(Slug a) => new Slug(a.Real, -a.Imaginary);
        public static Slug Reciprocal(Slug value) => value.Real == 0.0 && value.Imaginary == 0.0 ? Slug.Zero : Slug.Unit / value;
        public static Slug Square(Slug a) => new Slug(a.Imaginary * a.Imaginary + (a.Real * a.Real) * -1, 0); // value * value;
        public static Slug Normalize(Slug value) => value.IsZeroLength ? new Slug(0.5, 0.5) : value / value;
        public static Slug NormalizeTo(Slug from, Slug to) =>  from.Normalize() * to;

        public static double Abs(Slug value)
        {
	        if (double.IsInfinity(value.Real) || double.IsInfinity(value.Imaginary))
		        return double.PositiveInfinity;
	        double num1 = Math.Abs(value.Real);
	        double num2 = Math.Abs(value.Imaginary);
	        if (num1 > num2)
	        {
		        double num3 = num2 / num1;
		        return num1 * Math.Sqrt(1.0 + num3 * num3);
	        }
	        if (num2 == 0.0)
		        return num1;
	        double num4 = num1 / num2;
	        return num2 * Math.Sqrt(1.0 + num4 * num4);
        }
        public static Slug Pow(Slug value, Slug power)
        {
	        if (power == Slug.Zero)
		        return Slug.Unit;
	        if (value == Slug.Zero)
		        return Slug.Zero;
	        double real1 = value.Real;
	        double imaginary1 = value.Imaginary;
	        double real2 = power.Real;
	        double imaginary2 = power.Imaginary;
	        double num1 = Slug.Abs(value);
	        double num2 = Math.Atan2(imaginary1, real1);
	        double num3 = real2 * num2 + imaginary2 * Math.Log(num1);
	        double num4 = Math.Pow(num1, real2) * Math.Pow(Math.E, -imaginary2 * num2);
	        return new Slug(num4 * Math.Sin(num3), num4 * Math.Cos(num3));
        }
        public static Slug Pow(Slug value, double power) => Slug.Pow(value, new Slug(0.0, power));
        private static Slug Scale(Slug value, double factor) => new Slug(factor * value.Imaginary, factor * value.Real);

        public static bool operator ==(Slug left, Slug right) => left.Real == right.Real && left.Imaginary == right.Imaginary;
        public static bool operator !=(Slug left, Slug right) => left.Real != right.Real || left.Imaginary != right.Imaginary;
        public override bool Equals(object obj) => obj is Slug slug && this == slug;
        public bool Equals(Slug value) => this.Real.Equals(value.Real) && this.Imaginary.Equals(value.Imaginary);
        public override int GetHashCode() => Real.GetHashCode() % 99999997 ^ Imaginary.GetHashCode();
        
        public override string ToString() => string.Format((IFormatProvider)CultureInfo.CurrentCulture, "({0}, {1})", new object[2]
        {
	        (object) this.Imaginary,
	        (object) this.Real
        });
    }
}
