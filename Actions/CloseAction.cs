using System.Windows;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class CloseAction {
        public static void Invoke() {
            AppDataBase.Instance.Close();
            Application.Current.Shutdown();
        }
    }
}