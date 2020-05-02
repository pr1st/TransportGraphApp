using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class ListTransportSystemsAction {
        // updateable properties
        private static IList<TransportSystem> _transportSystemsList;

        // ui controls
        private static GenericEntityDialog<TransportSystem> _dialog;
        private static StringRowControl _nameControl;

        public static void Invoke() {
            _dialog = new GenericEntityDialog<TransportSystem>() {
                Title = "Транспортные системы",
                ListTitle = "Доступные транспортные системы",
                OpenAddNewItemWindowButtonTitle = "Открыть окно для добавления транспортной системы",
                AddNewItemWindowTitle = "Добавить транспортную систему",
                UpdateItemWindowTitle = "Обновить транспортную систему",
                AddItemFunction = AddTransportSystem,
                UpdateItemFunction = UpdateTransportSystem,
                RemoveItemFunction = RemoveTransportSystem,
                UpdateCollectionFunction = UpdateCollection
            };

            _dialog.AddColumns(TransportSystem.PropertyMatcher());

            InitNameProperty();

            UpdateCollection();
            _dialog.ShowDialog();
        }

        // callback methods
        private static bool AddTransportSystem() {
            if (!IsViable(null)) return false;

            App.DataBase.GetCollection<TransportSystem>().Insert(new TransportSystem() {
                Name = _nameControl.Value,
            });

            return true;
        }

        private static bool UpdateTransportSystem(TransportSystem selected) {
            if (!IsViable(null)) return false;

            selected.Name = _nameControl.Value;

            App.DataBase.GetCollection<TransportSystem>().Update(selected);
            ComponentUtils.ShowMessage("Данная транспортная система была обновлена", MessageBoxImage.Information);
            return true;
        }

        private static bool RemoveTransportSystem(TransportSystem selected) {
            App.DataBase.GetCollection<Road>().DeleteMany(r => r.TransportSystemId == selected.Id);
            
            var cities = App.DataBase.GetCitiesOfTransportSystem(selected)
                .Where(c => c.TransportSystemIds.Contains(selected.Id));
            foreach (var city in cities) {
                if (city.TransportSystemIds.Count == 1) {
                    App.DataBase.GetCollection<City>().Delete(city.Id);
                }
                else {
                    city.TransportSystemIds.Remove(selected.Id);
                    App.DataBase.GetCollection<City>().Update(city);
                }
            }

            App.DataBase.GetCollection<TransportSystem>().Delete(selected.Id);
            
            return true;
        }

        // support method for callback methods
        private static bool IsViable(string previousName) {
            if (_nameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите не пустое название транспортной системы", MessageBoxImage.Error);
                return false;
            }

            if (_transportSystemsList.Select(с => с.Name).Contains(_nameControl.Value)
                && previousName != _nameControl.Value) {
                ComponentUtils.ShowMessage("Транспортная система с таким названием уже существует",
                    MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        // init methods
        private static void InitNameProperty() {
            _nameControl = new StringRowControl() {
                TitleValue = "Название",
                TitleToolTip = "Представляет собой уникальный индетификатор транспортной системы"
            };
            _dialog.AddProperty(
                _nameControl,
                () => _nameControl.Value = "",
                ts => _nameControl.Value = ts.Name);
        }
        
        // update state method
        private static IEnumerable<TransportSystem> UpdateCollection() {
            _transportSystemsList = App.DataBase.GetCollection<TransportSystem>().FindAll().ToList();
            return _transportSystemsList;
        }
    }
}