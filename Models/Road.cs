using System.Collections;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class Road : IAppModel {
        public ObjectId Id { get; set; }
        
        public ObjectId FromCityId { get; set; }
        
        public ObjectId ToCityId { get; set; }

        public double Length { get; set; }
        
        public double Cost { get; set; }

        public double Time { get; set; }
        
        public string RoadType { get; set; }
        
        public IList<DepartureTime> DepartureTimes { get; set; } = new List<DepartureTime>();
        public ObjectId TransportSystemId { get; set; }
    }
}