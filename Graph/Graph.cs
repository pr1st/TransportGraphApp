using System;

namespace TransportGraphApp.Graph {
    public class Graph {
        public Func<Edge, Node, Edge, Weight> WeightFunction { get; }
    }
}