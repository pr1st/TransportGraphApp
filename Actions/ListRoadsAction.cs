using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class ListRoadsAction {
        public static void Invoke() {
            var selected = new SelectTransportSystemDialog();
            if (selected.ShowDialog() != true) return;

            new ListRoadsDialog(selected.SelectedSystem).ShowDialog();
        }
    }
}