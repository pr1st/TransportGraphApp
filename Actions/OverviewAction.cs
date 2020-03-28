using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class OverviewAction {
        public static void Invoke() {
            new OverviewDialog().ShowDialog();
        }
    }
}