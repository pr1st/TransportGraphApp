using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class ListTransportSystemsAction {
        public static void Invoke() {
            new ListTransportSystemsDialog().ShowDialog();
        }
    }
}