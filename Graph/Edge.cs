using System;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Graph {
    public class Edge {
        public ObjectId Id { get; set; }

        public Time RunTime { get; set; }
        
        public Time DepartureTime { get; set; }
    }
}