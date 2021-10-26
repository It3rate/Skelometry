using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationalGeometry
{
    public class Verb
    {
        public int ID { get; }
        public string Name { get; }
        public int Kind { get; }

        public IReference Left { get; private set; }
        public IReference Right { get; private set; }
        public Operation Op { get; private set; }
    }

}
