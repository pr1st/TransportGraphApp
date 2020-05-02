using System.Windows;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class TaskCheckDataAction {
        public static void Invoke() {
            TaskUpdateConfigDataAction.Invoke();
            var cfg = App.DataBase.GetCollection<AlgorithmConfig>().FindOne(a => a.IsPrimary);
            var completed = App.Algorithm.CheckTransportSystems(cfg);
            if (!completed) {
                ComponentUtils.ShowMessage("Проверка отрицательная \n" +
                                           "У указанных в концигурации транспортных сетей нету необходимой связности для работы алгоритма", 
                    MessageBoxImage.Information);
            }
            else {
                ComponentUtils.ShowMessage("Проверка положительная\n" +
                                           "Поставленная задача корректна и может быть обработанна алгоритмом\n",
                    MessageBoxImage.Information);
            }
        }
    }
}