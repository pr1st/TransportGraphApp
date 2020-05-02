using System;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class City : IAppModel, IEquatable<City> {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public double CostOfStaying { get; set; }

        public IList<CityTag> Tags { get; set; } = new List<CityTag>();

        public IList<ObjectId> TransportSystemIds { get; set; } = new List<ObjectId>();

        public bool Equals(City other) {
            return other != null && Id == other.Id;
        }

        public static IDictionary<string, Func<City, object>> PropertyMatcher() {
            return new Dictionary<string, Func<City, object>>() {
                {"Название", c => c.Name},
                {"Стоимость проживания", c => c.CostOfStaying},
                {"Кол.во Тэгов", c => c.Tags.Count},
                {"Задействован в транспортных системах", c => c.TransportSystemIds.Count},
            };
        }
    }
}