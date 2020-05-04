using System;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Graph {
    public class Node : IEquatable<Node> {
        public ObjectId Id { get; set; }
        
        public bool IsCentral { get; set; }
        
        public IDictionary<Time, GraphWeight> TimeTable { get; } = new Dictionary<Time, GraphWeight>();
        
        public bool Equals(Node other) {
            return other != null && Id == other.Id;
        }
    }
}