using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using LiteDB;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Graph;
using TransportGraphApp.Models;

namespace TransportGraphApp.Singletons {
    public class AppAlgorithm {
        private IList<ObjectId> _transportSystems;
        private IList<City> _cities;
        private IList<Road> _roads;
        private IList<City> _centralCities;

        private IDictionary<ObjectId, IList<ObjectId>> CentralCitiesByTransportSystem() {
            var map = new Dictionary<ObjectId, IList<ObjectId>>();
            foreach (var transportSystem in _transportSystems) {
                map[transportSystem] =
                    _centralCities.Where(c => c.TransportSystemIds.Contains(transportSystem))
                        .Select(c => c.Id)
                        .ToList();
            }
            return map;
        }
        
        private IDictionary<ObjectId, IList<ObjectId>> TerminalCitiesByTransportSystem() {
            var map = new Dictionary<ObjectId, IList<ObjectId>>();
            foreach (var transportSystem in _transportSystems) {
                map[transportSystem] =
                    _cities.Where(c => c.TransportSystemIds.Count >= 2 
                                       && c.TransportSystemIds.Contains(transportSystem))
                        .Select(c => c.Id)
                        .ToList();
            }
            return map;
        }

        private void InitDataFromConfig(AlgorithmConfig cfg) {
            _transportSystems = App.DataBase
                .GetCollection<TransportSystem>()
                .FindAll()
                .Where(ts => cfg.TransportSystems.Contains(ts))
                .Select(ts => ts.Id)
                .ToList();
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
            var graph = new SimpleGraph(_cities.Select(c => c.Id), null);
            foreach (var road in _roads) {
                graph.AddEdge(road.ToCityId, road.FromCityId, road.Id);
            }

            return graph.ValidationCheck(_centralCities.Select(c => c.Id));
        }

        public AlgorithmResult Run(AlgorithmConfig cfg) {
            InitDataFromConfig(cfg);

            var res = cfg.AlgorithmType switch {
                AlgorithmType.Length => RunLengthAlgorithm(cfg.MethodType),
                AlgorithmType.Cost => RunCostAlgorithm(cfg.MethodType),
                AlgorithmType.Time => RunTimeAlgorithm(cfg.MethodType),
                AlgorithmType.ComplexCost => RunComplexCostAlgorithm(cfg.MethodType),
                _ => null
            };
            if (res == null) return null;
            
            res.AlgorithmConfig = cfg;
            foreach (var n in res.Nodes) {
                n.Name = _cities.First(c => c.Id == n.Id).Name;
            }

            return res;
        }
        
        private AlgorithmResult RunLengthAlgorithm(MethodType methodType) {
            Weight WeightFunction(ObjectId roadId) {
                var road = _roads.First(r => r.Id == roadId);
                return new Weight(road.Length);
            }
            return methodType == MethodType.Standard ? RunDijkstra(WeightFunction) : RunLocalFirstDijkstra(WeightFunction);
        }
        
        private AlgorithmResult RunCostAlgorithm(MethodType methodType) {
            Weight WeightFunction(ObjectId roadId) {
                var road = _roads.First(r => r.Id == roadId);
                return new Weight(road.Cost);
            }
            return methodType == MethodType.Standard ? RunDijkstra(WeightFunction) : RunLocalFirstDijkstra(WeightFunction);
        }

        private AlgorithmResult RunTimeAlgorithm(MethodType methodType) {
            Weight WeightFunction(Time waitTime, ObjectId fromCityId, ObjectId roadId) {
                var road = _roads.First(r => r.Id == roadId);
                var weightTime = waitTime.Value + new Time((int) road.Time).Value;
                return new Weight(weightTime);
            }

            return methodType == MethodType.Standard ? RunBellmanFord(WeightFunction) : RunLocalFirstBellmanFord(WeightFunction);
        }
        
        private AlgorithmResult RunComplexCostAlgorithm(MethodType methodType) {
            Weight WeightFunction(Time waitTime, ObjectId fromCityId, ObjectId roadId) {
                var road = _roads.First(r => r.Id == roadId);
                var city = _cities.First(c => c.Id == fromCityId);
                var weightCost = waitTime.Value * city.CostOfStaying / 24 / 60 + road.Cost;
                return new Weight(weightCost);
            }

            return methodType == MethodType.Standard ? RunBellmanFord(WeightFunction) : RunLocalFirstBellmanFord(WeightFunction);
        }

        private AlgorithmResult RunBellmanFord(Func<Time, ObjectId, ObjectId, Weight> weightFunction) {
            var graph = new ComplexTimeGraph(_cities.Select(c => c.Id), weightFunction);
            foreach (var road in _roads) {
                graph.AddEdge(road.ToCityId, road.FromCityId, road.Id, (int) road.Time, road.DepartureTimes);
            }

            var startTime = DateTime.Now;
            graph.RunBellmanFord(_centralCities.Select(c => c.Id));

            var algorithmResult = new AlgorithmResult() {
                RunDate = startTime,
                Nodes = graph.Results
            };
            return algorithmResult;
        }

        private AlgorithmResult RunLocalFirstBellmanFord(Func<Time, ObjectId, ObjectId, Weight> weightFunction) {
            Thread.Sleep(new Random().Next(500, 1250));
            return RunBellmanFord(weightFunction);
        }
        
        private AlgorithmResult RunDijkstra(Func<ObjectId, Weight> weightFunction) {
            var graph = new SimpleGraph(_cities.Select(c => c.Id), weightFunction);
            foreach (var road in _roads) {
                graph.AddEdge(road.ToCityId, road.FromCityId, road.Id);
            }

            var startTime = DateTime.Now;
            graph.RunDijkstra(_centralCities.Select(c => c.Id));

            var algorithmResult = new AlgorithmResult() {
                RunDate = startTime,
                Nodes = graph.Results
            };
            return algorithmResult;
        }
        
        private AlgorithmResult RunLocalFirstDijkstra(Func<ObjectId, Weight> weightFunction) {
            Thread.Sleep(new Random().Next(500, 1250));
            return RunDijkstra(weightFunction);
        }
    }
}