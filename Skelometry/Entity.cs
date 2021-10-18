

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


}

public class Entity
{
    public List<Facet> Facets { get; set; }
}