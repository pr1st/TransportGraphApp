using LiteDB;

namespace TransportGraphApp.Models {
    public class RoadType : IAppModel {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }
}