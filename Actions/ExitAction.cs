using System.Windows;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    public static class ExitAction  {
        public static void Invoke() {
            AppDataBase.Instance.Close();
            Application.Current.Shutdown();
        }
    }
}