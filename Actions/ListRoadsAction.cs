using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LiteDB;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Dialogs;
using TransportGraphApp.Models;

namespace TransportGraphApp.Actions {
    public static class ListRoadsAction {
        public static void Invoke() {
            var selectDialog = new SelectTransportSystemDialog();
            if (selectDialog.ShowDialog() != true) return;

            _selectedSystem = selectDialog.SelectedSystem;
            _roadTypes = App.DataBase.GetCollection<RoadTypes>().FindOne(rt => rt.IsPrimary);
            _idToNameCitiesMap = App.DataBase.GetCitiesOfTransportSystem(_selectedSystem)
                .ToDictionary(c => c.Id, c => c.Name);
            _nameToIdCitiesMap = _idToNameCitiesMap
                .ToDictionary(kv => kv.Value, kv => kv.Key);
            
            
            var genericEntityDialog = new GenericEntityDialog<Road>() {
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
            
            genericEntityDialog.AddColumn("Откуда", r => _idToNameCitiesMap[r.FromCityId]);
            genericEntityDialog.AddColumn("Куда", r => _idToNameCitiesMap[r.ToCityId]);
            genericEntityDialog.AddColumn("Расстояние", r => r.Length);
            genericEntityDialog.AddColumn("Стоимость", r => r.Cost);
            genericEntityDialog.AddColumn("Время", r => r.Time);
            genericEntityDialog.AddColumn("Тип дороги", r => r.RoadType.Name);
            genericEntityDialog.AddColumn("Кол.во. значений времени отправленя", r => r.DepartureTimes.Count);
            
            _fromControl = new StringWithHelpRowControl() {
                TitleValue = "Откуда",
                HelpingValues = _nameToIdCitiesMap.Keys.ToList(),
                TitleToolTip = "Представляет собой название нас. пункта откуда будет работать маршрут"
            };
            _toControl = new StringWithHelpRowControl() {
                TitleValue = "Куда",
                HelpingValues = _nameToIdCitiesMap.Keys.ToList(),
                TitleToolTip = "Представляет собой название нас. пункта куда будет работать маршрут"
            };
            _lengthControl = new PositiveDoubleRowControl() {
                TitleValue = "Длинна пути",
                TitleToolTip = "Представляет собой расстояние(в условных единицах), используется в алгоритме основанном на длинне маршрута"
            };
            _costControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проезда",
                TitleToolTip = "Представляет собой стоимость проезда на данном маршруте (в условных денежных единицах), " +
                               "используется в базовом и комплексном алгоритмах основанных на денежных затратах"
            };
            _timeControl = new PositiveDoubleRowControl() {
                TitleValue = "Время в движении (в минутах)",
                TitleToolTip = "Представляет собой время проезда на данном маршруте (в минутах), " +
                               "используется в алгоритме основанном на временных затратах"
            };
            _roadTypeControl = new StringWithHelpRowControl() {
                TitleValue = "Тип дороги",
                TitleToolTip = "Представляет собой один из типов дороги (заполненных в глобальных параметрах), используется в спецификации для фильтрации по типам маршрутов",
                HelpingValues = _roadTypes.Values.Select(rt => rt.Name).ToList()
            };
            _departureTimeTableControl = new GenericTableRowControl<DepartureTime>() {
                TitleValue = "Время отправления",
                TitleToolTip = "Представляет собой расписание отправлений по данному маршруту",
                OnAdd = timeTable => {
                    var d = new DepartureTimeFieldDialog();
                    return d.ShowDialog() != true ? null : d.DepartureTime();
                }
            };
            _departureTimeTableControl.AddColumn("Время отправления", dt => $"{dt.Hour:D2}:{dt.Minute:D2}");
            _departureTimeTableControl.AddColumn("Пн", dt => dt.DaysAvailable.Contains(DayOfWeek.Monday) ? "+" : "-");
            _departureTimeTableControl.AddColumn("Вт", dt => dt.DaysAvailable.Contains(DayOfWeek.Tuesday) ? "+" : "-");
            _departureTimeTableControl.AddColumn("Ср", dt => dt.DaysAvailable.Contains(DayOfWeek.Wednesday) ? "+" : "-");
            _departureTimeTableControl.AddColumn("Чт", dt => dt.DaysAvailable.Contains(DayOfWeek.Thursday) ? "+" : "-");
            _departureTimeTableControl.AddColumn("Пт", dt => dt.DaysAvailable.Contains(DayOfWeek.Friday) ? "+" : "-");
            _departureTimeTableControl.AddColumn("Сб", dt => dt.DaysAvailable.Contains(DayOfWeek.Saturday) ? "+" : "-");
            _departureTimeTableControl.AddColumn("Вс", dt => dt.DaysAvailable.Contains(DayOfWeek.Sunday) ? "+" : "-");
            
            genericEntityDialog.AddProperty(
                _fromControl,
                () => _fromControl.Value = "",
                r => _fromControl.Value = _idToNameCitiesMap[r.FromCityId]);
            
            genericEntityDialog.AddProperty(
                _toControl,
                () => _toControl.Value = "",
                r => _toControl.Value = _idToNameCitiesMap[r.ToCityId]);

            genericEntityDialog.AddProperty(
                _lengthControl,
                () => _lengthControl.Value = 0,
                r => _lengthControl.Value = r.Length);
            
            genericEntityDialog.AddProperty(
                _costControl,
                () => _costControl.Value = 0,
                r => _costControl.Value = r.Cost);
            
            genericEntityDialog.AddProperty(
                _timeControl,
                () => _timeControl.Value = 0,
                r => _timeControl.Value = r.Time);
            
            genericEntityDialog.AddProperty(
                _roadTypeControl,
                () => _roadTypeControl.Value = "",
                r => _roadTypeControl.Value = r.RoadType.Name);
            
            genericEntityDialog.AddProperty(
                _departureTimeTableControl.GetUiElement,
                () => _departureTimeTableControl.Value = new List<DepartureTime>(),
                r => _departureTimeTableControl.Value = r.DepartureTimes);
            
            genericEntityDialog.ShowDialog();
        }
        
        private static TransportSystem _selectedSystem;
        private static RoadTypes _roadTypes;
        private static IDictionary<ObjectId, string> _idToNameCitiesMap;
        private static IDictionary<string, ObjectId> _nameToIdCitiesMap;
        
        private static IList<Road> _roadList;

        private static StringWithHelpRowControl _fromControl;
        private static StringWithHelpRowControl _toControl;
        private static PositiveDoubleRowControl _lengthControl;
        private static PositiveDoubleRowControl _costControl;
        private static PositiveDoubleRowControl _timeControl;
        private static StringWithHelpRowControl _roadTypeControl;
        private static GenericTableRowControl<DepartureTime> _departureTimeTableControl; 

        private static bool AddRoad() {
            if (_fromControl.Value == _toControl.Value) {
                ComponentUtils.ShowMessage("Поля \"Откуда\" и \"Куда\" должны представлять названия разных населенных пунктов", MessageBoxImage.Error);
                return false;
            }

            if (!_nameToIdCitiesMap.ContainsKey(_fromControl.Value) || 
                !_nameToIdCitiesMap.ContainsKey(_toControl.Value)) {
                ComponentUtils.ShowMessage("Поля \"Откуда\" и \"Куда\" должны представлять названия населенных пунктов", MessageBoxImage.Error);
                return false;
            }

            if (!_roadTypes.Values.Select(rt => rt.Name).Contains(_roadTypeControl.Value)) {
                ComponentUtils.ShowMessage("Поле \"Тип дороги\" должен представлять название одного из типа дорог которые были объявленны в глобальных параметрах сети", MessageBoxImage.Error);
                return false;
            }

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
            if (_fromControl.Value == _toControl.Value) {
                ComponentUtils.ShowMessage("Поля \"Откуда\" и \"Куда\" должны представлять названия разных населенных пунктов", MessageBoxImage.Error);
                return false;
            }
            
            if (!_nameToIdCitiesMap.ContainsKey(_fromControl.Value) || 
                !_nameToIdCitiesMap.ContainsKey(_toControl.Value)) {
                ComponentUtils.ShowMessage("Поля \"Откуда\" и \"Куда\" должны представлять названия населенных пунктов", MessageBoxImage.Error);
                return false;
            }

            if (!_roadTypes.Values.Select(rt => rt.Name).Contains(_roadTypeControl.Value)) {
                ComponentUtils.ShowMessage("Поле \"Тип дороги\" должен представлять название одного из типа дорог которые были объявленны в глобальных параметрах сети", MessageBoxImage.Error);
                return false;
            }
            
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

        private static IEnumerable<Road> UpdateCollection() {
            _roadList = App.DataBase
                .GetCollection<Road>()
                .Find(r => r.TransportSystemId == _selectedSystem.Id)
                .ToList();
            return _roadList;
        }
    }
}