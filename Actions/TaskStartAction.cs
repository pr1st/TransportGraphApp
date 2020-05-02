using System;
using System.Windows;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class TaskStartAction {
        public static void Invoke() {
            TaskUpdateConfigDataAction.Invoke();
            var cfg = App.DataBase.GetCollection<AlgorithmConfig>().FindOne(a => a.IsPrimary);
            var completed = App.Algorithm.CheckTransportSystems(cfg);
            if (!completed) {
                ComponentUtils.ShowMessage("Проверка данных показала отрицательный ответ \n" +
                                           "У транспортных сетей нету необходимой связности для работы алгоритма", 
                    MessageBoxImage.Information);
                return;
            }

            var algorithmResult = App.Algorithm.Run(cfg);
            
            if (algorithmResult == null) {
                ComponentUtils.ShowMessage("Выбранный метод работы алгоритма или тип алгоритма еще не поддерживается", MessageBoxImage.Information);
                return;
            }

            App.DataBase.GetCollection<AlgorithmResult>().Insert(algorithmResult);
            ComponentUtils.ShowMessage("Поставленная задача была выполнена, результаты выполнения, можно посмотреть во вкладке \"Результаты расчетов\"", MessageBoxImage.Information);
        }
    }
}