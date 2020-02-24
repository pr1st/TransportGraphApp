using System.Windows;
using System.Windows.Threading;
using TransportGraphApp.Actions;
using TransportGraphApp.Singletons;

namespace TransportGraphApp {
    public partial class App : Application {
        private void ApplicationStartup(object sender, StartupEventArgs e) {
            var mainWindow = AppWindow.Instance;
            mainWindow.Show();
            InitializationAction.Invoke();
        }

        public static AppState CurrentState = AppState.Initial;

        public static void ChangeAppState(AppState newState) {
            AppWindow.Instance.AppStateChanged(newState);
            CurrentState = newState;
        }

        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, 
                "Exception", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            e.Handled = true;
        }
    }

    public enum AppState {
        Initial,
        ConnectedToDatabase,
        GraphSelected
    }
}