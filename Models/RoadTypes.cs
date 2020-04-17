using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class RoadTypes : IAppModel {
        public ObjectId Id { get; set; }
        
        public bool IsPrimary { get; set; } = false;

        public IList<RoadType> Values { get; set; } = new List<RoadType>();
    }
    
    public class RoadType {
        public string Name { get; set; }
    }
}