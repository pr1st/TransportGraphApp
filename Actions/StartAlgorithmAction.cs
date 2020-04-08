using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class StartAlgorithmAction {
        public static void Invoke() {
            new StartAlgorithmDialog().ShowDialog();
        }
    }
}