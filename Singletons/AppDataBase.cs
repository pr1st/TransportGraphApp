using System.IO;
using LiteDB;
using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    public class AppDataBase {
        private LiteDatabase _liteDatabase;
        
        public void Open(string filePath) {
            _liteDatabase = new LiteDatabase(filePath);
        }

        public void Close() {
            _liteDatabase?.Dispose();
        }

        public ILiteCollection<T> GetCollection<T>() where T : IAppModel {
            return _liteDatabase.GetCollection<T>(typeof(T).Name);
        }
    }
}