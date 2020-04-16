using LiteDB;

namespace TransportGraphApp.Models {
    public class CityTag : IAppModel {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }
}