using System;
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
            CurrentState = newState;
            var fileLocation = CurrentState switch {
                AppState.ConnectedToDatabase => $" ({AppDataBase.Instance.DataBaseFileLocation()})",
                AppState.GraphSelected => $" ({AppDataBase.Instance.DataBaseFileLocation()})",
                AppState.Initial => "",
                _ => throw new NotImplementedException()
            };
            if (CurrentState != AppState.GraphSelected) {
                AppGraph.Instance.TransportSystem = null;
            }
            AppWindow.Instance.Title = $"{AppResources.GetAppTitle}{fileLocation}";
            AppActions.Instance.AppStateChanged();
        }

        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, 
                "Exception", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
        }
    }

    public enum AppState {
        Initial,
        ConnectedToDatabase,
        GraphSelected
    }
}