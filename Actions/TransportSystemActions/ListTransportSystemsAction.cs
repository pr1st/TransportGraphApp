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
            return App.CurrentStates[AppStates.ConnectedToDatabase];
        }

        public void Invoke() {
            var dialog = new ListTransportSystemsDialog() {
                TransportSystemsSupplier = () =>
                    AppDataBase
                        .Instance
                        .GetCollection<TransportSystem>()
                        .FindAll(),
                NumberOfCitiesInTransportSystemSupplier = ts =>
                    AppDataBase
                        .Instance
                        .GetCollection<City>()
                        .Find(c => c.TransportSystemId == ts.Id)
                        .Count(),
                NumberOfRoadsInTransportSystemSupplier = ts =>
                    AppDataBase
                        .Instance
                        .GetCollection<Road>()
                        .Find(r => r.TransportSystemId == ts.Id)
                        .Count()
            };
            dialog.ShowDialog();
            if (dialog.DialogResult != true) return;

            AppCurrentSystem.Instance.Select(dialog.SelectedSystem);
        }
    }
}