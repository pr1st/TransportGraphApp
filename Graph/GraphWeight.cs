namespace TransportGraphApp.Graph {
    public class GraphWeight {
        public Time Time { get; set; }
        public Weight Weight { get; set; }
        public Node From { get; set; }
        public Time FromTime { get; set; }


        public GraphWeight() {
            Time = null;
            Weight = new Weight();
            From = null;
            FromTime = null;
        }
        
        public GraphWeight(Time time, Node from, Time fromTime, Weight weight) {
            Time = time;
            Weight = weight;
            From = from;
            FromTime = fromTime;
        }
    }
}