using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.DataBaseActions {
    public class OpenDataBaseAction : IAppAction {
        private OpenDataBaseAction() {
        }

        public bool IsAvailable() => true;

        public void Invoke() {
            var openFileDialog = new OpenFileDialog {
                Filter = "Файлы базы данных (*.db)|*.db",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            if (openFileDialog.ShowDialog() != true) return;

            Invoke(openFileDialog.FileName);
        }

        public void Invoke(string filePath) {
            try {
                AppDataBase.Instance.Build(filePath);
                App.ChangeAppState(AppState.ConnectedToDatabase);
            }
            catch (Exception) {
                ComponentUtils.ShowMessage("Выбранный файл поврежден или занят другим процессом",
                    MessageBoxImage.Error);
            }
        }
    }
}