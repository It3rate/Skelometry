namespace Skelometry
{
    using System;

    // Contains one intersection (looks like a point to the line). This gives information about forwards and backwards (positive/negative).
    // Can be a line in one direction with the other only extrapolated.
    public class Ray : Line
    {
	    protected float Start = 0;
	    public bool IsForward = true;
    }
}
