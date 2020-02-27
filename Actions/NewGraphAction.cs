using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using LiteDB;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class NewGraphAction {
        public static void Invoke() {
            var newGraphDialog = new NewGraphDialog();
            newGraphDialog.ShowDialog();
            if (newGraphDialog.DialogResult != true) return;

            AppDataBase.Instance.GetCollection<Graph>().Insert(new Graph() {
                Name = newGraphDialog.GraphName,
                GraphAttributes = newGraphDialog.GraphAttributes,
                DefaultNodeAttributes = newGraphDialog.NodeAttributes,
                DefaultEdgeAttributes = newGraphDialog.EdgeAttributes
            });
        }
    }
}