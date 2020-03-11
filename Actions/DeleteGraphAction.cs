//using System;
//using System.Collections.Generic;
//using System.Text;
//using TransportGraphApp.Models;
//using TransportGraphApp.Singletons;

//namespace TransportGraphApp.Actions {
//    internal static class DeleteGraphAction {
//        public static void Invoke(Graph selectedGraph) {
//            AppDataBase.Instance.GetCollection<Graph>().DeleteMany(g => g.Id == selectedGraph.Id);
//            AppDataBase.Instance.GetCollection<Node>().DeleteMany(n => n.GraphId == selectedGraph.Id);
//            AppDataBase.Instance.GetCollection<Edge>().DeleteMany(e => e.GraphId == selectedGraph.Id);
//            if (AppGraph.Instance.Graph != null && AppGraph.Instance.Graph.Id == selectedGraph.Id) {
//                AppGraph.Instance.Graph = null;
//                App.ChangeAppState(AppState.ConnectedToDatabase);
//            }
//        }
//    }
//}