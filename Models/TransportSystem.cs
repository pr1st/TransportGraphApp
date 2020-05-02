using System;
using System.Collections.Generic;
using LiteDB;

namespace TransportGraphApp.Models {
    public class TransportSystem : IAppModel, IEquatable<TransportSystem> {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public bool Equals(TransportSystem other) {
            return other != null && Id == other.Id;
        }

        public static IDictionary<string, Func<TransportSystem, object>> PropertyMatcher() {
            return new Dictionary<string, Func<TransportSystem, object>> {
                {
                    "Название",
                    ts => ts.Name
                }, {
                    "Кол-во нас. пунктов",
                    ts => App.DataBase.CountCitiesOfTransportSystem(ts)
                }, {
                    "Кол-во маршрутов",
                    ts => App.DataBase.GetCollection<Road>().Count(r => r.TransportSystemId == ts.Id)
                }
            };
        }
    }
}