using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace TransportGraphApp.Models {
    public class Node {
        public int Id { get; set; }

        public string Name { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public IList<Attribute> Attributes { get; set; }

        public int GraphId { get; set; }
    }
}