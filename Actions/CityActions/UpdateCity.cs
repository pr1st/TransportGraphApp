using System;
using System.Collections.Generic;
using System.Text;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.CityActions {
    public class UpdateCity : IAppAction {
        private UpdateCity() {
        }

        public bool IsAvailable() {
            return App.CurrentState == AppState.GraphSelected;
        }

        public void Invoke() {
            throw new NotImplementedException();
        }

        public void Invoke(City updatedCity) {
            var collection = AppDataBase.Instance.GetCollection<City>();
            collection.Update(updatedCity);
        }
    }
}