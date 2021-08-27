namespace Skelometry
{
    using System;

    public class Segment : Ray
    {
	    protected float End = 1;
	    public float Magnitude => End - Start;
    }
}
