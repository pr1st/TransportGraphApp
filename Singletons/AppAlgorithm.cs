using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using LiteDB;
using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    public class AppAlgorithm {
        private IList<City> _cities;
        private IList<Road> _roads;
        private IList<City> _centralCities;

        private void InitDataFromConfig(AlgorithmConfig cfg) {
            _cities = App.DataBase
                .GetCollection<City>()
                .FindAll()
                .Where(c => c.TransportSystemIds.Any(cc => cfg.TransportSystems.Select(ts => ts.Id).Contains(cc)))
                .ToList();
            _roads = App.DataBase
                .GetCollection<Road>()
                .FindAll()
                .Where(r => !cfg.RoadTypes.Select(rt => rt.Name).Contains(r.RoadType.Name))
                .Where(r => cfg.TransportSystems.Select(ts => ts.Id).Contains(r.TransportSystemId))
                .ToList();
            _centralCities = _cities
                .Where(c => c.Tags.Select(ct => ct.Name)
                    .Any(ctName => cfg.CityTags.Select(ctt => ctt.Name).Contains(ctName)))
                .ToList();
        }


        public bool CheckTransportSystems(AlgorithmConfig cfg) { 
            InitDataFromConfig(cfg);
            var graph = new Graph(_cities) {
                Roads = _roads.ToDictionary(r => r.Id, r => r)
            };
            foreach (var road in _roads) {
                graph.AddEdge(road);
            }

            return graph.ValidationCheck(_centralCities.Select(c => c.Id));
        }

        public AlgorithmResult Run(AlgorithmConfig cfg) {
            InitDataFromConfig(cfg);
            var res = cfg.MethodType switch {
                MethodType.Standard => RunStandardMethod(AlgorithmFunction(cfg)),
                MethodType.Local => RunLocalFirstMethod(AlgorithmFunction(cfg)),
                MethodType.Another => null,
                _ => null
            };
            if (res != null) {
                res.AlgorithmConfig = cfg;
            }
            return res;
        }

        private static Func<int, City, Road, double> AlgorithmFunction(AlgorithmConfig cfg) {
            return cfg.AlgorithmType switch {
                AlgorithmType.Length => (waitTime, city, road) => road.Length,
                // AlgorithmType.Time => (waitTime, city, road) => waitTime + road.Time,
                AlgorithmType.Time => null,
                AlgorithmType.Cost => (waitTime, city, road) => road.Cost,
                AlgorithmType.ComplexCost => null,
                // AlgorithmType.ComplexCost => (waitTime, city, road) =>
                    // waitTime * city.CostOfStaying / 24 / 60 + road.Cost,
                _ => throw new NotImplementedException()
            };
        }

        private AlgorithmResult RunStandardMethod(Func<int, City, Road, double> weightFunction) {
            if (weightFunction == null) return null;
            
            var graph = new Graph(_cities) {
                Cities = _cities.ToDictionary(c => c.Id, c => c),
                Roads = _roads.ToDictionary(r => r.Id, r => r),
                WeightFunction = weightFunction
            };
            foreach (var road in _roads) {
                graph.AddEdge(road);
            }

            var res = graph.RunDijkstra(_centralCities.Select(c => c.Id));
            return res;
        }

        private AlgorithmResult RunLocalFirstMethod(Func<int, City, Road, double> weightFunction) {
            return null;
        }
    }

    public class Graph {
        private readonly IDictionary<ObjectId, IList<ObjectId>> _linkedMap = new Dictionary<ObjectId, IList<ObjectId>>();

        public IDictionary<ObjectId, City> Cities { get; set; }
        public IDictionary<ObjectId, Road> Roads { get; set; }
        public Func<int, City, Road, double> WeightFunction { get; set; }

        public Graph(IEnumerable<City> cities) {
            foreach (var city in cities) {
                _linkedMap[city.Id] = new List<ObjectId>();
            }
        }

        public void AddEdge(Road road) {
            _linkedMap[road.ToCityId].Add(road.Id);
        }

        public bool ValidationCheck(IEnumerable<ObjectId> centralCities) {
            var visited = new Dictionary<ObjectId, bool>();
            foreach (var key in _linkedMap.Keys) {
                visited[key] = false;
            }

            var stack = new Stack<ObjectId>();
            foreach (var c in centralCities) {
                stack.Push(c);
            }

            while (stack.Any()) {
                var v = stack.Pop();
                if (visited[v]) continue;

                visited[v] = true;
                foreach (var edge in _linkedMap[v]) {
                    stack.Push(Roads[edge].FromCityId);
                }
            }

            return visited.All(kv => kv.Value);
        }

        public AlgorithmResult RunDijkstra(IEnumerable<ObjectId> centralCities) {
            var startTime = DateTime.Now;
            // initialization
            var queue = new HashSet<ObjectId>(_linkedMap.Keys);

            var dist = new Dictionary<ObjectId, double>(
                _linkedMap.ToDictionary(
                    kv => kv.Key,
                    kv => double.MaxValue));
            
            var previousCity = new Dictionary<ObjectId, ObjectId>(_linkedMap.Count);
            
            foreach (var centralCity in centralCities) {
                dist[centralCity] = 0;
                previousCity[centralCity] = null;
            }
            
            while (queue.Any()) {
                var u = queue.OrderBy(v => dist[v]).First();
                var uCity = Cities[u];
                
                queue.Remove(u);

                foreach (var edge in _linkedMap[u]) {
                    var road = Roads[edge];
                    var v = road.FromCityId;
                    
                    if (!queue.Contains(v)) continue;

                    var alt = dist[u] + WeightFunction.Invoke(0, uCity, road);
                    if (alt < dist[v]) {
                        dist[v] = alt;
                        previousCity[v] = u;
                    }
                }
            }

            return new AlgorithmResult() {
                RunDate = startTime,
                Cities = Cities.Values.ToList(),
                Values = dist.Select(kv => kv.Value).ToList()
            };
        }
    }
}