﻿using TransportGraphApp.Dialogs;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class CreateAndOpenDataBaseFileAction {
        public static void Invoke() {
            var newDataBaseFileDialog = new NewDataBaseFileDialog();
            newDataBaseFileDialog.ShowDialog();
            if (newDataBaseFileDialog.DialogResult != true) return;
            AppDataBase.Instance.Build(newDataBaseFileDialog.NewDataBaseFileName);
            App.ChangeAppState(AppState.ConnectedToDatabase);
        }
    }
}