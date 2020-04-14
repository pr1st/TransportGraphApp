using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class ListCitiesAction {
        public static void Invoke() {
            var selected = new SelectTransportSystemDialog();
            if (selected.ShowDialog() != true) return;
            
            new ListCitiesDialog(selected.SelectedSystem).ShowDialog();
        }
    }
}