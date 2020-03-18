using System;
using System.Collections.Generic;
using System.Text;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.TransportSystemActions
{
    public class UpdateTransportSystem : IAppAction
    {
        public bool IsAvailable() {
            return App.CurrentStates[AppStates.ConnectedToDatabase];
        }

        public void Invoke() {
            throw new NotImplementedException();
        }

        public void Invoke(TransportSystem updatedSystem) {
            var collection = AppDataBase.Instance.GetCollection<TransportSystem>();
            collection.Update(updatedSystem);
        }
    }
}
