using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class SelectGraphAction {
        public static void Invoke() {
            var selectGraphDialog = new SelectGraphDialog();
            selectGraphDialog.ShowDialog();
            if (selectGraphDialog.DialogResult != true) {
                if (AppGraph.Instance.Graph == null || AppDataBase.Instance.GetCollection<Graph>()
                    .Exists(g => g.Id == AppGraph.Instance.Graph.Id)) return;
                AppGraph.Instance.Graph = null;
                App.ChangeAppState(AppState.ConnectedToDatabase);

                return;
            }

            AppGraph.Instance.Graph = selectGraphDialog.SelectedGraph;
            App.ChangeAppState(AppState.GraphSelected);
        }
    }
}