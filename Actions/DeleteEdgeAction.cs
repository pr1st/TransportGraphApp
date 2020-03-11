//using System;
//using System.Collections.Generic;
//using System.Text;
//using TransportGraphApp.Models;
//using TransportGraphApp.Singletons;

//namespace TransportGraphApp.Actions {
//    internal static class DeleteEdgeAction {
//        public static void Invoke(Edge selectedEdge) {
//            AppDataBase.Instance.GetCollection<Edge>().DeleteMany(e => e.Id == selectedEdge.Id);
//        }
//    }
//}