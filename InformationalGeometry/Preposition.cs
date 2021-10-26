using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationalGeometry
{
    public class Preposition
    {
        public int ID { get; }
        public string Name { get; }
        public int Kind { get; }

        public IReference Source { get; private set; }
        public IReference Target { get; private set; }
        public float Magnitude { get; private set; }

    }
}
