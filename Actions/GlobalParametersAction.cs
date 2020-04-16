using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class GlobalParametersAction {
        public static void Invoke() {
            new GlobalParametersDialog().ShowDialog();
        }
    }
}