using System.Collections.Generic;
using System.Linq;
using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    internal class AppCurrentSystem {
        private static AppCurrentSystem _instance;
        public static AppCurrentSystem Instance => _instance ??= new AppCurrentSystem();


        private AppCurrentSystem() {
        }
        
        private TransportSystem _transportSystem;
        private IList<City> _cities;
        private IList<Road> _roads;

        public void Select(TransportSystem ts) {
            if (ts == null) {
                _transportSystem = null;
                _cities = null;
                _roads = null;
                
                App.ChangeAppState(AppStates.TransportSystemSelected, false);
            }
            else {
                _transportSystem = ts;
                _cities = AppDataBase.Instance
                    .GetCollection<City>()
                    .Find(c => c.TransportSystemId == ts.Id)
                    .ToList();
                _roads = AppDataBase.Instance
                    .GetCollection<Road>()
                    .Find(r => r.TransportSystemId == ts.Id)
                    .ToList();
            
                App.ChangeAppState(AppStates.TransportSystemSelected, true);   
            }
        }

        public TransportSystem GetTransportSystem() => _transportSystem;
        public IList<City> GetCities() => _cities;
        public IList<Road> GetRoads() => _roads;
    }
}