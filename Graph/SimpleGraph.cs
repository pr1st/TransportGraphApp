using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using TransportGraphApp.Models;

namespace TransportGraphApp.Graph {
    public class SimpleGraph {
        // arg1 - road id
        // return - weight of this edge
        private readonly Func<ObjectId, Weight> _weightFunction;

        private readonly IDictionary<Node, IList<KeyValuePair<Node, ObjectId>>> _graphMap =
            new Dictionary<Node, IList<KeyValuePair<Node, ObjectId>>>();

        public IList<Node> Results => _graphMap.Keys.ToList();

        public SimpleGraph(IEnumerable<ObjectId> cityIds, Func<ObjectId, Weight> weightFunction) {
            _weightFunction = weightFunction;
            foreach (var id in cityIds) {
                var node = new Node() {
                    Id = id,
                    IsCentral = false
                };
                _graphMap[node] = new List<KeyValuePair<Node, ObjectId>>();
            }
        }

        public void AddEdge(ObjectId fromId, ObjectId toId, ObjectId edgeId) {
            var nodes = _graphMap.Keys;
            var from = nodes.First(n => n.Id == fromId);
            var to = nodes.First(n => n.Id == toId);

            _graphMap[from].Add(new KeyValuePair<Node, ObjectId>(to, edgeId));
        }

        public bool ValidationCheck(IEnumerable<ObjectId> centralCities) {
            var visited = new Dictionary<Node, bool>();
            foreach (var key in _graphMap.Keys) {
                visited[key] = false;
            }

            var stack = new Stack<Node>();
            foreach (var centralCity in centralCities) {
                stack.Push(_graphMap.Keys.First(n => n.Id == centralCity));
            }

            while (stack.Any()) {
                var v = stack.Pop();
                if (visited[v]) continue;

                visited[v] = true;
                foreach (var (next, _) in _graphMap[v]) {
                    if (visited[next]) continue;
                    stack.Push(next);
                }
            }

            return visited.All(kv => kv.Value);
        }

        public void RunDijkstra(IEnumerable<ObjectId> centralCities) {
            var queue = new HashSet<Node>();
            var dist = new Dictionary<Node, Weight>();
            var prev = new Dictionary<Node, Node>();
            foreach (var from in _graphMap.Keys) {
                dist[from] = new Weight();
                prev[from] = null;
                queue.Add(from);
            }

            foreach (var centralCity in centralCities) {
                var centralNode = _graphMap.Keys.First(n => n.Id == centralCity);
                centralNode.IsCentral = true;
                dist[centralNode] = new Weight(0);

                foreach (var list in _graphMap.Values) {
                    var toDelete = list.Where(kv => kv.Key.Id == centralCity).ToList();
                    foreach (var kv in toDelete) {
                        list.Remove(kv);
                    }
                }
            }

            while (queue.Any()) {
                var minDistNode = queue.Aggregate((min, cur) => dist[cur] < dist[min] ? cur : min);
                queue.Remove(minDistNode);
                foreach (var (to, edgeId) in _graphMap[minDistNode]) {
                    if (!queue.Contains(to)) continue;
                    var alt = dist[minDistNode] + _weightFunction.Invoke(edgeId);
                    if (alt < dist[to]) {
                        dist[to] = alt;
                        prev[to] = minDistNode;
                    }
                }
            }

            foreach (var node in _graphMap.Keys) {
                node.Weights.Add(new GraphWeight(null, prev[node], null, dist[node]));
            }
        }
        
        public void RunLocalFirstDijkstra(
            IDictionary<ObjectId, IList<ObjectId>> roads,
            Func<ObjectId, ObjectId> getFrom,
            Func<ObjectId, ObjectId> getTo,
            IDictionary<ObjectId, IList<ObjectId>> allCities, 
            IDictionary<ObjectId, IList<ObjectId>> centralCities,
            IDictionary<ObjectId, IList<ObjectId>> terminalCities) {
            var dictionary = new Dictionary<ObjectId, IDictionary<ObjectId, Node>>();
            foreach (var graphMapKey in _graphMap.Keys) {
                dictionary[graphMapKey.Id] = new Dictionary<ObjectId, Node>();
            }

            foreach (var transportSystem in centralCities.Keys) {
                var road = roads[transportSystem];
                var all = allCities[transportSystem];
                var central = centralCities[transportSystem];
                var terminal = terminalCities[transportSystem];
                foreach (var terminalCity in terminal.Concat(central)) {
                    var simpleGraph = new SimpleGraph(all, _weightFunction);
                    foreach (var r in road) {
                        simpleGraph.AddEdge(getFrom.Invoke(r), getTo.Invoke(r), r);
                    }
                    simpleGraph.RunDijkstra(new[] {terminalCity});
                    foreach (var result in simpleGraph.Results) {
                        dictionary[result.Id][terminalCity] = result;
                    }
                }
            }
            
            var weights = new Dictionary<GlobalGraphEdge, Weight>();
            var globalGraph = new GlobalGraph(terminalCities.Values.Aggregate((l1, l2) => l1.Concat(l2).ToList()), edge => {
                var keyValuePair = weights.First(w => w.Key.Equals(edge));
                return keyValuePair.Value;
            });
            foreach (var weightsKey in weights.Keys) {
                globalGraph.AddEdge(weightsKey);
            }
            globalGraph.RunDijkstra(
                dictionary.Where(w => terminalCities.Keys.Contains(w.Key))
                    .ToDictionary(kv => kv.Key, 
                        kv => kv.Value.First().Value.Weights[0].Weight));

            foreach (var result in globalGraph.Results) {
                foreach (var node in _graphMap.Keys) {
                    node.Weights.Add(new GraphWeight(null, dictionary[node.Id][result.Id], null, result.MinWeight().Weight));
                }   
            }
        }
    }
}