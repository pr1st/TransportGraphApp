using System;
using System.Collections.Generic;
using System.Text;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.CityActions {
    public class DeleteCityAction : IAppAction {
        private DeleteCityAction() {
        }

        public bool IsAvailable() {
            return App.CurrentState == AppState.GraphSelected;
        }

        public void Invoke() {
            throw new NotImplementedException();
        }

        public void Invoke(City city) {
            var collection = AppDataBase.Instance.GetCollection<TransportSystem>();
            collection.Delete(city.Id);
        }
    }
}