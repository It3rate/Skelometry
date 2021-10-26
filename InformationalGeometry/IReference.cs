using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationalGeometry
{
    public interface IReference
    {
        int ID { get; }
        string Name { get; }
        int Kind { get; }
        bool IsRoot { get; }

        Noun Parent { get; }

        // references to joints, should these be stored/calcuated in the 'world' area?
        // Children and attributes are a form of joints, and clearly local,
        // but 'between' is world level and may need to be calculated.
        List<Preposition> Linkages { get; } 
    }

}
