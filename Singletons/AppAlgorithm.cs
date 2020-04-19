using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    public class AppAlgorithm {
        public AppAlgorithm() {
            // todo
        }

        public string ResultDescription { get; set; }

        private bool _isStop = false;
        public void Stop() => _isStop = true;

        private bool StopAction() {
            _isStop = false;
            ResultDescription = "Работа алгоритма была приостановленна";
            return false;
        }

        private IList<City> _cities;
        private IList<Road> _roads;
        
        public bool CheckTransportSystems(AlgorithmConfig cfg) {
            _cities = App.DataBase
                .GetCollection<City>()
                .FindAll()
                .Where(c => c.TransportSystemIds.Any(cc => cfg.TransportSystems.Select(ts => ts.Id).Contains(cc)))
                .ToList();
            _roads = App.DataBase
                .GetCollection<Road>()
                .FindAll()
                .Where(r => cfg.RoadTypes.Select(rt => rt.Name).Contains(r.RoadType.Name))
                .Where(r => cfg.TransportSystems.Select(ts => ts.Id).Contains(r.TransportSystemId))
                .ToList();
            // todo
            Console.WriteLine(_cities.Count);
            Console.WriteLine(_roads.Count);
            return true;
        }
    }
}