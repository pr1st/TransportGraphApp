using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class City : IAppModel {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public double CostOfStaying { get; set; }

        public IList<ObjectId> Tags { get; set; } = new List<ObjectId>();

        public IList<ObjectId> TransportSystemIds { get; set; } = new List<ObjectId>();
    }
}