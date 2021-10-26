using System.Collections.Generic;

namespace InformationalGeometry
{
    // Entity is an object with no other inherent information. 
    // Entities can have other entities attached through composition, and these can be single or groups of entities.
    // Following from this, they also can be part of a group, and can have a parent.
    //
    // Information can also be attached as Facets (attributes of the object) that are themselves entities.
    // Facets that directly describe the object are like adjectives, (eg. color, length, weight vs tires, doors, speakers)
    // where the property is named, units are specified and a value is given, 
    // but they can not exist without being attached to something.
    // Generally these have one value, but can have multiple (eg. a zebra's color is black and white).
    //
    // These things are all joined together with joints. All the parent, array, child groupings apply,
    // as well as joins to Facets, and even along Facets. A line would be an entity with a length Facet,
    // and things can join to that line at the tips, or anywhere along it.
    //
    // Entities can be used as units, but only for counting - the value is generally one, and the unit is the entity itself.
    // A specific Facet value is used as the unit in continuous measurements, and with join, extend and stretch you get the 
    // traditional +-*/ etc.

	public class Noun : IReference
    {
        public int ID { get; }
        public string Name { get; }
        public int Kind { get; }

        public Noun Parent { get; }
        public List<Preposition> Linkages { get; }

        public List<Adjective> Facets { get; }

        public List<Noun> Children { get; }
        public List<Noun> Siblings { get; }
        public bool IsRoot { get; }


	}
}