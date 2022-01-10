using System.Globalization;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;

namespace Slugs.Slugs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Numerics;

    public struct Slug : IElement, IEquatable<Slug>
    {
        // This should all be done with ints, setting a unit size abs(push-pull), and a max value. I think.

        public static readonly Slug Empty = new Slug(0.0, 1.0, true); // instead of empty, perhaps the fall back slug is always Unit?

        public static readonly Slug Zero = new Slug(0.0, 0.0);
	    public static readonly Slug Unit = new Slug(0.0, 1.0);
	    public static readonly Slug Unot = new Slug(1.0, 0.0);
	    public static readonly Slug Unil = new Slug(-0.5, 0.5);
	    public static readonly Slug Max = new Slug(double.MaxValue, double.MaxValue);
	    public static readonly Slug Min = new Slug(double.MinValue, double.MinValue);

        public double Pull { get; } // should be able to make these int and everything rational
	    public double Push { get; }
	    private readonly bool _hasValue;

	    public float Natural => (float)Push;
	    public float Complex => (float)Pull;
	    public float End => (float)Push;
	    public float Start => (float)Pull;

        public Slug(int pull, int push)
	    {
		    _hasValue = true;
		    Pull = pull;
		    Push = push;
	    }
        public Slug(double pull, double push)
	    {
		    _hasValue = true;
            Pull = pull;
		    Push = push;
	    }
	    public Slug(Slug value)
	    {
		    _hasValue = true;
            Pull = value.Pull;
		    Push = value.Push;
	    }

	    private Slug(double pull, double push, bool isEmpty) // empty ctor
	    {
		    _hasValue = !isEmpty;
		    Pull = pull;
		    Push = push;
	    }

        public bool IsEmpty => _hasValue;

	    public Slug Clone() => new Slug(Pull, Push);

	    public double Length() => Slug.Length(this);
        public double AbsLength() => Slug.AbsLength(this);
        public Slug Conjugate() => Slug.Conjugate(this);
        public Slug Reciprocal() => Slug.Reciprocal(this);
        public Slug Square() => Slug.Square(this);
        public Slug Normalize() => Slug.Normalize(this);
        public Slug NormalizeTo(Slug value) => Slug.NormalizeTo(this, value);

        public bool IsZero => Push == 0 && Pull == 0;
	    public bool IsZeroLength => AbsLength() == 0;
	    public bool IsForward => Push >= Pull;
	    public double Direction => Push >= Pull ? 1.0 : -1.0;

        // because everything is segments, can add 'prepositions' (before, after, between, entering, leaving, near etc)
        public bool IsWithin(Slug value) => Pull >= value.Pull && Push <= value.Push; // todo: account for line direction
        public bool IsBetween(Slug value) => Pull > value.Pull && Push < value.Push;
        public bool IsBefore(Slug value) => Pull < value.Pull && Push < value.Pull;
        public bool IsAfter(Slug value) => Pull > value.Push && Push > value.Push;
        public bool IsBeginning(Slug value) => Pull <= value.Pull && Push > value.Pull;
        public bool IsEnding(Slug value) => Pull >= value.Push && Push > value.Push;
        public bool IsTouching(Slug value) => (Pull >= value.Pull && Pull <= value.Push) || (Push >= value.Pull && Push <= value.Push);
        public bool IsNotTouching(Slug value) => !IsTouching(value);



        public static Slug operator -(Slug value) => new Slug(-value.Pull, -value.Push);

        public static Slug operator +(Slug a, double value) => new Slug(a.Pull + value, a.Push + value);
        public static Slug operator -(Slug a, double value) => new Slug(a.Pull - value, a.Push - value);
        public static Slug operator *(Slug a, double value) => new Slug(a.Pull * value, a.Push * value);
        public static Slug operator /(Slug a, double value) => new Slug(value == 0 ? double.MaxValue : a.Pull / value, value == 0 ? double.MaxValue : a.Push / value);

        public static Slug operator +(Slug a, Slug b) => new Slug(a.Pull + b.Pull, b.Pull + b.Push);
        public static Slug operator -(Slug a, Slug b) =>  new Slug(a.Pull - b.Pull, b.Pull - b.Push);
        public static Slug operator *(Slug a, Slug b) => new Slug(a.Pull * b.Pull - a.Push * b.Push, a.Pull * b.Push + a.Push * b.Pull);
        public static Slug operator /(Slug a, Slug b)
        {
            var acbd = a.Pull * b.Push + a.Push * b.Pull;
            var bc_ad = a.Push * b.Pull - a.Pull * b.Push;
            var c2d2 = b.Pull * b.Pull + b.Push * b.Push;
            return c2d2 == 0 ? Max : new Slug(acbd/c2d2, bc_ad/c2d2);
        }

        // Need to decide if pull's positive points left or not. Probably does, but this will affect other calculations.
        public static double Length(Slug value) => value.Push - -value.Pull;
        public static double AbsLength(Slug value) => Math.Abs(value.Push - -value.Pull);
        public static Slug Conjugate(Slug a) => new Slug(a.Push, -a.Pull);
        public static Slug Reciprocal(Slug value) => value.Push == 0.0 && value.Pull == 0.0 ? Slug.Zero : Slug.Unit / value;
        public static Slug Square(Slug a) => new Slug(a.Pull * a.Pull + (a.Push * a.Push) * -1, 0); // value * value;
        
        public static Slug Normalize(Slug value)
        {
            // should be divide by self?
            return value.IsZeroLength ? new Slug(0.5, 0.5) : value / value;
	        //Slug result;
	        //if (value.IsZeroLength)
	        //{
		       // result = value.IsForward ? new Slug(0.5, 0.5) : new Slug(-0.5, -0.5); // (-0.5, 0.5); // (0, 1.0);
	        //}
	        //else
	        //{
		       // double scale = 1.0 / value.AbsLength();
		       // result = new Slug(value.Pull * scale, value.Push * scale);
	        //}

	        //return result;
        }
        public static Slug NormalizeTo(Slug from, Slug to)
        {
	        return from.Normalize() * to;
	        //var norm = Normalize(value);
	        //var scale = value.Length();
	        //var result = new Slug(norm.Pull * scale, norm.Push * scale);

	        //var offset = value.Pull - result.Pull;
	        //result.Offset((int)offset); // not sure if measure needs to be offset or multiplied here, probably everything needs to be wedged etc.
	        //return result;
        }

        public static bool operator ==(Slug left, Slug right) => left.Push == right.Push && left.Pull == right.Pull;
        public static bool operator !=(Slug left, Slug right) => left.Push != right.Push || left.Pull != right.Pull;
        public override bool Equals(object obj) => obj is Slug complex && this == complex;
        public bool Equals(Slug value) => this.Push.Equals(value.Push) && this.Pull.Equals(value.Pull);
        public override int GetHashCode()
        {
	        int num = 99999997;
	        return Push.GetHashCode() % num ^ Pull.GetHashCode();
        }
        public override string ToString() => string.Format((IFormatProvider)CultureInfo.CurrentCulture, "({0}, {1})", new object[2]
        {
	        (object) this.Pull,
	        (object) this.Push
        });
    }
}
