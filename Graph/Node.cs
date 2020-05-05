using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace TransportGraphApp.Graph {
    public class Node : IEquatable<Node> {
        public ObjectId Id { get; set; }
        
        public bool IsCentral { get; set; }
        
        public IList<Time> TimeTable { get; set; } = new List<Time>();
        
        public IList<GraphWeight> Weights { get; set; } = new List<GraphWeight>();

        public GraphWeight GetWeightForTime(Time t) {
            var index = TimeTable.IndexOf(t);
            return index == -1 ? null : Weights[index];
        }

        public void AddWeight(Time t, GraphWeight weight) {
            TimeTable.Add(t);
            Weights.Add(weight);
        }

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