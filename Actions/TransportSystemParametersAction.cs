using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class TransportSystemParametersAction {
        public static void Invoke() {
            new TransportSystemParametersDialog().ShowDialog();
        }
    }
}