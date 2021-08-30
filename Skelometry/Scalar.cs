namespace Skelometry
{
    using System;

    // A segment pair with the ability to stretch and shrink (multiply and divide).
    // Can now do any type of decimal, and compare lines segment pairs with different/arbitrary unit sizes.
    // Represents all real numbers, can be continuous.
    // Still in 1D.
    public class Scalar : SegmentPair
    {
	    private int UnitIndex = 0;
	    private int MeasureIndex = 1;

	    public float Scale => Segments[MeasureIndex].Magnitude / Segments[UnitIndex].Magnitude;
	    public float Reciprocal => Segments[UnitIndex].Magnitude / Segments[MeasureIndex].Magnitude;
    }
}
