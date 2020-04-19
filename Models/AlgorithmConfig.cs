﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public static AlgorithmConfig GetDefault => new AlgorithmConfig() {
            IsPrimary = true,
            TransportSystems = new List<TransportSystem>(),
            AlgorithmType = AlgorithmType.Cost,
            MethodType = MethodType.Standard,
            CityTags = new List<CityTag>(),
            RoadTypes = new List<RoadType>(),
        };
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