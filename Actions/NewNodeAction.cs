using System.Linq;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class NewNodeAction {
        public static void Invoke() {
            Invoke(AppGraph.Instance.Graph);
        }

        public static void Invoke(Graph g) {
            var nodes = AppDataBase.Instance.GetCollection<Node>();
            nodes.EnsureIndex(n => n.Name);
            nodes.EnsureIndex(n => n.GraphId);

            var newNodeDialog = new NewNodeDialog(g,
                nodes
                    .Find(n => n.GraphId == g.Id)
                    .Select(n => n.Name));
            newNodeDialog.ShowDialog();
            if (newNodeDialog.DialogResult != true) return;


            nodes.Insert(newNodeDialog.CreatedNode);
        }
    }
}