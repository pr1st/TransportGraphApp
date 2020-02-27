using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class NewNodeAction {
        public static void Invoke() {
            Invoke(AppGraph.Instance.Graph);
        }

        public static void Invoke(Graph g) {
            var newNodeDialog = new NewNodeDialog(g);
            newNodeDialog.ShowDialog();
            if (newNodeDialog.DialogResult != true) return;
            AppDataBase.Instance.GetCollection<Node>().Insert(newNodeDialog.CreatedNode);
        }
    }
}