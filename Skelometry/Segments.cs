namespace Skelometry
{
    using System;

    // Multiple segments allows counting (number of segments) - next, previous
    // extrapolating beyond line in each direction - before, after (infront, behind)
    // able to compare two lengths, understand equal, greater, less, but not by how much
    // sliding along line for:
    //   Adding: tip to tail - beside
    //   Subtracting: one from other, can be negative result (extrapolated) - into
    // ratios (small to large): compare one to the other, how many fit 4:1 - whole numbers only, or between numbers at best - beside
    // ratios (large to small): One fits how many 1:4
    // can divide and multiply with whole numbers? Fractions, but no stretching.
    // specifically, can't do trancendental numbers, can't be continuous (any continnuous distance implies infinite decimal places).
    public class SegmentPair
    {
	    protected Segment[] Segments;
    }
}
