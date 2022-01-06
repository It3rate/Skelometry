using SkiaSharp;

namespace Slugs.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPoint : IEquatable<IPoint>
    {
        float X { get; }
        float Y { get; }
	    bool IsEmpty { get; }
	    float Length { get; }
        float LengthSquared { get; }
        void Offset(IPoint p);
	    void Offset(float dx, float dy);
        //bool Equals(IPoint obj);
        //bool Equals(object obj);
        //int GetHashCode();
    }

    public abstract class PointBase : IPoint
    {
	    public float X { get; }
	    public float Y { get; }

	    public bool IsEmpty { get; }
	    public float Length { get; }
	    public float LengthSquared { get; }
	    public void Offset(IPoint p)
	    {
		    throw new NotImplementedException();
	    }

	    public void Offset(float dx, float dy)
	    {
		    throw new NotImplementedException();
	    }

	    public bool Equals(IPoint obj)
	    {
		    throw new NotImplementedException();
	    }
    }
}
