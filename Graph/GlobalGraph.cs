using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace TransportGraphApp.Graph {
    public class GlobalGraph {
        private readonly Func<GlobalGraphEdge, Weight> _weightFunction;

        private readonly IDictionary<Node, IList<KeyValuePair<Node, ObjectId>>> _graphMap =
            new Dictionary<Node, IList<KeyValuePair<Node, ObjectId>>>();

        public IList<Node> Results => _graphMap.Keys.ToList();

        public GlobalGraph(IEnumerable<ObjectId> cityIds, Func<GlobalGraphEdge, Weight> weightFunction) {
            _weightFunction = weightFunction;
            foreach (var id in cityIds) {
                var node = new Node() {
                    Id = id,
                    IsCentral = false
                };
                _graphMap[node] = new List<KeyValuePair<Node, ObjectId>>();
            }
        }

        public void AddEdge(GlobalGraphEdge edge) {
            var nodes = _graphMap.Keys;
            var from = nodes.First(n => n.Id == edge.From);
            var to = nodes.First(n => n.Id == edge.To);

            _graphMap[from].Add(new KeyValuePair<Node, ObjectId>(to, edge.Edge));
        }

        public void RunDijkstra(IDictionary<ObjectId, Weight> weights) {
            var queue = new HashSet<Node>();
            var dist = new Dictionary<Node, Weight>();
            var prev = new Dictionary<Node, Node>();
            foreach (var from in _graphMap.Keys) {
                dist[from] = new Weight();
                prev[from] = null;
                queue.Add(from);
            }

            foreach (var (cityId, weight) in weights) {
                var centralNode = _graphMap.Keys.First(n => n.Id == cityId);
                dist[centralNode] = weight;
            }

            while (queue.Any()) {
                var minDistNode = queue.Aggregate((min, cur) => dist[cur] < dist[min] ? cur : min);
                queue.Remove(minDistNode);
                foreach (var (to, edgeId) in _graphMap[minDistNode]) {
                    if (!queue.Contains(to)) continue;
                    var alt = dist[minDistNode] + _weightFunction.Invoke(new GlobalGraphEdge() {
                        From = minDistNode.Id,
                        Edge = edgeId,
                        To = to.Id
                    });
                    if (alt < dist[to]) {
                        dist[to] = alt;
                        prev[to] = minDistNode;
                    }
                }
            }

            foreach (var node in _graphMap.Keys) {
                node.Weights.Add(new GraphWeight(null, prev[node], null, dist[node]));
                Console.WriteLine(dist[node]);
            }
        }
        
    }

    public class GlobalGraphEdge : IEquatable<GlobalGraphEdge> {
        public ObjectId From { get; set; }
        public ObjectId Edge { get; set; }
        public ObjectId To { get; set; }

        public bool Equals(GlobalGraphEdge other) {
            return other != null && From == other.From && To == other.To && Edge == other.Edge;
        }
    }
}