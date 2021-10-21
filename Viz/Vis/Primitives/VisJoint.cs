using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Distributions;


namespace MLTest.Vis
{
    /// <summary>
    /// A joint is observed on a concrete shape (primitives can not make joints). Generally any node with a reference that is a stroke will be a joint, but there can be others, such as crosses.
    /// </summary>
    public class VisJoint
    {
        // center point of joint - average of connection points if they aren't perfect
        // one+ points of a joint (ideally just one, but might not be perfect, so more are needed when joints don't meet correctly)
        // joint type
        // joint approximate angle 
        // compare with perfect, get likelihood 
        // error level

        // maybe joints need to be oriented on creation, like circles

	    public VisJointType JointType { get; }

	    public Point Center { get; }
	    public CompassDirection Direction { get; }
        public float Sharpness { get; } // smallest angle of the joint if it is a butt or cross, otherwise angle of the corner.

        // computed
        public double JointAngle { get; }

	    public VisJoint(Point center, VisJointType jointType, CompassDirection direction)
	    {
		    Center = center;
		    JointType = jointType;
		    Direction = direction;
	    }

        public static Gaussian TipProbability;
	    public static Gaussian ButtProbability;
	    public static Gaussian TangentProbability;
    }

	public enum VisJointType
	{
		Corner, // "L" May need to distinguish between serifs, not quite connecting corners, and other poorly made corners. Or just be able to eval probability of all types
        Butt, // T, H
        Split, // Y
        CurveSplit, // like in 'r' or 'h'
        Cross, // X
	}
}