using System.Collections.Generic;

namespace TransportGraphApp.Graph {
    public class Node {
        private int Id { get; set; }
        
        private IDictionary<Time, Weight> TimeTable { get; } = new Dictionary<Time, Weight>(); 
    }
}