using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class ListCitiesAction {
        public static void Invoke() {
            new ListCitiesDialog().ShowDialog();
        }
    }
}