namespace TransportGraphApp.Graph {
    public class GraphWeight {
        public Node From { get; set; }
        public Weight Weight { get; set; }
        
        
        public GraphWeight(Node from, Weight weight) {
            From = from;
            Weight = weight;
        }
    }
}