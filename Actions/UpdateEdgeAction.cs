using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class UpdateEdgeAction {

        public static void Invoke(Edge edge) {
            var updateEdgeDialog = new UpdateEdgeDialog(edge);
            updateEdgeDialog.ShowDialog();
            if (updateEdgeDialog.DialogResult != true) return;
            AppDataBase.Instance.GetCollection<Edge>().Update(edge);
        }
    }
}