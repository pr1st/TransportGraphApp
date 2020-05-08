using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace TransportGraphApp.Graph {
    public class Node : IEquatable<Node> {
        public ObjectId Id { get; set; }
        
        public bool IsCentral { get; set; }
        
        public string Name { get; set; }

        public IList<GraphWeight> Weights { get; set; } = new List<GraphWeight>();

        public GraphWeight MinWeight() {
            var minWeight = new GraphWeight();
            foreach (var weight in Weights) {
                if (weight.Weight < minWeight.Weight) {
                    minWeight = weight;
                }
            }

            return minWeight;
        } 
        
        public bool Equals(Node other) {
            return other != null && Id == other.Id;
        }
    }
}