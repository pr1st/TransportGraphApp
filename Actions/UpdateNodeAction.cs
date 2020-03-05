using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions {
    internal static class UpdateNodeAction {
        public static void Invoke(Node n) {
            var updateNodeDialog = new UpdateNodeDialog(n);
            updateNodeDialog.ShowDialog();
            if (updateNodeDialog.DialogResult != true) return;

            AppDataBase.Instance.GetCollection<Node>().Update(updateNodeDialog.UpdatedNode);
        }
    }
}