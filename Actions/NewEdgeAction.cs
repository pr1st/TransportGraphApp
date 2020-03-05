using System.Linq;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class NewEdgeAction {
        public static void Invoke() {
            Invoke(AppGraph.Instance.Graph);
        }

        public static void Invoke(Graph g) {
            var nodes = AppDataBase.Instance.GetCollection<Node>();
            nodes.EnsureIndex(n => n.GraphId);
            var dictionary = nodes
                .Find(n => n.GraphId == g.Id)
                .ToDictionary(n => n.Name, n => n.Id);

            var newEdgeDialog = new NewEdgeDialog(g, dictionary);
            newEdgeDialog.ShowDialog();
            if (newEdgeDialog.DialogResult != true) return;

            AppDataBase.Instance.GetCollection<Edge>().Insert(newEdgeDialog.CreatedEdge);
        }
    }
}