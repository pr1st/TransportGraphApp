using System;
using System.Collections.Generic;
using System.Text;
using TransportGraphApp.Dialogs.TransportSystemDialogs;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Actions.TransportSystemActions {
    public class ListTransportSystemsAction : IAppAction {
        private ListTransportSystemsAction() {
        }

        public bool IsAvailable() {
            return App.CurrentState switch {
                AppState.Initial => false,
                AppState.ConnectedToDatabase => true,
                AppState.GraphSelected => true,
                _ => throw new NotImplementedException()
            };
        }

        public void Invoke() {
            
            var dialog = new ListTransportSystemsDialog(
                () => AppDataBase.Instance.GetCollection<TransportSystem>().FindAll(),
                ts => 0);
            dialog.ShowDialog();
            if (dialog.DialogResult != true) return;

            AppGraph.Instance.TransportSystem = dialog.SelectedSystem;
            App.ChangeAppState(AppState.GraphSelected);
        }
    }
}