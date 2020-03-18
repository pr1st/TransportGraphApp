using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using TransportGraphApp.Actions;
using TransportGraphApp.Actions.UtilActions;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp {
    public partial class App : Application {
        private void ApplicationStartup(object sender, StartupEventArgs e) {
            foreach (var stateType in Enum.GetValues(typeof(AppStates))) {
                CurrentStates.Add((AppStates) stateType, false);
            }

            var mainWindow = AppWindow.Instance;
            mainWindow.Show();
            InitializationAction.Invoke();
        }

        public static readonly IDictionary<AppStates, bool> CurrentStates = new Dictionary<AppStates, bool>();

        public static void ChangeAppState(AppStates state, bool isTrue) {
            CurrentStates[state] = isTrue;
            if (!CurrentStates[AppStates.ConnectedToDatabase])
                CurrentStates[AppStates.TransportSystemSelected] = false;

            var fileLocation = "";
            if (CurrentStates[AppStates.ConnectedToDatabase]) {
                fileLocation = $" ({AppDataBase.Instance.DataBaseFileLocation()})";
            }
            
            AppWindow.Instance.Title = $"{AppResources.GetAppTitle}{fileLocation}";
            AppActions.Instance.AppStateChanged();
            AppWindow.Instance.DrawGraph();
        }

        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, 
                "Exception", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            AppActions.Instance.GetAction<ExitAction>().Invoke();
        }
    }

    public enum AppStates {
        ConnectedToDatabase,
        TransportSystemSelected
    }
}