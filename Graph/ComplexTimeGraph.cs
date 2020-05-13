using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using TransportGraphApp.Models;

namespace TransportGraphApp.Graph {
    public class ComplexTimeGraph {
        // arg1 - time used to wait in city
        // arg2 - city id
        // arg3 - road id
        // return - weight of this edge
        private readonly Func<Time, ObjectId, ObjectId, Weight> _weightFunction;

        private readonly IDictionary<Node, IList<KeyValuePair<Node, Edge>>> _graphMap =
            new Dictionary<Node, IList<KeyValuePair<Node, Edge>>>();

        public IList<Node> Results => _graphMap.Keys.ToList();

        public ComplexTimeGraph(IEnumerable<ObjectId> cityIds, Func<Time, ObjectId, ObjectId, Weight> weightFunction) {
            _weightFunction = weightFunction;
            foreach (var id in cityIds) {
                var node = new Node() {
                    Id = id,
                    IsCentral = false
                };
                _graphMap[node] = new List<KeyValuePair<Node, Edge>>();
            }
        }

        public void AddEdge(ObjectId fromId, ObjectId toId, ObjectId edgeId, int runTime,
            IList<DepartureTime> departureTimes) {
            var nodes = _graphMap.Keys;
            var from = nodes.First(n => n.Id == fromId);
            var to = nodes.First(n => n.Id == toId);
            foreach (var departureTime in departureTimes) {
                foreach (var dayOfWeek in departureTime.DaysAvailable) {
                    var time = 60 * 24 * dayOfWeek switch {
                        DayOfWeek.Monday => 0,
                        DayOfWeek.Tuesday => 1,
                        DayOfWeek.Wednesday => 2,
                        DayOfWeek.Thursday => 3,
                        DayOfWeek.Friday => 4,
                        DayOfWeek.Saturday => 5,
                        DayOfWeek.Sunday => 6,
                        _ => -1
                    };
                    time += departureTime.Hour * 60;
                    time += departureTime.Minute;


                    var edge = new Edge() {
                        Id = edgeId,
                        RunTime = new Time(runTime),
                        DepartureTime = new Time(time)
                    };
                    _graphMap[from].Add(new KeyValuePair<Node, Edge>(to, edge));
                }
            }
        }

        public void RunBellmanFord(IEnumerable<ObjectId> centralCities) {
            foreach (var centralCity in centralCities) {
                var centralNode = _graphMap.Keys.First(n => n.Id == centralCity);
                centralNode.IsCentral = true;
                centralNode.Weights.Add(new GraphWeight(null, null, null, new Weight(0)));
                foreach (var list in _graphMap.Values) {
                    var toDelete = list.Where(kv => kv.Key.Id == centralCity).ToList();
                    foreach (var kv in toDelete) {
                        list.Remove(kv);
                    }
                }
            }

            for (var i = 0; i < _graphMap.Keys.Count - 1; i++) {
                foreach (var (from, list) in _graphMap) {
                    foreach (var (to, edge) in list) {
                        if (from.IsCentral) {
                            var weight = _weightFunction.Invoke(new Time(), from.Id, edge.Id);
                            var previousValue = to.Weights.FirstOrDefault(w => w.Time == edge.DepartureTime);
                            if (previousValue != null) {
                                if (weight < previousValue.Weight) {
                                    previousValue.Weight = weight;
                                    previousValue.From = from;
                                }
                            }
                            else {
                                to.Weights.Add(new GraphWeight(edge.DepartureTime, from, null, weight));
                            }
                        }
                        else {
                            foreach (var fromWeight in from.Weights) {
                                var waitTime = fromWeight.Time - edge.DepartureTime - edge.RunTime;
                                var weight = _weightFunction.Invoke(waitTime, from.Id, edge.Id);
                                var previousValue = to.Weights.FirstOrDefault(w => w.Time == edge.DepartureTime);
                                if (previousValue != null) {
                                    if (weight + fromWeight.Weight < previousValue.Weight) {
                                        previousValue.Weight = weight + fromWeight.Weight;
                                        previousValue.FromTime = fromWeight.Time;
                                        previousValue.From = from;
                                    }
                                }
                                else {
                                    to.Weights.Add(new GraphWeight(edge.DepartureTime, from, fromWeight.Time, weight));
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RunLocalFirstBellmanFord(IDictionary<ObjectId, IList<ObjectId>> centralCities,
            IDictionary<ObjectId, IList<ObjectId>> terminalCities) {
            // todo
        }
    }
}