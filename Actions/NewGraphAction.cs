//using System.Linq;
//using TransportGraphApp.Dialogs;
//using TransportGraphApp.Models;
//using TransportGraphApp.Singletons;

//namespace TransportGraphApp.Actions {
//    internal static class NewGraphAction {
//        public static void Invoke() {
//            var graphCollection = AppDataBase.Instance.GetCollection<Graph>();
//            graphCollection.EnsureIndex(g => g.Name);
//            var alreadyUsedGraphNames = graphCollection.FindAll().Select(g => g.Name);

//            var newGraphDialog = new NewGraphDialog(alreadyUsedGraphNames);
//            newGraphDialog.ShowDialog();
//            if (newGraphDialog.DialogResult != true) return;

//            AppDataBase.Instance.GetCollection<Graph>().Insert(newGraphDialog.Graph);
//        }
//    }
//}