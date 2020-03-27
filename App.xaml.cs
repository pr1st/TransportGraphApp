using System.Windows;
using System.Windows.Threading;
using TransportGraphApp.Actions;
using TransportGraphApp.Singletons;

namespace TransportGraphApp {
    public partial class App : Application {
        private void ApplicationStartup(object sender, StartupEventArgs e) {
            var mainWindow = AppWindow.Instance;
            AppGraph.Instance.SelectSystem(null);
            mainWindow.Show();
            InitializationAction.Invoke();
        }
        
        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show("Возникло исключение: " + e.Exception.Message + "\n Приложение будет закрыто", 
                "Исключение", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            ExitAction.Invoke();
        }
    }
}