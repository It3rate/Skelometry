namespace Slugs.Pads
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public readonly struct PointRef : IEquatable<PointRef>
    {
	    public static readonly PointRef Empty = new PointRef(-1, -1, -1);
	    public readonly int PadIndex;
	    public readonly int LineIndex;
	    public readonly int PointIndex;
	    public PointRef(int padIndex, int lineIndex, int pointIndex)
	    {
		    PadIndex = padIndex;
		    LineIndex = lineIndex;
		    PointIndex = pointIndex;
	    }

	    public bool IsEmpty => PadIndex == -1 && LineIndex == -1 && PointIndex == -1;

	    public static bool operator ==(PointRef left, PointRef right) => left.PadIndex == right.PadIndex && left.LineIndex == right.LineIndex && left.PointIndex == right.PointIndex;
	    public static bool operator !=(PointRef left, PointRef right) => left.PadIndex != right.PadIndex || left.LineIndex != right.LineIndex || left.PointIndex != right.PointIndex;
	    public override bool Equals(object obj) => obj is PointRef complex && this == complex;
	    public bool Equals(PointRef value) => this.PadIndex.Equals(value.PadIndex) && this.LineIndex.Equals(value.LineIndex) && this.PointIndex.Equals(value.PointIndex);
	    public override int GetHashCode() => 17 * 23 + PadIndex.GetHashCode() * 23 + LineIndex.GetHashCode() * 23 + PointIndex.GetHashCode();
    }
}
