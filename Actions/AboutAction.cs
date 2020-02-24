using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TransportGraphApp.Actions {
    internal static class AboutAction {
        private const string Goal = "This application was developed for bachelor's degree.";
        private const string Description = "Application goal is to provide UI for creating graphs and visualizing algorithms.";
        private const string Developer = "Dmitry Dymov, Faculty of Information Technology, Novosibirsk State University.";

        public static void Invoke() {
            ComponentUtils.ShowMessage($"{Goal}\n\n{Description}\n\n{Developer}", MessageBoxImage.Information);
        }
    }
}