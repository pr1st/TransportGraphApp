using LiteDB;

namespace TransportGraphApp.Models {
    public interface IAppModel {
        public ObjectId Id { get; set; }
    }
}