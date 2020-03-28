using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class ListRoadsAction {
        public static void Invoke() {
            new ListRoadsDialog().ShowDialog();
        }
    }
}