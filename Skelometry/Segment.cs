namespace Skelometry
{
    using System;

    // A line with start and end. Has a length but isn't measurable without a reference or comparison.
    // Segments are the primitives for all quantification and calibration, but a single segment still gives you nothing past on and off.
    public class Segment : Ray
    {
	    protected float End = 1;
	    public float Magnitude => End - Start;
    }
}
