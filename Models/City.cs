using LiteDB;

namespace TransportGraphApp.Models {
    public class City : IAppModel {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double CostOfStaying { get; set; }
        
        public ObjectId TransportSystemId { get; set; }
    }
}