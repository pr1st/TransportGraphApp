using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using TransportGraphApp.Actions;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp {
    public partial class App : Application {
        private void ApplicationStartup(object sender, StartupEventArgs e) {
            Exit += (o, args) => ExitAction.Invoke();
            Window.Show();
            InitializationAction.Invoke();
            // DataBase.GetCollection<AlgorithmResult>().DeleteAll();
        }

        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show("Возникло исключение: " + e.Exception.Message + "\n Приложение будет закрыто", 
                "Исключение", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            ExitAction.Invoke();
        }
        
        public static AppWindow Window { get; } = new AppWindow();
        
        public static ReportSaver ReportSaver { get; } = new ReportSaver();
        
        public static AppDataBase DataBase { get; } = new AppDataBase();
        
        public static AppAlgorithm Algorithm { get; } = new AppAlgorithm();
    }
}