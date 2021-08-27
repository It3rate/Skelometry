namespace Skelometry
{
    using System;

    public class SegmentPair
    {
	    protected Segment[] Segments;
	    public Segment A => Segments[0];
	    public Segment B => Segments[1];
    }
}
