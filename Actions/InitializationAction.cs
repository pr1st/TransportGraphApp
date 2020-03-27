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
                ComponentUtils.ShowMessage("Файл базы данных по умолчанию (application-data.db) занят другим процессом или испорчен\n" +
                                           "Освободите его если он занят или удалите и создастся новый при повторном запуске приложения\n" +
                                           "Приложение будет закрыто",
                    MessageBoxImage.Error);
                ExitAction.Invoke();
            }
        }
    }
}