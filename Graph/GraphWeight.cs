namespace TransportGraphApp.Graph {
    public class GraphWeight {
        public Node From { get; set; }
        public Time FromTime { get; set; }
        public Weight Weight { get; set; }


        public GraphWeight() {
            From = null;
            FromTime = null;
            Weight = new Weight();
        }
        
        public GraphWeight(Node from, Time fromTime, Weight weight) {
            From = from;
            FromTime = fromTime;
            Weight = weight;
        }
    }
}