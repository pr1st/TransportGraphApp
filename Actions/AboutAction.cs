using System.Windows;

namespace TransportGraphApp.Actions {
    internal static class AboutAction {
        private const string Goal = "Программа разработанна с целбю получения диплома бакалавра";

        private const string Description =
            "Задача приложения предоставить интерфейс и алгоритмы для работы с транспортными системами и вычислением параметра транспортной дискриминации";

        private const string Developer =
            "Дымов Дмитрий Германович, Факультет информационных технологий, Новосибирский государственный университет";
        
        public static void Invoke() {
            ComponentUtils.ShowMessage($"{Goal}\n\n{Description}\n\n{Developer}", MessageBoxImage.Information);
        }
    }
}