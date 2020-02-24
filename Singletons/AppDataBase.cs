using LiteDB;

namespace TransportGraphApp.Singletons {
    internal class AppDataBase {
        private static AppDataBase _instance;

        public static AppDataBase Instance => _instance ??= new AppDataBase();


        private LiteDatabase _liteDatabase;

        private AppDataBase() { }

        public void Build(string filePath) {
            _liteDatabase?.Dispose();
            _liteDatabase = new LiteDatabase(filePath);
        }

        public ILiteCollection<T> GetCollection<T>() {
            return _liteDatabase.GetCollection<T>();
        }

        public void Close() => _liteDatabase?.Dispose();
    }
}