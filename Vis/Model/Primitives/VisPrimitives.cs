﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Probabilistic.Distributions;
using Vis.Model.Connections;

namespace Vis.Model.Primitives
{
	//public enum VisElementType { Any, Point, Node, Circle, Square, Rectangle, Oval, Joint, Stroke, Shape }

	public interface IElement
	{
        bool IsPath { get; }
        void AddOffset(float x, float y);
        VisPolyline GetPolyline();
        VisPoint ProjectPointOnto(VisPoint p);
	}
	/// <summary>
	/// The mental map primitives when we conceptualize things at a high level. These are meant to be (ideally) what we use, not what is mathematically possible or even simple.
	/// </summary>
	public interface IPrimitive : IElement, ICloneable, IEquatable<IPrimitive>
	{
		float X { get; }
		float Y { get; }
		float Similarity(IPrimitive p);
		VisPoint Sample(Gaussian g);
	}
	public interface IPrimitivePath : IElement, IPath
	{
		VisPoint[] GetPolylinePoints(int pointCount = 24);
	}


	public interface IArea
	{
		VisPoint Center { get; }
		float Area { get; }
		VisRectangle Bounds { get; }
		bool IsClosed { get; }
		bool IsConcave { get; }
		int JointCount { get; }
		int CornerCount { get; }
		float Sharpness { get; }
	}

    //   public class VisOval : CircleRef
    //   {
    //    public override VisElementType ElementType => VisElementType.Oval;

    //    public Node PerimeterSide { get; }
    //    public float RadiusSide{ get; }

    //    public VisOval(Node center, Node perimeterOrigin, Node perimeterSide) : base(center, perimeterOrigin)
    //    {
    //	    PerimeterSide = perimeterSide;
    //	    RadiusSide = center.Location.DistanceTo(PerimeterSide.Location);
    //       }

    //    public override Point GetPoint(float position, float offset)
    //    {
    //	    var rads = Utils.NormToRadians(position);
    //	    return new Point(Location.X + (float)Math.Sin(rads) * (Radius + offset), Location.Y + (float)Math.Cos(rads) * (Radius + offset));
    //    }
    //       public Stroke GetElement(CompassDirection direction) => null;
    //   }
    //   public class VisSquare : Point
    //{
    //	public override VisElementType ElementType => VisElementType.Square;

    //	public Stroke Reference { get; }
    //	public float Shift => Val0;
    //	public float Radius => Val1;


    //	public override Point Location { get; }

    //	public VisSquare(Stroke reference, float position, float offset) : base(position, offset * reference.Length())
    //	{
    //		Reference = reference;
    //		Location = Reference.GetPoint(Shift, 0); // start
    //	}

    //	public Stroke GetElement(CompassDirection direction) => null;

    //	public override Point GetPoint(float position, float offset)
    //	{
    //		var rads = Utils.NormToRadians(position);
    //		return new Point(Location.X + (float)Math.Sin(rads) * (Radius + offset), Location.Y + (float)Math.Cos(rads) * (Radius + offset));
    //	}
    //   }

}
