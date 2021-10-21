 

public class Facet
{
    public float Value { get; set; }
    public int Kind { get; set; }
    public Entity Parent { get; set; }
    public bool IsUnit { get; set; }
    
    public int PrecisionMeasured { get; set; }   
    public int PrecisionActual { get; set; }
    public int PrecisionPotential { get; set; }
    public float PrecisionChangeRate { get; set; } // (fn?) rate of change or precision as values scale and shrink

    public int NoiseKind { get; set; }
    public float NoiseValue { get; set; }
    public float NoiseChangeRate { get; set; }

    public int ComputationalComplexity { get; set; } // 1,2 - easy 3.1415... harder - factors of large numbers, hardest etc
    public float ComputationalComplexityChangeRate { get; set; }

    public float Noise { get; set; }

    public uint RangeMin { get; set; } // number of halfings possible, need to characterize limits. (binary)
    public uint RangeMax { get; set; } // number of doubling possible, need to characterize limits. (binary)
    public int RangeOffset { get; set; } // offset in doublings to center of range (can be negative for fractional values)

    public float DimensionalTolerance { get; set; } // hope close to 'on the line' (same dimension) is accepatble.


    public Facet Reciprocal { get; set; }
    public static Facet Identity { get; set; } 

}

public class Entity
{
    public List<Facet> Facets { get; set; }
}