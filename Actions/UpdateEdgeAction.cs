//using System.Linq;
//using TransportGraphApp.Dialogs;
//using TransportGraphApp.Models;
//using TransportGraphApp.Singletons;

//namespace TransportGraphApp.Actions {
//    internal static class UpdateEdgeAction {

//        public static void Invoke(Edge edge) {
//            var nodes = AppDataBase.Instance.GetCollection<Node>();
//            nodes.EnsureIndex(n => n.GraphId);
//            var dictionary = nodes
//                .Find(n => n.GraphId == edge.GraphId)
//                .ToDictionary(n => n.Name, n => n.Id);

//            var updateEdgeDialog = new UpdateEdgeDialog(edge, dictionary);
//            updateEdgeDialog.ShowDialog();
//            if (updateEdgeDialog.DialogResult != true) return;

//            AppDataBase.Instance.GetCollection<Edge>().Update(updateEdgeDialog.UpdatedEdge);
//        }
//    }
//}