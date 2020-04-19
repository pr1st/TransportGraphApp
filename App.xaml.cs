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
            AddNeededDataIfNotExistedBefore();
            
            Console.WriteLine($"Transport systems in database: {DataBase.GetCollection<TransportSystem>().Count()}");
            Console.WriteLine($"Cities in database: {DataBase.GetCollection<City>().Count()}");
            Console.WriteLine($"Roads in database: {DataBase.GetCollection<Road>().Count()}");
            Console.WriteLine($"City tags in database: {DataBase.GetCollection<CityTags>().FindOne(ct => ct.IsPrimary).Values.Count}");
            Console.WriteLine($"Road types in database: {DataBase.GetCollection<RoadTypes>().FindOne(rt => rt.IsPrimary).Values.Count}");
        }

        private void AddNeededDataIfNotExistedBefore() {
            if (!DataBase.GetCollection<AlgorithmConfig>().Find(a => a.IsPrimary).Any()) {
                DataBase.GetCollection<AlgorithmConfig>().Insert(AlgorithmConfig.GetDefault);
            }
            
            if (!DataBase.GetCollection<CityTags>().Find(ct => ct.IsPrimary).Any()) {
                DataBase.GetCollection<CityTags>().Insert(new CityTags() {IsPrimary = true});
            }

            if (!DataBase.GetCollection<RoadTypes>().Find(rt => rt.IsPrimary).Any()) {
                DataBase.GetCollection<RoadTypes>().Insert(new RoadTypes() {IsPrimary = true});
            }
        }

        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show("Возникло исключение: " + e.Exception.Message + "\n Приложение будет закрыто", 
                "Исключение", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
            ExitAction.Invoke();
        }
        
        public static AppWindow Window { get; } = new AppWindow();
        
        public static AppDataBase DataBase { get; } = new AppDataBase();
        
        public static AppAlgorithm Algorithm { get; } = new AppAlgorithm();
    }
}