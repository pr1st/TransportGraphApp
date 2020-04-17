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
            return GetCollection<City>().Find(c => c.TransportSystemIds.Count(tsId => tsId == ts.Id) == 1);
        }
        
        public int CountCitiesOfTransportSystem(TransportSystem ts) {
            return GetCollection<City>().Count(c => c.TransportSystemIds.Count(tsId => tsId == ts.Id) == 1);
        }
    }
}