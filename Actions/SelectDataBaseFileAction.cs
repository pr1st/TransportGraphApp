using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class SelectDataBaseFileAction {
        public static void Invoke() {
            var openFileDialog = new OpenFileDialog {
                Filter = "Database files (*.db)|*.db",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (openFileDialog.ShowDialog() != true) return;

            try {
                AppDataBase.Instance.Build(openFileDialog.FileName);
                App.ChangeAppState(AppState.ConnectedToDatabase);
            }
            catch (Exception) {
                App.ChangeAppState(AppState.Initial);
                ComponentUtils.ShowMessage("Selected database file corrupted", MessageBoxImage.Error);
            }
        }
    }
}