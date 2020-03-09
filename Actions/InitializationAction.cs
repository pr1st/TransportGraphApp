using System;
using System.Windows;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class InitializationAction {
        public static void Invoke() {
            var defaultFileName = AppResources.GetDefaultDataBasePath;
            try {
                AppDataBase.Instance.Build(defaultFileName);
                App.ChangeAppState(AppState.ConnectedToDatabase);
            }
            catch (Exception) {
                App.ChangeAppState(AppState.Initial);
                ComponentUtils.ShowMessage("Файл базы данных по умолчанию занят другим процессом или испорчен",
                    MessageBoxImage.Error);
            }
        }
    }
}