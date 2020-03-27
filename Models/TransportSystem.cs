using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class TransportSystem : IAppModel {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public SystemParameters Parameters { get; set; }
    }
}