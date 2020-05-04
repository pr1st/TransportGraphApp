using System;
using System.Collections.Generic;
using LiteDB;
using TransportGraphApp.Graph;

namespace TransportGraphApp.Models {
    public class AlgorithmResult : IAppModel, IEquatable<AlgorithmResult> {
        public ObjectId Id { get; set; }

        public DateTime RunDate { get; set; }

        public AlgorithmConfig AlgorithmConfig { get; set; }

        public IList<Node> Nodes { get; set; } = new List<Node>();

        
        public bool Equals(AlgorithmResult other) {
            return other != null && Id == other.Id;
        }

        public static IDictionary<string, Func<AlgorithmResult, object>> PropertyMatcher() {
            return new Dictionary<string, Func<AlgorithmResult, object>>() {
                {
                    "Дата выполнения",
                    res => res.RunDate
                }, {
                    "Метод",
                    res => res.AlgorithmConfig.MethodType.GetDescription()
                }, {
                    "Алгоритм",
                    res => res.AlgorithmConfig.AlgorithmType.GetDescription()
                }, {
                    "Кол-во. нас. пунктов",
                    res => res.Nodes.Count
                },
            };
        }
    }
}