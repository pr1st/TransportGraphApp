using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TransportGraphApp.CustomComponents;
using TransportGraphApp.Models;
using TransportGraphApp.Singletons;

namespace TransportGraphApp.Dialogs {
    public partial class ListCitiesDialog : Window {
        private IList<City> _currentCitiesList;

        private GenericEntityListControl<City> _entityList;

        private StackPanel _newPanel;
        private StringRowControl _newNameControl;
        private LatitudeLongitudeRowControl _newCooridnatesControl;
        private PositiveDoubleRowControl _newCostOfStayingControl;

        private StackPanel _updatePanel;
        private StringRowControl _updateNameControl;
        private LatitudeLongitudeRowControl _updateCooridnatesControl;
        private PositiveDoubleRowControl _updateCostOfStayingControl;
        
        public ListCitiesDialog() {
            InitializeComponent();
            Owner = AppWindow.Instance;
            
            var propertyMatcher = new Dictionary<string, Func<City, object>> {
                {"Название", c => c.Name}, 
                {"Широта", c => c.Latitude}, 
                {"Долгота", c => c.Longitude},
                {"Стоимость проживания", c => c.CostOfStaying},
            };

            _entityList = new GenericEntityListControl<City>(
                "Список доступных населенных пунктов",
                propertyMatcher,
                DisplayNew,
                DisplayUpdate);

            SetUpNewPropertiesPanel();
            SetUpUpdatePropertiesPanel();
            PropertiesPanel.Visibility = Visibility.Collapsed;
            ListPanel.Children.Add(_entityList.GetUiElement());
            UpdateState();
        }
        
        private void SetUpNewPropertiesPanel() {
            _newPanel = new StackPanel();
            var label = new Label() {
                Margin = new Thickness(5, 5, 5, 5),
                Content = "Добавить населенный пункт"
            };
            _newNameControl = new StringRowControl() {
                TitleValue = "Название"
            };
            _newCooridnatesControl = new LatitudeLongitudeRowControl();
            _newCostOfStayingControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проживания в данном населенном пункте (условных единиц в день)"
            };
            var addButton = new Button() {
                Margin = new Thickness(5,5,5,5),
                Content = "Добавить",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            addButton.Click += (sender, args) => AddCity();

            _newPanel.Children.Add(label);
            _newPanel.Children.Add(_newNameControl);
            _newPanel.Children.Add(_newCooridnatesControl);
            _newPanel.Children.Add(_newCostOfStayingControl);
            _newPanel.Children.Add(addButton);
        }
        
        private void SetUpUpdatePropertiesPanel() {
            _updatePanel = new StackPanel();
            var label = new Label() {
                Margin = new Thickness(5, 5, 5, 5),
                Content = "Обновить населенный пункт"
            };
            _updateNameControl = new StringRowControl() {
                TitleValue = "Название"
            };
            _updateCooridnatesControl = new LatitudeLongitudeRowControl();
            _updateCostOfStayingControl = new PositiveDoubleRowControl() {
                TitleValue = "Стоимость проживания в данном населенном пункте (условных единиц в день)"
            };
            var buttonPanel = new WrapPanel() {
                Margin = new Thickness(5, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var removeButton = new Button() {
                Content = "Удалить",
                Margin = new Thickness(0, 0, 5, 0),
            };
            removeButton.Click += (sender, args) => RemoveCity();
            var updateButton = new Button() {
                Content = "Обновить"
            };
            updateButton.Click += (sender, args) => UpdateCity();
            buttonPanel.Children.Add(removeButton);
            buttonPanel.Children.Add(updateButton);

            _updatePanel.Children.Add(label);
            _updatePanel.Children.Add(_updateNameControl);
            _updatePanel.Children.Add(_updateCooridnatesControl);
            _updatePanel.Children.Add(_updateCostOfStayingControl);
            _updatePanel.Children.Add(buttonPanel);
        }
        
        private void DisplayNew() {
            _newNameControl.Value = "";
            _newCooridnatesControl.Latitude = 0;
            _newCooridnatesControl.Longitude = 0;
            _newCostOfStayingControl.Value = 0;
            _entityList.Selected = null;
            
            PropertiesPanel.Children.Clear();
            PropertiesPanel.Children.Add(_newPanel);
            PropertiesPanel.Visibility = Visibility.Visible;
        }
        
        private void DisplayUpdate(City c) {
            _updateNameControl.Value = c.Name;
            _updateCooridnatesControl.Latitude = c.Latitude;
            _updateCooridnatesControl.Longitude = c.Longitude;
            _updateCostOfStayingControl.Value = c.CostOfStaying;
            
            PropertiesPanel.Children.Clear();
            PropertiesPanel.Children.Add(_updatePanel);
            PropertiesPanel.Visibility = Visibility.Visible;
        }
        
        private void AddCity() {
            if (_newNameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите название населенного пункта", MessageBoxImage.Error);
                return;
            }

            if (_currentCitiesList.Select(с => с.Name).Contains(_newNameControl.Value)) {
                ComponentUtils.ShowMessage("Населенный пункт с таким названием уже существует", MessageBoxImage.Error);
                return;
            }
            
            AppDataBase.Instance.GetCollection<City>().Insert(new City() {
                Name = _newNameControl.Value,
                Latitude = _newCooridnatesControl.Latitude,
                Longitude = _newCooridnatesControl.Longitude,
                CostOfStaying = _newCostOfStayingControl.Value,
                TransportSystemId = AppGraph.Instance.GetSelectedSystem.Id
            });
            UpdateState();
            DisplayNew();
        }
        
        private void UpdateCity() {
            var selected = _entityList.Selected;
            if (_updateNameControl.Value == "") {
                ComponentUtils.ShowMessage("Введите не пустое название населенного пункта", MessageBoxImage.Error);
                return;
            }

            if (selected.Name != _updateNameControl.Value && 
                _currentCitiesList.Select(с => с.Name).Contains(_updateNameControl.Value)) {
                ComponentUtils.ShowMessage("Населенный пункт с таким названием уже существует", MessageBoxImage.Error);
                return;
            }

            selected.Name = _updateNameControl.Value;
            selected.Latitude = _updateCooridnatesControl.Latitude;
            selected.Longitude = _updateCooridnatesControl.Longitude;
            selected.CostOfStaying = _updateCostOfStayingControl.Value;
            
            AppDataBase.Instance.GetCollection<City>().Update(selected);
            UpdateState();
            
            _entityList.Selected = _currentCitiesList.First(c => c.Id == selected.Id);
            ComponentUtils.ShowMessage("Данный населенный пункт был обновлен", MessageBoxImage.Information);
        }
        
        private void RemoveCity() {
            var selected = _entityList.Selected;
            AppDataBase.Instance.GetCollection<City>().Delete(selected.Id);
            UpdateState();
            DisplayNew();
        }
        
        private void UpdateState() {
            _currentCitiesList = AppDataBase.Instance
                .GetCollection<City>()
                .Find(c => c.TransportSystemId == AppGraph.Instance.GetSelectedSystem.Id)
                .ToList();
            _entityList.SetSource(_currentCitiesList);
        }
        
        private void CancelClick(object sender, RoutedEventArgs e) {
            AppGraph.Instance.UpdateSystem();
            DialogResult = true;
        }
    }
}