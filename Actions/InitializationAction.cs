﻿using System;
using System.Windows;

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
        }
    }
}