using System;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class CityTags : IAppModel {
        public ObjectId Id { get; set; }

        public bool IsPrimary { get; set; } = false;

        public IList<CityTag> Values { get; set; } = new List<CityTag>();
    }

    public class CityTag : IEquatable<CityTag> {
        public string Name { get; set; }

        public bool Equals(CityTag other) {
            return other != null && Name == other.Name;
        }

        public static IDictionary<string, Func<CityTag, object>> PropertyMatcher() {
            return new Dictionary<string, Func<CityTag, object>>() {
                {
                    "Название",
                    ct => ct.Name
                }
            };
        }
    }
}