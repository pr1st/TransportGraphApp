using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace TransportGraphApp.Models {
    public class City {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Magnitude { get; set; }

        public double CostOfStaying { get; set; }

        public ObjectId TransportSystemId { get; set; }
    }
}