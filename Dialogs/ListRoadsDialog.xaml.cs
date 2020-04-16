using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiteDB;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs {
    public partial class ListRoadsDialog : Window {
        private IList<Road> _currentRoadsList;
        private TransportSystem _selectedTransportSystem;
        private IDictionary<ObjectId, string> _idToNameCitiesMap;
        private IDictionary<string, ObjectId> _nameToIdCitiesMap;

        private GenericEntityListControl<Road> _entityList;

        private StackPanel _newPanel;
        private StringWithHelpRowControl _newFromControl;
        private StringWithHelpRowControl _newToControl;
        private PositiveDoubleRowControl _newLengthControl;
        private PositiveDoubleRowControl _newCostControl;
        private PositiveDoubleRowControl _newTimeControl;
        private StringWithHelpRowControl _newRoadTypeControl;
        private DepartureTimeTableControl _newDepartureTimeTableControl;

        private StackPanel _updatePanel;
        private StringWithHelpRowControl _updateFromControl;
        private StringWithHelpRowControl _updateToControl;
        private PositiveDoubleRowControl _updateLengthControl;
        private PositiveDoubleRowControl _updateCostControl;
        private PositiveDoubleRowControl _updateTimeControl;
        private StringWithHelpRowControl _updateRoadTypeControl;
        private DepartureTimeTableControl _updateDepartureTimeTableControl;

        public ListRoadsDialog(TransportSystem transportSystem) {
            InitializeComponent();
            Owner = App.Window;
            Icon = AppResources.GetAppIcon;
            _selectedTransportSystem = transportSystem;
            
            // _idToNameCitiesMap = App.DataBase.GetCollection<City>()
            //     .Find(c => c.TransportSystemId == _selectedTransportSystem.Id)
            //     .ToDictionary(c => c.Id, c => c.Name);
            _nameToIdCitiesMap = _idToNameCitiesMap
                .ToDictionary(kv => kv.Value, kv => kv.Key);
            
            var propertyMatcher = new Dictionary<string, Func<Road, object>> {
                {"Откуда", r => _idToNameCitiesMap[r.FromCityId]}, 
                {"Куда", r =>  _idToNameCitiesMap[r.ToCityId]}, 
                {"Расстояние", r => r.Length}, 
                {"Стоимость", r => r.Cost}, 
                {"Время", r => r.Time}, 
                {"Тип дороги", r => r.RoadType},
                {"Кол.во. значений времени отправленя", r => r.DepartureTimes.Count}
            };

            _entityList = new GenericEntityListControl<Road>(
                "Доступные маршруты",
                propertyMatcher,
                DisplayUpdate);

            SetUpNewPropertiesPanel();
            SetUpUpdatePropertiesPanel();
            PropertiesPanel.Visibility = Visibility.Collapsed;
            ListPanel.Children.Add(_entityList.GetUiElement());
            
            ComponentUtils.InsertIconToButton(AddButton, AppResources.GetAddItemIcon, "Открыть окно для добавления транспортной системы");
            AddButton.Click += (sender, args) => DisplayNew();
            
            UpdateState();
        }
        
        private void SetUpNewPropertiesPanel() {
            _newPanel = new StackPanel();
            var label = new Label() {
                Margin = new Thickness(5, 5, 5, 5),
                Content = "Добавить маршрут"
            };
            _newFromControl = new StringWithHelpRowControl() {
                TitleValue = "Откуда",
                HelpingValues = _nameToIdCitiesMap.Keys.ToList()
            };
            _newToControl = new StringWithHelpRowControl() {
                TitleValue = "Куда",
                HelpingValues = _nameToIdCitiesMap.Keys.ToList()
            };
            _newLengthControl = new PositiveDoubleRowControl() {
                TitleValue = "Длинна пути (в условных единицах)"
            };
            _newCostControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проезда на данном маршруте (в условных единицах)"
            };
            _newTimeControl = new PositiveDoubleRowControl() {
                TitleValue = "Время в движении (в минутах)"
            };
            // _newRoadTypeControl = new StringWithHelpRowControl() {
            //     TitleValue = "Тип дороги",
            //     HelpingValues = _selectedTransportSystem.Parameters.AvailableRoadTypes
            // };
            _newDepartureTimeTableControl = new DepartureTimeTableControl();
            var addButton = new Button() {
                Margin = new Thickness(5,5,5,5),
                Content = "Добавить",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            addButton.Click += (sender, args) => AddRoad();

            _newPanel.Children.Add(label);
            _newPanel.Children.Add(_newFromControl);
            _newPanel.Children.Add(_newToControl);
            _newPanel.Children.Add(_newLengthControl);
            _newPanel.Children.Add(_newCostControl);
            _newPanel.Children.Add(_newTimeControl);
            _newPanel.Children.Add(_newRoadTypeControl);
            _newPanel.Children.Add(_newDepartureTimeTableControl);
            _newPanel.Children.Add(new Separator() {
                Margin = new Thickness(5,5,5,5),
            });
            _newPanel.Children.Add(addButton);
        }
        
        private void SetUpUpdatePropertiesPanel() {
            _updatePanel = new StackPanel();
            var label = new Label() {
                Margin = new Thickness(5, 5, 5, 5),
                Content = "Обновить маршрут"
            };
            _updateFromControl = new StringWithHelpRowControl() {
                TitleValue = "Откуда",
                HelpingValues = _nameToIdCitiesMap.Keys.ToList()
            };
            _updateToControl = new StringWithHelpRowControl() {
                TitleValue = "Куда",
                HelpingValues = _nameToIdCitiesMap.Keys.ToList()
            };
            _updateLengthControl = new PositiveDoubleRowControl() {
                TitleValue = "Длинна пути (в условных единицах)"
            };
            _updateCostControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проезда на данном маршруте (в условных единицах)"
            };
            _updateTimeControl = new PositiveDoubleRowControl() {
                TitleValue = "Время в движении (в минутах)"
            };
            // _updateRoadTypeControl = new StringWithHelpRowControl() {
            //     TitleValue = "Тип дороги",
            //     HelpingValues = _selectedTransportSystem.Parameters.AvailableRoadTypes
            // };
            _updateDepartureTimeTableControl = new DepartureTimeTableControl();
            var buttonPanel = new WrapPanel() {
                Margin = new Thickness(5, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var removeButton = new Button() {
                Content = "Удалить",
                Margin = new Thickness(0, 0, 5, 0),
            };
            removeButton.Click += (sender, args) => RemoveRoad();
            var updateButton = new Button() {
                Content = "Обновить"
            };
            updateButton.Click += (sender, args) => UpdateRoad();
            buttonPanel.Children.Add(removeButton);
            buttonPanel.Children.Add(updateButton);

            _updatePanel.Children.Add(label);
            _updatePanel.Children.Add(_updateFromControl);
            _updatePanel.Children.Add(_updateToControl);
            _updatePanel.Children.Add(_updateLengthControl);
            _updatePanel.Children.Add(_updateCostControl);
            _updatePanel.Children.Add(_updateTimeControl);
            _updatePanel.Children.Add(_updateRoadTypeControl);
            _updatePanel.Children.Add(_updateDepartureTimeTableControl);
            _updatePanel.Children.Add(new Separator() {
                Margin = new Thickness(5,5,5,5),
            });
            _updatePanel.Children.Add(buttonPanel);
        }
        
        private void DisplayNew() {
            _newFromControl.Value = "";
            _newToControl.Value = "";
            _newLengthControl.Value = 0;
            _newCostControl.Value = 0;
            _newTimeControl.Value = 0;
            _newRoadTypeControl.Value = "";
            _updateDepartureTimeTableControl.Value = new List<DepartureTime>();
            _entityList.Selected = null;
            
            PropertiesPanel.Children.Clear();
            PropertiesPanel.Children.Add(_newPanel);
            PropertiesPanel.Visibility = Visibility.Visible;
        }
        
        private void DisplayUpdate(Road r) {
            _updateFromControl.Value = _idToNameCitiesMap[r.FromCityId];
            _updateToControl.Value = _idToNameCitiesMap[r.ToCityId];
            _updateLengthControl.Value = r.Length;
            _updateCostControl.Value = r.Cost;
            _updateTimeControl.Value = r.Time;
            // _updateRoadTypeControl.Value = r.RoadType;
            _updateDepartureTimeTableControl.Value = r.DepartureTimes;

            PropertiesPanel.Children.Clear();
            PropertiesPanel.Children.Add(_updatePanel);
            PropertiesPanel.Visibility = Visibility.Visible;
        }
        
        private void AddRoad() {
            if (!_nameToIdCitiesMap.ContainsKey(_newFromControl.Value) || 
                !_nameToIdCitiesMap.ContainsKey(_newToControl.Value)) {
                ComponentUtils.ShowMessage("Поля \"Откуда\" и \"Куда\" должны представлять названия населенных пунктов", MessageBoxImage.Error);
                return;
            }

            // if (!_selectedTransportSystem.Parameters.AvailableRoadTypes.Contains(_newRoadTypeControl.Value)) {
            //     ComponentUtils.ShowMessage("Поле \"Тип дороги\" должен представлять название одного из типа дорог которые были объявленны в транспортной сети", MessageBoxImage.Error);
            //     return;
            // }
            
            App.DataBase.GetCollection<Road>().Insert(new Road() {
                FromCityId = _nameToIdCitiesMap[_newFromControl.Value],
                ToCityId = _nameToIdCitiesMap[_newToControl.Value],
                Length = _newLengthControl.Value,
                Cost = _newCostControl.Value,
                Time = _newTimeControl.Value,
                // RoadType = _newRoadTypeControl.Value,
                DepartureTimes = _newDepartureTimeTableControl.Value,
                TransportSystemId = _selectedTransportSystem.Id
            });
            UpdateState();
            DisplayNew();
        }
        
        private void UpdateRoad() {
            var selected = _entityList.Selected;
            if (!_nameToIdCitiesMap.ContainsKey(_updateFromControl.Value) || 
                !_nameToIdCitiesMap.ContainsKey(_updateToControl.Value)) {
                ComponentUtils.ShowMessage("Поля \"Откуда\" и \"Куда\" должны представлять названия населенных пунктов", MessageBoxImage.Error);
                return;
            }

            // if (!_selectedTransportSystem.Parameters.AvailableRoadTypes.Contains(_updateRoadTypeControl.Value)) {
            //     ComponentUtils.ShowMessage("Поле \"Тип дороги\" должен представлять название одного из типа дорог которые были объявленны в транспортной сети", MessageBoxImage.Error);
            //     return;
            // }

            selected.FromCityId = _nameToIdCitiesMap[_updateFromControl.Value];
            selected.ToCityId = _nameToIdCitiesMap[_updateToControl.Value];
            selected.Length = _updateLengthControl.Value;
            selected.Cost = _updateCostControl.Value;
            selected.Time = _updateTimeControl.Value;
            // selected.RoadType = _updateRoadTypeControl.Value;
            selected.DepartureTimes = _updateDepartureTimeTableControl.Value;

            App.DataBase.GetCollection<Road>().Update(selected);
            UpdateState();
            
            _entityList.Selected = _currentRoadsList.First(r => r.Id == selected.Id);
            ComponentUtils.ShowMessage("Данный маршрут был обновлен", MessageBoxImage.Information);
        }
        
        private void RemoveRoad() {
            var selected = _entityList.Selected;
            App.DataBase.GetCollection<Road>().Delete(selected.Id);
            UpdateState();
            DisplayNew();
        }
        
        private void UpdateState() {
            _currentRoadsList = App.DataBase
                .GetCollection<Road>()
                .Find(r => r.TransportSystemId == _selectedTransportSystem.Id)
                .ToList();
            _entityList.SetSource(_currentRoadsList);
        }
    }
}