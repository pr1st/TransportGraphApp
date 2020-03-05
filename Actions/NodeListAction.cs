using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class NodeListAction {
        public static void Invoke() {
            Invoke(AppGraph.Instance.Graph);
        }

        public static void Invoke(Graph g) {
            var nodes = AppDataBase.Instance.GetCollection<Node>();
            nodes.EnsureIndex(n => n.GraphId);

            var nodeListDialog = new NodeListDialog(g, () => nodes.Find(n => n.GraphId == g.Id));
            nodeListDialog.ShowDialog();
        }
    }
}