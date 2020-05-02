using System;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class Road : IAppModel, IEquatable<Road> {
        public ObjectId Id { get; set; }

        public ObjectId FromCityId { get; set; }

        public ObjectId ToCityId { get; set; }

        public double Length { get; set; }

        public double Cost { get; set; }

        public double Time { get; set; }

        public RoadType RoadType { get; set; }

        public IList<DepartureTime> DepartureTimes { get; set; } = new List<DepartureTime>();

        public ObjectId TransportSystemId { get; set; }

        public bool Equals(Road other) {
            return other != null && Id == other.Id;
        }

        public static IDictionary<string, Func<Road, object>> PropertyMatcher(
            IDictionary<ObjectId, string> idToNameCitiesMap) {
            return new Dictionary<string, Func<Road, object>>() {
                {"Откуда", r => idToNameCitiesMap[r.FromCityId]},
                {"Куда", r => idToNameCitiesMap[r.ToCityId]},
                {"Расстояние", r => r.Length},
                {"Стоимость", r => r.Cost},
                {"Время", r => r.Time},
                {"Тип дороги", r => r.RoadType.Name},
                {"Кол.во. значений времени отправленя", r => r.DepartureTimes.Count},
            };
        }
    }
}