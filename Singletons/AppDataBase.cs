using System.IO;
using LiteDB;
using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    internal class AppDataBase {
        private static AppDataBase _instance;

        public static AppDataBase Instance => _instance ??= new AppDataBase();


        private LiteDatabase _liteDatabase;
        private string _filePath = "";

        private AppDataBase() {
        }

        public void Build(string filePath) {
            var newDatabase = new LiteDatabase(filePath);

            _liteDatabase?.Dispose();
            _liteDatabase = newDatabase;
            _filePath = new FileInfo(filePath).FullName;
        }

        public void Create(string filePath) {
            var createdDatabase = new LiteDatabase(filePath);
            createdDatabase.Dispose();
        }

        public string DataBaseFileLocation() => _filePath;

        public void Close() => _liteDatabase?.Dispose();

        public ILiteCollection<T> GetCollection<T>() {
            return _liteDatabase.GetCollection<T>(typeof(T).Name);
        }
    }
}