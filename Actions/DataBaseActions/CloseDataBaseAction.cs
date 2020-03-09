using System;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.DataBaseActions {
    public class CloseDataBaseAction : IAppAction {
        private CloseDataBaseAction() {
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
            AppDataBase.Instance.Close();
            App.ChangeAppState(AppState.Initial);
        }
    }
}