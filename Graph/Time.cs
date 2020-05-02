using System;

namespace TransportGraphApp.Graph {
    public class Time : IEquatable<Time> {
        public int Value { get; set; }
        
        
        public bool Equals(Time other) {
            return other != null && Value == other.Value;
        }
    }
}