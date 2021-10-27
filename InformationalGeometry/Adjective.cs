using System.Collections.Generic;

namespace InformationalGeometry
{
	public class Adjective : IReference
	{
		public int ID { get; }
		public string Name { get; }
        public int Kind { get; private set; }
        public bool IsRoot { get; }
        public float Value { get; set; }

		public Noun Parent { get; }
		public List<Preposition> Linkages { get; }

		public bool IsUnit { get; set; }
		public Adjective UnitInstance { get; private set; } // probably need a translator here too
     
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
}