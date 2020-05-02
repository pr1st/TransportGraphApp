using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LiteDB;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Dialogs.ResultDialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class ListRoadsAction {
        // unmodifiable properties
        private static TransportSystem _selectedSystem;
        private static RoadTypes _availableRoadTypes;
        private static IDictionary<ObjectId, string> _idToNameCitiesMap;
        private static IDictionary<string, ObjectId> _nameToIdCitiesMap;
        
        // updateable properties
        private static IList<Road> _roadList;

        // ui controls
        private static GenericEntityDialog<Road> _dialog;
        private static StringWithHelpRowControl _fromControl;
        private static StringWithHelpRowControl _toControl;
        private static PositiveDoubleRowControl _lengthControl;
        private static PositiveDoubleRowControl _costControl;
        private static PositiveDoubleRowControl _timeControl;
        private static StringWithHelpRowControl _roadTypeControl;
        private static GenericTableRowControl<DepartureTime> _departureTimeTableControl; 
        
        public static void Invoke() {
            var selectDialog = new GenericSelectEntitiesDialog<TransportSystem>(
                "Выберите транспортную систему",
                TransportSystem.PropertyMatcher(),
                App.DataBase.GetCollection<TransportSystem>().FindAll());
            if (selectDialog.Selected == null) return;
            _selectedSystem = selectDialog.Selected[0];
            
            _availableRoadTypes = App.DataBase.GetCollection<RoadTypes>().FindOne(ct => ct.IsPrimary);
            
            _idToNameCitiesMap = App.DataBase.GetCitiesOfTransportSystem(_selectedSystem)
                .ToDictionary(c => c.Id, c => c.Name);
            _nameToIdCitiesMap = _idToNameCitiesMap
                .ToDictionary(kv => kv.Value, kv => kv.Key);
            
            _dialog = new GenericEntityDialog<Road>() {
                Title = "Маршруты",
                ListTitle = "Доступные маршруты",
                OpenAddNewItemWindowButtonTitle = "Открыть окно для добавления маршрута",
                AddNewItemWindowTitle = "Добавить маршрут",
                UpdateItemWindowTitle = "Обновить маршрут",
                AddItemFunction = AddRoad,
                UpdateItemFunction = UpdateRoad,
                RemoveItemFunction = RemoveRoad,
                UpdateCollectionFunction = UpdateCollection
            };
            
            _dialog.AddColumns(Road.PropertyMatcher(_idToNameCitiesMap));

            InitFromCityProperty();
            InitToCityProperty();
            InitLengthProperty();
            InitCostProperty();
            InitTimeProperty();
            InitRoadTypeProperty();
            InitTimeTableProperty();

            UpdateCollection();
            _dialog.ShowDialog();
        }
        
        // callback methods
        private static bool AddRoad() {
            if (!IsViable()) return false;

            App.DataBase.GetCollection<Road>().Insert(new Road() {
                FromCityId = _nameToIdCitiesMap[_fromControl.Value],
                ToCityId = _nameToIdCitiesMap[_toControl.Value],
                Length = _lengthControl.Value,
                Cost = _costControl.Value,
                Time = _timeControl.Value,
                RoadType = new RoadType() {Name = _roadTypeControl.Value},
                DepartureTimes = _departureTimeTableControl.Value,
                TransportSystemId = _selectedSystem.Id
            });
            return true;
        }

        private static bool UpdateRoad(Road selectedRoad) {
            if (!IsViable()) return false;
            
            selectedRoad.FromCityId = _nameToIdCitiesMap[_fromControl.Value];
            selectedRoad.ToCityId = _nameToIdCitiesMap[_toControl.Value];
            selectedRoad.Length = _lengthControl.Value;
            selectedRoad.Cost = _costControl.Value;
            selectedRoad.Time = _timeControl.Value;
            selectedRoad.RoadType.Name = _roadTypeControl.Value;
            selectedRoad.DepartureTimes = _departureTimeTableControl.Value;
            
            App.DataBase.GetCollection<Road>().Update(selectedRoad);
            ComponentUtils.ShowMessage("Данный маршрут был обновлен", MessageBoxImage.Information);
            return true;
        }

        private static bool RemoveRoad(Road selectedRoad) {
            App.DataBase.GetCollection<Road>().Delete(selectedRoad.Id);
            return true;
        }
        
        // support method for callback methods
        private static bool IsViable() {
            if (_fromControl.Value == _toControl.Value) {
                ComponentUtils.ShowMessage("Поля \"Откуда\" и \"Куда\" должны представлять названия разных населенных пунктов", MessageBoxImage.Error);
                return false;
            }
            
            if (!_nameToIdCitiesMap.ContainsKey(_fromControl.Value) || 
                !_nameToIdCitiesMap.ContainsKey(_toControl.Value)) {
                ComponentUtils.ShowMessage("Поля \"Откуда\" и \"Куда\" должны представлять названия населенных пунктов", MessageBoxImage.Error);
                return false;
            }

            if (!_availableRoadTypes.Values.Contains(new RoadType() { Name = _roadTypeControl.Value })) {
                ComponentUtils.ShowMessage("Поле \"Тип дороги\" должен представлять название одного из типа дорог которые были объявленны в глобальных параметрах сети", MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        // init methods
        private static void InitFromCityProperty() {
            _fromControl = new StringWithHelpRowControl() {
                TitleValue = "Откуда",
                HelpingValues = _nameToIdCitiesMap.Keys.ToList(),
                TitleToolTip = "Представляет собой название нас. пункта откуда будет работать маршрут"
            };
            _dialog.AddProperty(
                _fromControl,
                () => _fromControl.Value = "",
                r => _fromControl.Value = _idToNameCitiesMap[r.FromCityId]);
        }
        
        private static void InitToCityProperty() {
            _toControl = new StringWithHelpRowControl() {
                TitleValue = "Куда",
                HelpingValues = _nameToIdCitiesMap.Keys.ToList(),
                TitleToolTip = "Представляет собой название нас. пункта куда будет работать маршрут"
            };
            _dialog.AddProperty(
                _toControl,
                () => _toControl.Value = "",
                r => _toControl.Value = _idToNameCitiesMap[r.ToCityId]);
        }
        
        private static void InitLengthProperty() {
            _lengthControl = new PositiveDoubleRowControl() {
                TitleValue = "Длинна пути",
                TitleToolTip = "Представляет собой расстояние(в условных единицах), используется в алгоритме основанном на длинне маршрута"
            };
            _dialog.AddProperty(
                _lengthControl,
                () => _lengthControl.Value = 0,
                r => _lengthControl.Value = r.Length);
        }
        
        private static void InitCostProperty() {
            _costControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проезда",
                TitleToolTip = "Представляет собой стоимость проезда на данном маршруте (в условных денежных единицах), " +
                               "используется в базовом и комплексном алгоритмах основанных на денежных затратах"
            };
            _dialog.AddProperty(
                _costControl,
                () => _costControl.Value = 0,
                r => _costControl.Value = r.Cost);
        }
        
        private static void InitTimeProperty() {
            _timeControl = new PositiveDoubleRowControl() {
                TitleValue = "Время в движении (в минутах)",
                TitleToolTip = "Представляет собой время проезда на данном маршруте (в минутах), " +
                               "используется в алгоритме основанном на временных затратах"
            };
            _dialog.AddProperty(
                _timeControl,
                () => _timeControl.Value = 0,
                r => _timeControl.Value = r.Time);
        }
        
        private static void InitRoadTypeProperty() {
            _roadTypeControl = new StringWithHelpRowControl() {
                TitleValue = "Тип дороги",
                TitleToolTip = "Представляет собой один из типов дороги (заполненных в глобальных параметрах), используется в спецификации для фильтрации по типам маршрутов",
                HelpingValues = _availableRoadTypes.Values.Select(rt => rt.Name).ToList()
            };
            _dialog.AddProperty(
                _roadTypeControl,
                () => _roadTypeControl.Value = "",
                r => _roadTypeControl.Value = r.RoadType.Name);
        }
        
        private static void InitTimeTableProperty() {
            _departureTimeTableControl = new GenericTableRowControl<DepartureTime>() {
                TitleValue = "Время отправления",
                TitleToolTip = "Представляет собой расписание отправлений по данному маршруту",
                OnAdd = timeTable => {
                    var addDialog = new AddDepartureTimeDialog();
                    
                    if (addDialog.ShowDialog() != true) return null;

                    return new List<DepartureTime>() {addDialog.DepartureTime()};
                }
            };
            _departureTimeTableControl.AddColumns(DepartureTime.PropertyMatcher());
            
            _dialog.AddProperty(
                _departureTimeTableControl.GetUiElement,
                () => _departureTimeTableControl.Value = new List<DepartureTime>(),
                r => _departureTimeTableControl.Value = r.DepartureTimes);
        }
        
        // update state method
        private static IEnumerable<Road> UpdateCollection() {
            _roadList = App.DataBase
                .GetCollection<Road>()
                .Find(r => r.TransportSystemId == _selectedSystem.Id)
                .ToList();
            return _roadList;
        }
    }
}