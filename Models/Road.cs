using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace TransportGraphApp.Models {
    public class Road {
        public ObjectId Id { get; set; }

        public ObjectId TransportSystemId { get; set; }
    }
}