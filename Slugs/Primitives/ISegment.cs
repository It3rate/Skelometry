using System;

namespace Slugs.Primitives
{
	public interface ISegment : IEquatable<ISegment>
    {
		double Imaginary { get; set; } // should be able to make these int and everything rational
		double Real { get; set; }
		float End { get; }
		float Start { get; }
		double Length { get; }
		bool IsForward { get; }
		Slug Clone();
		double DirectedLength();
		double AbsLength();
		bool Equals(object obj);
		bool Equals(Slug value);
		int GetHashCode();
		string ToString();
	}
}