using System;
using System.IO;
using System.Windows;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class InitializationAction {
        public static void Invoke() {
            var defaultFileName = AppResources.GetDefaultDataBasePath;
            var fileExists = File.Exists(defaultFileName);
            try {
                AppDataBase.Instance.Build(defaultFileName);
                App.ChangeAppState(AppState.ConnectedToDatabase);
                if (!fileExists)
                    ComponentUtils.ShowMessage("Default database file not found, new one was created",
                        MessageBoxImage.Information);
            }
            catch (Exception) {
                App.ChangeAppState(AppState.Initial);
                ComponentUtils.ShowMessage(fileExists
                        ? "Default database file corrupted, delete it or select another database file"
                        : "Something gone wrong, contact me",
                    MessageBoxImage.Error);
            }
        }
    }
}