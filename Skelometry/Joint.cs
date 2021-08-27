namespace Skelometry
{
    using System;

    public class Joint
    {
	    protected Line[] Line;
	    public Line A => Line[0];
	    public Line B => Line[1];
        public float Angle = 0;
    }
}
