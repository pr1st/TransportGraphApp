using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class TaskResultsAction {
        public static void Invoke() {
            new TaskResultsDialog().ShowDialog();
        }
    }
}