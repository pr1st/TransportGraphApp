﻿using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    internal class Node : IAttributeElement {
        public int Id { get; set; }

        public string Name { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public IDictionary<string, BsonValue> Attributes { get; set; }

        public int GraphId { get; set; }
    }
}