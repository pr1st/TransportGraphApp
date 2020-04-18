using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public IEnumerable<City> GetCitiesOfTransportSystem(TransportSystem ts) {
            return GetCollection<City>().FindAll().Where(c => c.TransportSystemIds.Contains(ts.Id));
        }
        
        public int CountCitiesOfTransportSystem(TransportSystem ts) {
            return GetCollection<City>().FindAll().Count(c => c.TransportSystemIds.Contains(ts.Id));
        }
    }
}