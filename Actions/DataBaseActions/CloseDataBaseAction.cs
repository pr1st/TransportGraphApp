using System;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.DataBaseActions {
    public class CloseDataBaseAction : IAppAction {
        private CloseDataBaseAction() {
        }

        public bool IsAvailable() {
            return App.CurrentStates[AppStates.ConnectedToDatabase];
        }

        public void Invoke() {
            AppDataBase.Instance.Close();
        }
    }
}