using System;
using System.Collections;
using System.Collections.Generic;

namespace TransportGraphApp.Singletons {
    public class AppAlgorithm {
        private static AppAlgorithm _instance;

        public static AppAlgorithm Instance => _instance ??= new AppAlgorithm();

        public readonly IList<AlgorithmResults> ResentResults = new List<AlgorithmResults>();
        
        private AppAlgorithm() {
        }

        public void StartAlgorithm(AlgorithmConfig cfg) {
            
            
            var results = new AlgorithmResults() {
                UsedConfig = cfg
            };
            ResentResults.Add(results);
        }
    }

    public enum AlgorithmType {
        Length,
        Cost,
        Time,
        CostWithCities
    }
    
    public class AlgorithmConfig {
        public AlgorithmType AlgorithmType { get; set; }
        public DayOfWeek StartWeekDay { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
    }

    public class AlgorithmResults {
        public AlgorithmConfig UsedConfig { get; set; }
    }
}