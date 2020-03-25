using TransportGraphApp.Dialogs.DataBaseDialogs;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.DataBaseActions {
    public class CreateDataBaseAction : IAppAction {
        private CreateDataBaseAction() {
        }

        public bool IsAvailable() => true;

        public void Invoke() {
            var dialog = new CreateDataBaseFileDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult != true) return;

            AppDataBase.Create(dialog.NewDataBaseFileName);

            if (dialog.OpenAfterCreation) {
                AppActions.Instance.GetAction<OpenDataBaseAction>().Invoke(dialog.NewDataBaseFileName);
            }
        }
    }
}