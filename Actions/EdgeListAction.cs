//using System.Linq;
//using TransportGraphApp.Dialogs;
//using TransportGraphApp.Models;
//using TransportGraphApp.Singletons;

//namespace TransportGraphApp.Actions {
//    internal static class EdgeListAction {
//        public static void Invoke() {
//            Invoke(AppGraph.Instance.Graph);
//        }

//        public static void Invoke(Graph g) {
//            var edges = AppDataBase.Instance.GetCollection<Edge>();
//            edges.EnsureIndex(e => e.GraphId);

//            var nodes = AppDataBase.Instance.GetCollection<Node>();
//            nodes.EnsureIndex(n => n.GraphId);
//            var dictionary = nodes
//                .Find(n => n.GraphId == g.Id)
//                .ToDictionary(n => n.Id, n => n.Name);

//            var edgeListDialog = new EdgeListDialog(g,
//                () => edges.Find(e => e.GraphId == g.Id),
//                dictionary);
//            edgeListDialog.ShowDialog();
//        }
//    }
//}