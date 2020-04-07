using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class GraphParametersAction {
        public static void Invoke() {
            new GraphParametersDialog().ShowDialog();
        }
    }
}