namespace Slugs.Entities
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
}
