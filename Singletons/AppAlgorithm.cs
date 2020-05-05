﻿using System;
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
            var graph = new GraphG(_cities) {
                Roads = _roads.ToDictionary(r => r.Id, r => r)
            };
            foreach (var road in _roads) {
                graph.AddEdge(road);
            }

            // todo change source
            
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
            res.CityNames = new List<string>(res.Nodes.Count);
            foreach (var n in res.Nodes) {
                res.CityNames.Add(_cities.First(c => c.Id == n.Id).Name);
            }

            return res;
        }
        
        private AlgorithmResult RunLengthAlgorithm(MethodType methodType) {
            // Weight WeightFunction(Time waitTime, ObjectId fromCityId, ObjectId roadId) {
            //     var road = _roads.First(r => r.Id == roadId);
            //     var weightTime = waitTime.Value + new Time((int) road.Time).Value;
            //     return new Weight(weightTime);
            // }
            // todo
            return methodType == MethodType.Standard ? RunDijkstra() : RunLocalFirstDijkstra();
        }
        
        private AlgorithmResult RunCostAlgorithm(MethodType methodType) {
            // Weight WeightFunction(Time waitTime, ObjectId fromCityId, ObjectId roadId) {
            //     var road = _roads.First(r => r.Id == roadId);
            //     var weightTime = waitTime.Value + new Time((int) road.Time).Value;
            //     return new Weight(weightTime);
            // }
            // todo
            return methodType == MethodType.Standard ? RunDijkstra() : RunLocalFirstDijkstra();
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
                // var weightTime = waitTime.Value + new Time((int) road.Time).Value;
                // return new Weight(weightTime);
                // todo
                return null;
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
            // todo
            return null;
        }
        
        private AlgorithmResult RunDijkstra() {
            // todo
            return null;
        }
        
        private AlgorithmResult RunLocalFirstDijkstra() {
            // todo
            return null;
        }

        // todo delete
        public class GraphG {
            private readonly IDictionary<ObjectId, IList<ObjectId>> _linkedMap =
                new Dictionary<ObjectId, IList<ObjectId>>();

            public IDictionary<ObjectId, Road> Roads { get; set; }

            public GraphG(IEnumerable<City> cities) {
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
        }
    }
}