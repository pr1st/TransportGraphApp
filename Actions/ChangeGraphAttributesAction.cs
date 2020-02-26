using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class ChangeGraphAttributesAction {
        public static void Invoke() {
            Invoke(AppGraph.Instance.Graph);
        }

        public static void Invoke(Graph graph) {
            var changeGraphAttributesDialog = new ChangeGraphAttributesDialog(graph);
            changeGraphAttributesDialog.ShowDialog();
            if (changeGraphAttributesDialog.DialogResult != true) return;
            AppDataBase.Instance.GetCollection<Graph>().Update(graph);
        }
    }
}