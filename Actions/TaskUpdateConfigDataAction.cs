using System;
using System.Collections.Generic;
using System.Linq;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class TaskUpdateConfigDataAction {
        public static void Invoke() {
            var config = App.DataBase.GetCollection<AlgorithmConfig>().FindOne(a => a.IsPrimary);
            
            var cityTags = App.DataBase.GetCollection<CityTags>().FindOne(ct => ct.IsPrimary).Values;
            var roadTypes = App.DataBase.GetCollection<RoadTypes>().FindOne(rt => rt.IsPrimary).Values;
            
            var trash0 = new List<TransportSystem>();
            foreach (var ts in config.TransportSystems) {
                var tsFound = App.DataBase.GetCollection<TransportSystem>().FindById(ts.Id);
                if (tsFound != null) {
                    ts.Name = tsFound.Name;
                }
                else {
                    trash0.Add(ts);
                }
            }
            
            var trash1 = config.CityTags
                .Where(ct => !cityTags.Contains(ct))
                .ToList();

            var trash2 = config.RoadTypes
                .Where(rt => !roadTypes.Contains(rt))
                .ToList();

            foreach (var transportSystem in trash0) {
                config.TransportSystems.Remove(transportSystem);
            }
            
            foreach (var cityTag in trash1) {
                config.CityTags.Remove(cityTag);
            }
            
            foreach (var roadType in trash2) {
                config.RoadTypes.Remove(roadType);
            }

            App.DataBase.GetCollection<AlgorithmConfig>().Update(config);
        }
    }
}