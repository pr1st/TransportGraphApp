using System;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class RoadTypes : IAppModel {
        public ObjectId Id { get; set; }

        public bool IsPrimary { get; set; } = false;

        public IList<RoadType> Values { get; set; } = new List<RoadType>();
    }

    public class RoadType : IEquatable<RoadType> {
        public string Name { get; set; }

        public bool Equals(RoadType other) {
            return other != null && Name == other.Name;
        }

        public static IDictionary<string, Func<RoadType, object>> PropertyMatcher() {
            return new Dictionary<string, Func<RoadType, object>>() {
                {
                    "Название",
                    rt => rt.Name
                }
            };
        }
    }
}