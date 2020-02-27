using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class Graph {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<Attribute> GraphAttributes { get; set; }

        public IList<Attribute> DefaultNodeAttributes { get; set; }

        public IList<Attribute> DefaultEdgeAttributes { get; set; }

        public override string ToString() {
            return Name;
        }
    }
}