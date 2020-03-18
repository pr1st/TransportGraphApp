using System;
using System.Windows;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class InitializationAction {
        public static void Invoke() {
            var defaultFileName = AppResources.GetDefaultDataBasePath;
            try {
                AppDataBase.Instance.Open(defaultFileName);
            }
            catch (Exception) {
                ComponentUtils.ShowMessage("Файл базы данных по умолчанию (application-data.db) занят другим процессом или испорчен",
                    MessageBoxImage.Error);
            }
        }
    }
}