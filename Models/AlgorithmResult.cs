using System;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class AlgorithmResult : IAppModel {
        public ObjectId Id { get; set; }
        
        public DateTime RunDate { get; set; }
        
        public AlgorithmConfig AlgorithmConfig { get; set; }
        
        public IList<City> Cities { get; set; } = new List<City>();
        public IList<double> Values { get; set; } = new List<double>();
    }
}