namespace Slugs.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MathF
    {
	    public static float tolerance = 0.0001f;

	    public const float QuarterPI = (float)Math.PI * 0.25f;
	    public const float HalfPI = (float)Math.PI * 0.5f;
        public const float PI = (float)Math.PI;
        public const float PIAndAHalf = (float)Math.PI * 1.5f;
        public const float TwoPI = (float)Math.PI * 2f;

        public static float ToDegrees(float radAngle) => (radAngle / MathF.PI) * 180f;
        public static float ToRadians(float degAngle) => (degAngle * MathF.PI) / 180f;
    }
}
