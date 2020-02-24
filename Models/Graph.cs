using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    internal class Graph : IAttributeElement {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<Attribute> GraphAttributes { get; set; }

        public IList<Attribute> NodeAttributes { get; set; }

        public IList<Attribute> EdgeAttributes { get; set; }

        public IDictionary<string, BsonValue> Attributes { get; set; }
    }
}