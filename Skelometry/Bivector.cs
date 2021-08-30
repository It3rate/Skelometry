namespace Skelometry
{
    using System;

    // Two vectors multiplied to give an area and orientation in a dual triangle.
    // The minimal way to encode this is probably the area, a spin direction, the unit ratio, and magnitude of one side.
    // This lines up with GA, but seems like it throws out a lot of info that gets recovered with calculation. Maybe not.
    // Maybe the dot product part is ad-bc what the triangles do, and the wedge is the area/rotation.
    // Would be nice if the wedge captured the triangles better, but maybe thats just visualization.

    // When a V shape joins, you do see extra matter at the joint, or at least extra weight.
    // It mentally degrades if you see the parts aren't acutally joined but just visually touch. This is joined objects.

    public class Bivector : Vector
    {
        public Joint Joint;
        public Vector RefVector;
        public float UnitArea;// automatic based on vector units { get; }
        public float Area;
    }
}
