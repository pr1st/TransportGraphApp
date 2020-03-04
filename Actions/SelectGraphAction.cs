using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class SelectGraphAction {
        public static void Invoke() {
            var selectGraphDialog = new SelectGraphDialog(
                () => AppDataBase.Instance.GetCollection<Graph>().FindAll(),
                AppGraph.Instance.Graph);
            selectGraphDialog.ShowDialog();
            if (selectGraphDialog.DialogResult != true) return;

            AppGraph.Instance.Graph = selectGraphDialog.SelectedGraph;
            App.ChangeAppState(AppState.GraphSelected);
        }
    }
}