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
                ts => App.DataBase.GetCollection<City>().Count(c => c.TransportSystemId == ts.Id));
            genericEntityDialog.AddColumn(
                "Кол-во маршрутов", 
                ts => App.DataBase.GetCollection<Road>().Count(r => r.TransportSystemId == ts.Id));
            genericEntityDialog.AddColumn(
                "Кол-во доступных типов дорог", 
                ts => ts.Parameters.AvailableRoadTypes.Count);
            
            _nameControl = new StringRowControl() {
                TitleValue = "Название",
                TitleToolTip = "Представляет собой уникальный индетификатор транспортной системы"
            };
            _availableRoadTypesControl = new GenericTableRowControl<string>() {
                TitleValue = "Используемые типы дорог",
                TitleToolTip = "Представляет собой набор допустимых типов маршрутов в данной системе, используется при добавлении или обновлении маршрута",
                OnAdd = roadTypes => {
                    var d = new StringFieldDialog() {
                        Title = "Новый тип дороги",
                        IsViable = newRoadTypeName => {
                            if (newRoadTypeName.Trim() == "") {
                                ComponentUtils.ShowMessage("Введите не пустое название", MessageBoxImage.Error);
                                return false;
                            }

                            if (roadTypes.Contains(newRoadTypeName.Trim())) {
                                ComponentUtils.ShowMessage("Тип дороги с таким названием уже существует",
                                    MessageBoxImage.Error);
                                return false;
                            }

                            return true;
                        }
                    };
                    d.FieldName.Text = "Введите название";
                    d.FieldValue.Text = "";
                    return d.ShowDialog() != true ? null : d.FieldValue.Text;
                }
            };
            _availableRoadTypesControl.AddColumn("Название", s => s);
            
            genericEntityDialog.AddProperty(
                _nameControl, 
                () => _nameControl.Value = "",
                ts => _nameControl.Value = ts.Name);
            genericEntityDialog.AddProperty(
                _availableRoadTypesControl.GetUiElement, 
                () => _availableRoadTypesControl.Value = new List<string>(),
                ts => _availableRoadTypesControl.Value = ts.Parameters.AvailableRoadTypes);
            
            genericEntityDialog.ShowDialog();
        }
        
        private static IList<TransportSystem> _currentSystemList;
        
        private static StringRowControl _nameControl;
        private static GenericTableRowControl<string> _availableRoadTypesControl;
        
        private static bool AddTransportSystem() {
            if (_nameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите название транспортной системы", MessageBoxImage.Error);
                return false;
            }

            if (_currentSystemList.Select(ts => ts.Name).Contains(_nameControl.Value)) {
                ComponentUtils.ShowMessage("Система с таким названием уже существует", MessageBoxImage.Error);
                return false;
            }
            
            App.DataBase.GetCollection<TransportSystem>().Insert(new TransportSystem() {
                Name = _nameControl.Value,
                Parameters = new TransportSystemParameters() {
                    AvailableRoadTypes = _availableRoadTypesControl.Value
                }
            });
            return true;
        }
        
        private static bool UpdateTransportSystem(TransportSystem selected) {
            if (_nameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите не пустое название транспортной системы", MessageBoxImage.Error);
                return false;
            }

            if (selected.Name != _nameControl.Value && 
                _currentSystemList.Select(t => t.Name).Contains(_nameControl.Value)) {
                ComponentUtils.ShowMessage("Система с таким названием уже существует", MessageBoxImage.Error);
                return false;
            }

            selected.Name = _nameControl.Value;
            selected.Parameters.AvailableRoadTypes = _availableRoadTypesControl.Value;
            
            App.DataBase.GetCollection<TransportSystem>().Update(selected);
            ComponentUtils.ShowMessage("Данная транспортная система была обновлена", MessageBoxImage.Information);
            return true;
        }
        
        private static bool RemoveTransportSystem(TransportSystem selected) {
            App.DataBase.GetCollection<TransportSystem>().Delete(selected.Id);
            return true;
        }

        private static IEnumerable<TransportSystem> UpdateCollection() {
            _currentSystemList = App.DataBase.GetCollection<TransportSystem>().FindAll().ToList();
            return _currentSystemList;
        }
    }
}