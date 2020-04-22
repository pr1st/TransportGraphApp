using TransportGraphApp.Dialogs;

namespace TransportGraphApp.Actions {
    public static class TaskSpecificationAction {
        public static void Invoke() {
            TaskUpdateConfigDataAction.Invoke();
            new TaskSpecificationDialog().ShowDialog();
        }
    }
}