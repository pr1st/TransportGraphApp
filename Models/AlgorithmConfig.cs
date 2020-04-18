using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class AlgorithmConfig : IAppModel {
        public ObjectId Id { get; set; }
        
        public bool IsPrimary { get; set; }

        public IList<TransportSystem> TransportSystems { get; set; } = new List<TransportSystem>();
        
        public AlgorithmType AlgorithmType { get; set; }
        
        public MethodType MethodType { get; set; }

        public IList<CityTag> CityTags { get; set; } = new List<CityTag>();
        
        public IList<RoadType> RoadTypes { get; set; } = new List<RoadType>();
        
        public DepartureTime DepartureTime { get; set; }
    }

    public enum AlgorithmType {
        Length,
        Time,
        Cost,
        ComplexCost
    }

    public enum MethodType {
        Standard,
        Local,
        Another
    }
}