using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class Edge {
        public int Id { get; set; }

        public int FromNodeId { get; set; }

        public int ToNodeId { get; set; }

        public IList<Attribute> Attributes { get; set; }

        public int GraphId { get; set; }
    }
}