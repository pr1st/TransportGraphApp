using System.Windows;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.UtilActions {
    public class ExitAction : IAppAction {
        private ExitAction() {
        }

        public bool IsAvailable() => true;

        public void Invoke() {
            AppDataBase.Instance.Close();
            Application.Current.Shutdown();
        }
    }
}