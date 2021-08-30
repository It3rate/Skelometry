namespace Skelometry
{
    using System;

    // Vector can know about other dimensions by joining another segment and comparing them (with a Joint).
    // This join can be additive or multiplied. So a vector is always relative to something, just like a number is relative to some unit.
    // This only creates area if they aren't parallel, otherwise behaves as a scalar.
    // They don't need to be perpendicular, though that allows matching units to behave the same (just as matching units allow scaling to behave the same).
    // Adds concept of left and right, area, clockwise, angular magnitude (relative direction).
    // Just like with scalars, there needs to be a unit direction, but the unit area comes directly from the vector units.
    // You can intersect with vectors. (through, cut) Also all the join types (TLX,Y)

    // *** maybe this is add only and bivector is multiplied? Add meaning end to start, or start to start etc - just get the angle and new length etc.
    // maybe doesn't even have to join - extrapolated lines work.
    public class Vector : Scalar
    {
        public bool DirectionRuleIsLeft = true;
	    public Joint Joint;
        public float UnitAngle;
        public float Angle; // to direction rule (defaults to left)
        public float ReverseAngle; // to opposite of direction rule
    }
}
