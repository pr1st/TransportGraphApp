using System;
using System.Linq;
using System.Windows;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    internal static class InitializationAction {
        public static void Invoke() {
            var defaultFileName = AppResources.GetDefaultDataBasePath;
            try {
                App.DataBase.Open(defaultFileName);
            }
            catch (Exception) {
                ComponentUtils.ShowMessage("Файл базы данных по умолчанию (application-data.db) занят другим процессом или испорчен\n" +
                                           "Освободите его если он занят или удалите и создастся новый при повторном запуске приложения\n" +
                                           "Приложение будет закрыто",
                    MessageBoxImage.Error);
                ExitAction.Invoke();
            }
            
            AddNeededDataIfNotExistedBefore();
            PrintDataState();
        }

        private static void AddNeededDataIfNotExistedBefore() {
            if (!App.DataBase.GetCollection<AlgorithmConfig>().Find(a => a.IsPrimary).Any()) {
                App.DataBase.GetCollection<AlgorithmConfig>().Insert(AlgorithmConfig.GetDefault);
            }
            
            if (!App.DataBase.GetCollection<CityTags>().Find(ct => ct.IsPrimary).Any()) {
                App.DataBase.GetCollection<CityTags>().Insert(new CityTags() {IsPrimary = true});
            }

            if (!App.DataBase.GetCollection<RoadTypes>().Find(rt => rt.IsPrimary).Any()) {
                App.DataBase.GetCollection<RoadTypes>().Insert(new RoadTypes() {IsPrimary = true});
            }
        }

        private static void PrintDataState() {
            Console.WriteLine($"Transport systems in database: {App.DataBase.GetCollection<TransportSystem>().Count()}");
            Console.WriteLine($"Cities in database: {App.DataBase.GetCollection<City>().Count()}");
            Console.WriteLine($"Roads in database: {App.DataBase.GetCollection<Road>().Count()}");
            Console.WriteLine($"City tags in database: {App.DataBase.GetCollection<CityTags>().FindOne(ct => ct.IsPrimary).Values.Count}");
            Console.WriteLine($"Road types in database: {App.DataBase.GetCollection<RoadTypes>().FindOne(rt => rt.IsPrimary).Values.Count}");
            Console.WriteLine($"Algorithm results: {App.DataBase.GetCollection<AlgorithmResult>().Count()}");
        }
    }
}