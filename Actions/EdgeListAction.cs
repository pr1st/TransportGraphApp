using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class EdgeListAction {
        public static void Invoke()
        {
            Invoke(AppGraph.Instance.Graph);
        }
        public static void Invoke(Graph g)
        {
            var edgeListDialog = new EdgeListDialog(g);
            edgeListDialog.ShowDialog();
        }
    }
}