using System;
using System.Collections.Generic;
using System.Linq;
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
                () => AppDataBase
                    .Instance
                    .GetCollection<TransportSystem>()
                    .FindAll(),
                ts => AppDataBase
                    .Instance
                    .GetCollection<City>()
                    .Find(c => c.TransportSystemId == AppGraph.Instance.TransportSystem.Id)
                    .Count());
            dialog.ShowDialog();
            if (dialog.DialogResult != true) return;

            AppGraph.Instance.TransportSystem = dialog.SelectedSystem;
            App.ChangeAppState(AppState.GraphSelected);
        }
    }
}