using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class ListTransportSystemsAction {
        public static void Invoke() {
            var genericEntityDialog = new GenericEntityDialog<TransportSystem>() {
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

            genericEntityDialog.AddColumn(
                "Название",
                ts => ts.Name);
            genericEntityDialog.AddColumn(
                "Кол-во нас. пунктов",
                ts => {
                    // todo whats happening? check when cities are done
                    return App.DataBase.GetCollection<City>().Count(c => c.TransportSystemIds.Count(tsId => tsId == ts.Id) == 1);
                });
            genericEntityDialog.AddColumn(
                "Кол-во маршрутов",
                ts => App.DataBase.GetCollection<Road>().Count(r => r.TransportSystemId == ts.Id));

            _nameControl = new StringRowControl() {
                TitleValue = "Название",
                TitleToolTip = "Представляет собой уникальный индетификатор транспортной системы"
            };

            genericEntityDialog.AddProperty(
                _nameControl,
                () => _nameControl.Value = "",
                ts => _nameControl.Value = ts.Name);

            genericEntityDialog.ShowDialog();
        }

        private static IList<TransportSystem> _systemsList;

        private static StringRowControl _nameControl;

        private static bool AddTransportSystem() {
            if (_nameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите название транспортной системы", MessageBoxImage.Error);
                return false;
            }

            if (_systemsList.Select(ts => ts.Name).Contains(_nameControl.Value)) {
                ComponentUtils.ShowMessage("Система с таким названием уже существует", MessageBoxImage.Error);
                return false;
            }

            App.DataBase.GetCollection<TransportSystem>().Insert(new TransportSystem() {
                Name = _nameControl.Value,
            });
            
            return true;
        }

        private static bool UpdateTransportSystem(TransportSystem selected) {
            if (_nameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите не пустое название транспортной системы", MessageBoxImage.Error);
                return false;
            }

            if (selected.Name != _nameControl.Value &&
                _systemsList.Select(t => t.Name).Contains(_nameControl.Value)) {
                ComponentUtils.ShowMessage("Система с таким названием уже существует", MessageBoxImage.Error);
                return false;
            }

            selected.Name = _nameControl.Value;

            App.DataBase.GetCollection<TransportSystem>().Update(selected);
            ComponentUtils.ShowMessage("Данная транспортная система была обновлена", MessageBoxImage.Information);
            return true;
        }

        private static bool RemoveTransportSystem(TransportSystem selected) {
            App.DataBase.GetCollection<Road>().DeleteMany(r => r.TransportSystemId == selected.Id);
            var cities = App.DataBase.GetCollection<City>().Find(c => c.TransportSystemIds.Contains(selected.Id));
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

        private static IEnumerable<TransportSystem> UpdateCollection() {
            _systemsList = App.DataBase.GetCollection<TransportSystem>().FindAll().ToList();
            return _systemsList;
        }
    }
}