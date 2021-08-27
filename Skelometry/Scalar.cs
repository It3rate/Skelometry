namespace Skelometry
{
    using System;

    public class Scalar : SegmentPair
    {
	    private int UnitIndex = 0;
	    private int MeasureIndex = 1;

	    public float Scale => Segments[MeasureIndex].Magnitude / Segments[UnitIndex].Magnitude;
	    public float Reciprocal => Segments[UnitIndex].Magnitude / Segments[MeasureIndex].Magnitude;
    }
}
