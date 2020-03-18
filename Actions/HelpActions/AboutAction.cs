using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TransportGraphApp.Actions.HelpActions {
    internal class AboutAction : IAppAction {
        private const string Goal = "Программа разработанна с целбю получения диплома бакалавра";

        private const string Description =
            "Задача приложения предоставить интерфейс и алгоритмы для работы с транспортными системами и вычислением параметра транспортной дискриминации";

        private const string Developer =
            "Дымов Дмитрий Германович, Факультет информационных технологий, Новосибирский государственный университет";

        public bool IsAvailable() => true;

            void IAppAction.Invoke() {
            ComponentUtils.ShowMessage($"{Goal}\n\n{Description}\n\n{Developer}", MessageBoxImage.Information);
        }
    }
}