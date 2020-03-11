using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace TransportGraphApp.Models {
    public class City {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        [BsonRef("TransportSystem")]
        public TransportSystem TransportSystem { get; set; }
    }
}