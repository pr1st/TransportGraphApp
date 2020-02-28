using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class NewEdgeAction {
        public static void Invoke() {
            Invoke(AppGraph.Instance.Graph);
        }

        public static void Invoke(Graph g) {
            var newEdgeDialog = new NewEdgeDialog(g);
            newEdgeDialog.ShowDialog();
            if (newEdgeDialog.DialogResult != true) return;
            AppDataBase.Instance.GetCollection<Edge>().Insert(newEdgeDialog.CreatedEdge);
        }
    }
}