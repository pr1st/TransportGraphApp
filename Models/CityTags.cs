using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class CityTags : IAppModel {
        public ObjectId Id { get; set; }

        public bool IsPrimary { get; set; } = false;

        public IList<CityTag> Values { get; set; } = new List<CityTag>();
    }
    
    public class CityTag {
        public string Name { get; set; }
    }
}