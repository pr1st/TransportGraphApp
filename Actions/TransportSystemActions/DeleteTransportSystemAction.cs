using System;
using System.Collections.Generic;
using System.Text;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.TransportSystemActions {
    public class DeleteTransportSystemAction : IAppAction {
        private DeleteTransportSystemAction() {
        }

        public bool IsAvailable() {
            return App.CurrentState switch {
                AppState.Initial => false,
                AppState.ConnectedToDatabase => true,
                AppState.GraphSelected => true,
                _ => throw new NotImplementedException()
            };
        }

        public void Invoke() {
            throw new NotImplementedException();
        }

        public void Invoke(TransportSystem ts) {
            var collection = AppDataBase.Instance.GetCollection<TransportSystem>();
            collection.Delete(ts.Id);
        }
    }
}