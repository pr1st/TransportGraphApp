using System;
using System.Collections.Generic;
using System.Text;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.CityActions {
    public class CreateCityAction : IAppAction {
        private CreateCityAction() {
        }

        public bool IsAvailable() {
            return App.CurrentStates[AppStates.TransportSystemSelected];
        }

        public void Invoke() {
            throw new NotImplementedException();
        }

        public void Invoke(City city) {
            var collection = AppDataBase.Instance.GetCollection<City>();
            collection.Insert(city);
        }
    }
}